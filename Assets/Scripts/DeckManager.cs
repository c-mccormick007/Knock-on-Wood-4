using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using BandCproductions;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{
    public List<Card> allCards = new List<Card>();
    public DeckPile deckPile;
    public HandManager HandManager;


    private void Start()
    {
        Card[] cards = Resources.LoadAll<Card>("Cards");

        allCards.AddRange(cards);

        ShuffleDeck();

        StartCoroutine(BuildDeckCoroutine());
    }

    private IEnumerator BuildDeckCoroutine()
    {
        // Add all cards to the deck with a delay
        foreach (Card card in allCards)
        {
            deckPile.AddCardToDeckPile(card);
            yield return new WaitForSeconds(0.05f); // Adjust delay as needed
        }

        // Once the deck is built, deal cards to the hand
        HandManager hand = FindFirstObjectByType<HandManager>();
        for (int i = 0; i < 10; i++)
        {
            DrawCard(hand);

            yield return new WaitForSeconds(0.1f); // Add delay between dealing cards
        }
        yield return new WaitForSeconds(1f);

        HandManager.SortHandBySuit();
        HandManager.UpdateHand();
    }

    public void DrawCard(HandManager handManager)
    {
        int lastIndex = deckPile.deck.Count - 1;
        if (lastIndex < 0) return;

        Card nextCard = deckPile.deck[lastIndex];
        handManager.AddTopDeckCardToHand();

        // Remove from the back
        deckPile.deck.RemoveAt(lastIndex);

        // Destroy the top child (last child) in deckPosition
        //deckPile.DestroyTopCardObj();
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < allCards.Count; i++)
        {
            int randomIndex = Random.Range(0, allCards.Count);
            Card temp = allCards[i];
            allCards[i] = allCards[randomIndex];
            allCards[randomIndex] = temp;
        }
    }

 


}
