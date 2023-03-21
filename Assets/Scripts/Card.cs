using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum State
{
    unflipped,
    flipped,
    solved
}

public class Card : MonoBehaviour, IPointerClickHandler
{
    public GameController game;
    public SpriteRenderer backImage;
    public State state = State.unflipped;

    //flip over card on click
    public void OnPointerClick(PointerEventData eventData)
    {
        if (state == State.unflipped)
            StartCoroutine(Flip());
    }

    public IEnumerator Flip(bool bypass = false)
    {
        //if a card can be flipped, and this card is not solved
        if ((game.canFlip || bypass) && state != State.solved)
        {
            //lock out other cards from flipping
            game.canFlip = false;

            //rotate card 180 degrees
            Vector3 startingRotation = transform.rotation.eulerAngles;
            for (int i = 0; i < 180; i++)
            {
                transform.rotation = Quaternion.Euler(startingRotation + new Vector3(0, i, 0));
                yield return new WaitForSeconds(0.005f);
            }

            if (state == State.unflipped)
            {
                //set currently flipped card
                state = State.flipped;
                game.currentlyFlipped = this;
            }
            else if (state == State.flipped)
                state = State.unflipped;

            //allow another card to flip
            game.canFlip = true;
        }
    }

    public IEnumerator moveToPosition(Vector3 pos)
    {
        Vector3 direction = pos - transform.position;
        int totalSteps = 60; //more totalSteps = more granular movement, less totalSteps = less calculations
        for (int i = 0; i < totalSteps; i++)
        {
            transform.position += direction / totalSteps;
            yield return new WaitForSeconds(1f / totalSteps);
        }
    }


    public void setSprite(Sprite sprite)
    {
        backImage.sprite = sprite;
    }
}
