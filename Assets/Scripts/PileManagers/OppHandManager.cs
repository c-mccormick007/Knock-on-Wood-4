using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BandCproductions;
using System;
using DG.Tweening;

public class OppHandManager : MonoBehaviour
{
    public DeckManager deckManager;
    public GameObject cardPrefab; // Assign Card Prefab Here
    public Transform deckPilePos;
    public Transform oppHandTransform; //Root of the Hand Position
    public float fanSpread = 4.81f; //Fan spread

    public DeckPile deckPile;

    public float cardSpacing = 60.2f; //Card Spacing
    public float verticalSpacing = -39f;
    public List<GameObject> cardsInHand = new List<GameObject>(); //List of card objects
    public float tweenDuration = 0.02f;

    void Start()
    {
    }

    private void Update()
    {
        UpdateHandVisuals();
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
        int cardsInHandCount = cardsInHand.Count;
        float dealOffset = 0f;
        if (cardsInHandCount > 0 && cardsInHandCount < 5)
        {
            dealOffset = cardsInHandCount * 75;
        }
        else if (cardsInHandCount >= 5)
        {
            dealOffset = 336;
        }

        Vector3 handPosition = oppHandTransform.position + new Vector3(dealOffset, 0, 0);

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

        // Second half: Rotate back to 0 degrees (visible)
        flipSequence.Append(card.transform.DORotate(new Vector3(0, 0, 0), flipTime).SetEase(Ease.OutQuad));

        // Play both animations
        flipSequence.Play();
        yield return moveTween.WaitForCompletion();

        // After animation, set parent and update visuals
        card.transform.SetParent(oppHandTransform);
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

            GameObject cardObj = cardsInHand[i];
            CardMovement cardMovement = cardObj.GetComponent<CardMovement>();

            // If this card is currently being dragged, skip layout
            if (cardMovement != null && cardMovement.IsDragging())
            {
                Debug.Log("Card object: " + cardObj);
                // DO NOT tween or reposition this card; it's manually controlled.
                continue;
            }


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
            cardTransform.DOLocalMove(newLocalPos, tweenDuration).SetEase(Ease.OutQuad)
                .SetId(cardObj.GetComponent<CardMovement>().tweenId)
                .OnComplete(() => cardMovement.UpdateOriginalToCurrent());
            cardTransform.DOLocalRotate(newRotation, tweenDuration).SetEase(Ease.OutQuad)
                .SetId(cardObj.GetComponent<CardMovement>().tweenId)
                .OnComplete(() => cardMovement.UpdateOriginalToCurrent());
            cardTransform.DOScale(new Vector3(70, 70, 1), tweenDuration).SetEase(Ease.OutQuad)
                .SetId(cardObj.GetComponent<CardMovement>().tweenId)
                .OnComplete(() => cardMovement.UpdateOriginalToCurrent());

        }

    }
    public void UpdateHand()
    {
        Debug.Log("Calling UpdateHand");
        UpdateHandVisuals();
    }
}
