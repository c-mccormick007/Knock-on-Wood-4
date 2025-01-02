using UnityEngine;
using BandCproductions;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

public class CardMovement : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerClickHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private HandManager handManager;
    private Vector2 originalLocalPointerPosition;
    private Vector3 originalPanelLocalPosition;
    private Vector3 originalScale;
    private int currentState = 0;
    private Quaternion originalRotation;
    private Vector3 originalPosition;
    private DiscardManager discardManager;
    private bool isInsideDiscardZone = false;
    private bool isHovering = false;

    [SerializeField] private float selectScale = 1.1f;
    [SerializeField] private Vector2 cardPlay;
    [SerializeField] private Vector3 playPosition;

    private float lastClickTime = 0f; 
    private float doubleClickThreshold = 0.3f;

    private string tweenId; // Unique ID for DOTween

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        handManager = FindFirstObjectByType<HandManager>();
        discardManager = FindFirstObjectByType<DiscardManager>();
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.localPosition;
        originalRotation = rectTransform.localRotation;

        // Assign a unique ID for this card
        tweenId = gameObject.GetInstanceID().ToString();
    }

    private void Update()
    {
        switch (currentState)
        {
            case 1:
                HandleHoverState();
                break;
            case 2:
                HandleDragState();
                break;
            case 3:
                HandlePlayState();
                break;
        }
    }

    private void TransitionToState0()
    {

        currentState = 0;

        // Kill all tweens for this object before starting new ones
        DOTween.Kill(tweenId);   

        // Smooth transition back to the original state
        rectTransform.DOLocalMove(originalPosition, 0.3f)
            .SetEase(Ease.OutBack)
            .SetId(tweenId);

        rectTransform.DOLocalRotateQuaternion(originalRotation, 0.3f)
            .SetEase(Ease.OutBack)
            .SetId(tweenId);

        rectTransform.DOScale(originalScale, 0.3f)
            .SetEase(Ease.OutBack)
            .SetId(tweenId)
            .OnComplete(() =>
            {
                // Ensure the scale is reset correctly
                rectTransform.localScale = new Vector3(100, 100, 1);

                // Call UpdateHand after animations are completed
                handManager.UpdateHand();
            });
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 1. Determine if this card is in the deck or in the hand
        bool isInDeck = (transform.parent == handManager.deckPilePos);
        bool isInHand = (transform.parent == handManager.handTransform);

        

        if (isInDeck)
        {
            // 2. Check if this is the TOP card of the deck
            int childCount = handManager.deckPilePos.childCount;
            if (childCount > 0)
            {
                Transform topCardTransform = handManager.deckPilePos.GetChild(childCount - 1);

                // If *this* is not the top card, ignore any click logic
                if (topCardTransform != transform)
                {
                    Debug.Log("You clicked a deck card that is NOT the top card. Ignored.");
                    return;
                }
            }

            // 3. If it is the top card in the deck, we can apply your existing double-click logic
            if (Time.time - lastClickTime <= doubleClickThreshold)
            {
                Debug.Log("Double-click detected on TOP deck card!");
                handManager.AddTopDeckCardToHand();
                // This presumably moves the top card from deck to hand
            }
            else
            {
                Debug.Log("Single-click detected on TOP deck card!");
            }
            lastClickTime = Time.time;
        }

        if (isInHand)
        {
            if (Time.time - lastClickTime <= doubleClickThreshold)
            {
                // 4. If this card is in the hand, clicking triggers “discard”
                Debug.Log("discard");
                Debug.Log("Double-click detected on hand card!");
            }
            else
            {
                Debug.Log("Single-click detected on hand card!");
            }
            lastClickTime = Time.time;
            // If you eventually have a discard mechanic, you can call it here
            // e.g., handManager.DiscardCard(this.gameObject);
        }
        else
        {
            // If for some reason it's neither in the deck nor the hand
            Debug.Log("Clicked a card that's neither in deck nor in hand?");
        }
    }

    public void ManualHoverEnter()
    {
        Debug.Log($"ManualHoverEnter on {name}, scale={rectTransform.localScale}");
        if (isHovering) return;
        isHovering = true;
        // The same logic you had for OnPointerEnter...
        // e.g., set currentState = 1 if not dragging
        DOTween.Kill(tweenId);
        bool isInDeck = (transform.parent == handManager.deckPilePos);
        if (currentState == 0 && !isInDeck)
        {
            originalPosition = rectTransform.localPosition;
            originalRotation = rectTransform.localRotation;
            //originalScale = rectTransform.localScale;

            currentState = 1; // hover state
        }
    }

    public void ManualHoverExit()
    {
        Debug.Log("Manual hover exit");
        if (!isHovering) return;
        isHovering = false;
        // The same logic you had for OnPointerExit
        // e.g., if we were hovering, transition to state 0
        DOTween.Kill(tweenId);
        if (currentState == 1)
        {
            TransitionToState0();
        }
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        // If we're in the deck, we must be the top card to drag
        if (IsInDeck() && !IsTopCardInDeck())
        {
            return; // Do nothing if not top deck card
        }

        if (transform.parent == discardManager.discardPileTransform)
        {
            // If we’re not the top card in the discard pile, do nothing
            if (!discardManager.IsTopDiscardCard(gameObject))
            {
                Debug.Log("Cannot pick up or edit a discard card that's not on top.");
                return;
            }
        }

        transform.SetAsLastSibling();

        if (currentState == 1)
        {
            currentState = 2;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera,
                out originalLocalPointerPosition
            );

            // Cache the original panel position
            originalPanelLocalPosition = rectTransform.localPosition;

            // Kill any active tweens to prevent conflicts
            DOTween.Kill(tweenId);
        }
    }



    public void OnDrag(PointerEventData eventData)
    {
        // If we're in the deck, must be top card
        // Or if we're in the hand, it's allowed to drag
        // If you do *not* want hand-drag at all, you could also check IsInHand() here.

        if (currentState == 2)
        {
            Vector2 localPointerPosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera,
                out localPointerPosition))
            {
                rectTransform.localPosition = originalPanelLocalPosition
                    + (Vector3)(localPointerPosition - originalLocalPointerPosition);

                // Example: If drag up above some Y threshold => currentState = 3
                /*
                if (rectTransform.localPosition.y > cardPlay.y)
                {
                    currentState = 3;
                    DOTween.Kill(tweenId);
                    rectTransform.DOLocalMove(playPosition, 0.3f)
                               .SetEase(Ease.OutQuad)
                               .SetId(tweenId);
                }*/
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isInsideDiscardZone)
        {
            // 1. Get the discard manager
            DiscardManager discardManager = FindFirstObjectByType<DiscardManager>();
            if (discardManager != null)
            {
                // 2. Discard the card
                discardManager.DiscardCard(gameObject, handManager);
            }
            else
            {
                Debug.LogWarning("No DiscardManager found in scene!");
            }
        }
        else
        {
            // Otherwise, just return to your idle state
            TransitionToState0();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayZone"))
        {
            isInsideDiscardZone = true;
            discardManager.SetCardHighlight(gameObject, true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayZone"))
        {
            isInsideDiscardZone = false;
            discardManager.SetCardHighlight(gameObject, false);
        }
    }


    private void HandleHoverState()
    {
        DOTween.Kill(tweenId); // Kill previous hover animations
        rectTransform.DOScale(originalScale * selectScale, 0.2f).SetEase(Ease.OutQuad).SetId(tweenId);
    }

    private void HandleDragState()
    {
        DOTween.Kill(tweenId); // Kill previous drag animations
        rectTransform.DOLocalRotateQuaternion(Quaternion.identity, 0.3f).SetEase(Ease.OutQuad).SetId(tweenId);
    }

    private void HandlePlayState()
    {
        DOTween.Kill(tweenId); // Kill previous play animations
        rectTransform.DOLocalMove(playPosition, 0.3f).SetEase(Ease.OutQuad).SetId(tweenId);
        rectTransform.DOLocalRotateQuaternion(Quaternion.identity, 0.3f).SetEase(Ease.OutQuad).SetId(tweenId);

        if (Input.mousePosition.y < cardPlay.y)
        {
            currentState = 2;
        }
    }

    private bool IsInDeck()
    {
        return (transform.parent == handManager.deckPilePos);
    }

    private bool IsInHand()
    {
        return (transform.parent == handManager.handTransform);
    }

    private bool IsTopCardInDeck()
    {
        if (!IsInDeck()) return false;
        if (handManager.deckPilePos.childCount == 0) return false;

        // The top card is the last child in the deck pile
        Transform topCardTransform = handManager.deckPilePos.GetChild(handManager.deckPilePos.childCount - 1);
        return (topCardTransform == this.transform);
    }

    public void UpdateOriginalToCurrent()
    {
        originalPosition = rectTransform.localPosition;
        originalRotation = rectTransform.localRotation;
        originalScale = rectTransform.localScale;
    }

}
