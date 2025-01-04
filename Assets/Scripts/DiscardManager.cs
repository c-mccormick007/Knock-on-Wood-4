using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class DiscardManager : MonoBehaviour
{
    // Tracks all cards in the discard pile (topmost is last in the list)
    public List<GameObject> discardPileObjects = new List<GameObject>();
    public Transform discardPileTransform;

    [Header("Highlight Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = new Color(1f, 1f, 1f, 1.3f);
    public GinGameState gameState;

    /// <summary>
    /// Sets the highlight on the card to indicate it can be discarded.
    /// Typically called when the card is hovering over the discard zone.
    /// </summary>
    public void SetCardHighlight(GameObject card, bool highlight)
    {

        //If using UI Image, do something similar:
        Image img = card.GetComponent<Image>();
        if (img != null) img.color = highlight ? highlightColor : normalColor;
    }

    /// <summary>
    /// Actually discard the card, removing it from the hand and
    /// placing it on top of the discard pile.
    /// </summary>
    public void DiscardCard(GameObject card, HandManager handManager)
    {
        // 1. Remove the card from the hand list, if it’s still there
        if (handManager.cardsInHand.Contains(card))
        {
            handManager.cardsInHand.Remove(card);
        }

        // 2. Parent it to the discard pile transform
        DOTween.Kill(card);
        RectTransform rect = card.GetComponent<RectTransform>();
        CardDisplay cardDisplay = card.GetComponent<CardDisplay>(); 
        card.transform.SetParent(discardPileTransform, false);

        // 3. Add it to the discard pile list
        discardPileObjects.Add(card);
        gameState.AddCardToDiscardState(cardDisplay.cardData.CardID);
        rect.anchoredPosition = Vector2.zero;
        rect.localRotation = Quaternion.identity;
        rect.localScale = new Vector3 (100, 100, 1);

        // 4. Move it visually on top
        //    (For UI) transforms, this ensures it’s the last sibling in the hierarchy
        card.transform.SetAsLastSibling();

        // 5. (Optional) Reset highlight color
        SetCardHighlight(card, false);

        // 6. Update the hand visuals so the removed card no longer is displayed
        handManager.UpdateHand();

    }

    /// <summary>
    /// Returns true if the given card is the top card in the discard pile.
    /// </summary>
    public bool IsTopDiscardCard(GameObject card)
    {
        // If no cards in discard, nothing is top
        if (discardPileObjects.Count == 0) return false;

        // The top card is the last item in discardPileObjects
        GameObject topCard = discardPileObjects[discardPileObjects.Count - 1];
        return (topCard == card);
    }
}
