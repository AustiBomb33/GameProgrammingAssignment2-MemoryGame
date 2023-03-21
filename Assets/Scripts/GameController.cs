using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    int cols = 4, rows = 4;
    float X_Gap = 2f, X_Start = -2f, Y_Gap = 2f, Y_Start = -2f;
    public GameObject cardPrefab;
    public Sprite[] sprites;
    public Canvas EndGameCanvas;
    public Text TimeText;

    private float startTime;
    private List<Card> cards;
    private Card _currentlyFlipped;
    private int score;

    /**
     * When currentlyFlipped is set to a new card, it checks if another unsolved card is flipped.
     * If so, it compares their image to see if they match, either locking them or flipping them back over, then resetting _currentlyFlipped to null
     * If there's no unsolved card currently flipped, it simply stores the flipped card.
     */
    public Card currentlyFlipped
    {
        set
        {
            //check if there is a card currently flipped
            if (_currentlyFlipped)
            {
                //if the cards match
                if (_currentlyFlipped.backImage.sprite == value.backImage.sprite)
                {
                    //lock both cards as solved
                    _currentlyFlipped.state = State.solved;
                    value.state = State.solved;

                    //reset _currentlyFlipped
                    _currentlyFlipped = null;
                    //increment score & check for win
                    score++;
                    if (score >= cards.Count / 2)
                    {
                        TimeText.text = string.Format("Time: {0}:{1:0.##}", Mathf.Floor((Time.time - startTime) / 60), (Time.time - startTime) % 60);
                        EndGameCanvas.gameObject.SetActive(true);
                    }
                }
                else
                {
                    //flip both cards back over
                    canFlip = false;
                    _currentlyFlipped.StartCoroutine(_currentlyFlipped.Flip(true));
                    value.StartCoroutine(value.Flip(true));
                    //reset _currentlyFlipped
                    _currentlyFlipped = null;
                }
            }
            //if there is no card currently flipped
            else
            {
                //save the flipped card
                _currentlyFlipped = value;
            }
        }
    }
    public bool canFlip = true;

    // Start is called before the first frame update
    void Start()
    {
        StartGame();
    }

    public void RestartGame()
    {
        EndGameCanvas.gameObject.SetActive(false);
        foreach (Card card in cards)
        {
            Destroy(card.gameObject);
        }
        StartGame();

    }
    public void StartGame()
    {
        //Randomly locate sprites
        List<int> selectedCards = new List<int>();
        for (int i = 0; i < (cols * rows) / 2; i++)
        {
            //this works for a 4x4 grid. for arbitrary sizes, I would select a random index and insert that twice, as opposed to inserting i twice
            //insert twice at random points in list
            selectedCards.Insert(Random.Range(0, selectedCards.Count), i);
            selectedCards.Insert(Random.Range(0, selectedCards.Count), i);
        }

        //Generate Cards   
        cards = new List<Card>();
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                //calculate point for card to spawn at
                Vector3 pos = new Vector3(x * X_Gap + X_Start, y * Y_Gap + Y_Start, 0);
                Quaternion rot = Quaternion.Euler(0, 0, 0);
                //spawn card
                Card card = Instantiate(cardPrefab, new Vector3(-4, 2, 0), rot).GetComponentInChildren<Card>();
                card.StartCoroutine(card.moveToPosition(pos));
                //set sprite
                card.setSprite(sprites[selectedCards[x * rows + y]]);
                //set GameController reference
                card.game = this;
                //add to list of cards
                cards.Add(card);
            }
            startTime = Time.time + 1; //+1 to account for 1 second card movement
            score = 0;
        }
    }
}
