using UnityEngine;
using System.Collections.Generic;
using BandCproductions;
using DG.Tweening;

public class DeckPile : MonoBehaviour
{
    public List<Card> deck = new List<Card>();
    public Transform deckPosition;
    public GameObject cardPrefab;
    public float deckOffset = -0.1f;
    public List<GameObject> deckObjects = new List<GameObject>();
    public GinGameState gameState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void AddCardToDeckPile(Card card)
    {
        deck.Add(card);

        // Create the card prefab as a child of deckPosition
        GameObject newCard = Instantiate(cardPrefab, deckPosition);

        // Offset the card's initial local position slightly
        RectTransform rectTransform = newCard.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(600, -500, -1800); 
        rectTransform.localRotation = Quaternion.Euler(180,180,0);
        rectTransform.localScale = new Vector3(200, 200, 1);

        CardDisplay cardDisplay = newCard.GetComponent<CardDisplay>();
        cardDisplay.cardData = card;

        //rectTransform.localPosition = new Vector3(0, 1 * deck.Count, 1 * deck.Count);
        rectTransform.DOLocalMove(new Vector3(0, 1 * deck.Count, 1 * deck.Count), 0.5f).SetEase(Ease.OutQuad);
        rectTransform.DOLocalRotate(new Vector3(0,0,0), 0.9f).SetEase(Ease.OutQuad);
        rectTransform.DOScale(new Vector3(100, 100, 1), 0.9f).SetEase(Ease.OutQuad);
        deckObjects.Add(newCard);
        //gameState.AddCardToDeckState(card.GetCardId());


    }

    public void DestroyTopCardObj()
    {
        if (deck.Count == 0) return;
        if (deckPosition.childCount == 0) return;


        Transform topCardTransform = deckPosition.GetChild(deckPosition.childCount - 1);
        Destroy(topCardTransform.gameObject);
    }

    public List<int> ReturnDeckIdArray()
    {
        List<int> listData = new List<int>();
        foreach (Card card in deck)
        {
            listData.Add(card.GetCardId());
        }
        return listData;
    }
}
