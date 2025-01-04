using UnityEngine;
using UnityEngine.UI;
using BandCproductions;
using System;

public class CardDisplay : MonoBehaviour
{
    public Card cardData;
    public string cardName;
    public Sprite cardSprite;
    public Sprite cardBack;

    private int flipState = 1;

    private void Start()
    {
        UpdateCardDisplay();
    }

    private void UpdateCardDisplay()
    {
        cardName = cardData.cardName;
        cardSprite = cardData.sprite;

        if (flipState == 0)
        {
            SetCardImage(cardSprite); 
        }
        else
        {
            SetCardImage(cardBack); 
        }
    }

    public void FlipCard()
    {
        switch (flipState)
        {
            case 0:
                SetCardImage(cardBack);
                flipState = 1;
                break;
            case 1:
                SetCardImage(cardSprite);
                flipState = 0;
                break;
        }
    }

    private void SetCardImage(Sprite sprite)
    {
        Transform canvas = transform.Find("CardCanvas");
        if (canvas == null)
        {
            throw new ArgumentNullException(nameof(canvas), "Canvas is null.");
        }

        Transform cardImageTransform = canvas.Find("CardImage");
        if (cardImageTransform == null)
        {
            throw new ArgumentNullException(nameof(cardImageTransform), "cardImageTransform is null.");
        }

        Image cardImage = cardImageTransform.GetComponent<Image>();
        if (cardImage == null)
        {
            throw new ArgumentNullException(nameof(cardImage), "cardImage is null.");
        }

        cardImage.sprite = sprite;
    }

    public Card GetCardData()
    {
        return cardData;
    }

}
