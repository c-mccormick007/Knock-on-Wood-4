using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BandCproductions;
using System;
using DG.Tweening;

public class HandManager : MonoBehaviour
{
    public DeckManager deckManager;
    public GameObject cardPrefab; // Assign Card Prefab Here
    public Transform deckPilePos;
    public Transform handTransform; //Root of the Hand Position
    public float fanSpread = -5.03f; //Fan spread

    public DeckPile deckPile;

    public float cardSpacing = 75.43f; //Card Spacing
    public float verticalSpacing = 44.8f;
    public List<GameObject> cardsInHand = new List<GameObject>(); //List of card objects
    public float tweenDuration = 0.02f;

    void Start()
    {
    }

    private void Update()
    {
        //UpdateHandVisuals();
    }

    public void AddTopDeckCardToHand()
    {
        // 1. Get the top card from the deck pile’s list
        int topIndex = deckPile.deckObjects.Count - 1;
        if (topIndex < 0) return;

        GameObject topCard = deckPile.deckObjects[topIndex];
        deckPile.deckObjects.RemoveAt(topIndex);

        // 2. Run the same coroutine to move it to the hand
        StartCoroutine(CardToHandCoroutine(topCard));

        //UpdateHandVisuals();
    }

    private IEnumerator CardToHandCoroutine(GameObject card)
    {
        // Target position for the card in the hand
        Vector3 handPosition = handTransform.position;

        // Duration for the movement and flip
        float duration = 0.2f;
        float flipTime = duration / 2; // Flip halfway through

        CardDisplay cardDisplay = card.GetComponent<CardDisplay>();

        // Animate movement to the hand
        Tween moveTween = card.transform.DOMove(handPosition, duration).SetEase(Ease.OutQuad);

        // Animate rotation to flip the card
        Sequence flipSequence = DOTween.Sequence();

        // First half: Rotate to 90 degrees (invisible)
        flipSequence.Append(card.transform.DORotate(new Vector3(0, 90, 0), flipTime).SetEase(Ease.InQuad));

        // Midway: Flip card display
        flipSequence.AppendCallback(() =>
        {
            if (cardDisplay != null)
            {
                cardDisplay.FlipCard(); // Switch between front and back
            }
        });

        // Second half: Rotate back to 0 degrees (visible)
        flipSequence.Append(card.transform.DORotate(new Vector3(0, 0, 0), flipTime).SetEase(Ease.OutQuad));

        // Play both animations
        flipSequence.Play();
        yield return moveTween.WaitForCompletion();

        // After animation, set parent and update visuals
        card.transform.SetParent(handTransform);
        cardsInHand.Add(card);

        UpdateHandVisuals();
        yield break;
    }


    private void UpdateHandVisuals()
    {
        int cardCount = cardsInHand.Count;
        if (cardCount == 0) return;

        // Special case for single card:
        if (cardCount == 1)
        {
            // Animate position and rotation to (0,0,0)
            cardsInHand[0].transform.DOLocalMove(Vector3.zero, tweenDuration).SetEase(Ease.OutQuad);
            cardsInHand[0].transform.DOLocalRotate(Vector3.zero, tweenDuration).SetEase(Ease.OutQuad);
            
            cardsInHand[0].transform.SetSiblingIndex(0);
            return;
        }

        for (int i = 0; i < cardCount; i++)
        {
            // Calculate rotation
            float rotationAngle = fanSpread * (i - (cardCount - 1) / 2f);
            Vector3 newRotation = new Vector3(0f, 0f, rotationAngle);

            // Calculate new local position
            float horizontalOffset = cardSpacing * (i - (cardCount - 1) / 2f);

            // Normalized position between -1 and 1
            float normalizedPos = 2f * i / (cardCount - 1) - 1f;
            float verticalOffset = verticalSpacing * (1 - normalizedPos * normalizedPos);

            Vector3 newLocalPos = new Vector3(horizontalOffset, verticalOffset, 0f);
            // Animate the card's position and rotation
            Transform cardTransform = cardsInHand[i].transform;
            cardTransform.SetSiblingIndex(i);
            cardTransform.DOLocalMove(newLocalPos, tweenDuration).SetEase(Ease.OutQuad);
            cardTransform.DOLocalRotate(newRotation, tweenDuration).SetEase(Ease.OutQuad);
            cardTransform.DOScale(new Vector3(100,100,0), tweenDuration).SetEase(Ease.OutQuad);

            CardMovement cm = cardTransform.GetComponent<CardMovement>();
            cm.UpdateOriginalToCurrent();

        }

    }
    public void UpdateHand()
    {
        Debug.Log("Calling UpdateHand");
        UpdateHandVisuals();
    }


    public void SortHandByRank()
    {
        cardsInHand.Sort((card1, card2) =>
        {
            CardDisplay display1 = card1.GetComponent<CardDisplay>();
            CardDisplay display2 = card2.GetComponent<CardDisplay>();

            return display1.cardData.rank.CompareTo(display2.cardData.rank);
        });

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            CardDisplay handCard = cardsInHand[i].GetComponent<CardDisplay>();
        }
    }

    public void SortHandBySuit()
    {
        cardsInHand.Sort((card1, card2) =>
        {
            CardDisplay display1 = card1.GetComponent<CardDisplay>();
            CardDisplay display2 = card2.GetComponent<CardDisplay>();

            int suitComparison = display1.cardData.suit.CompareTo(display2.cardData.suit);

            if (suitComparison != 0)
            {
                return suitComparison;
            }

            return display1.cardData.rank.CompareTo(display2.cardData.rank);
        });

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            CardDisplay handCard = cardsInHand[i].GetComponent<CardDisplay>();
        }
    }
}
