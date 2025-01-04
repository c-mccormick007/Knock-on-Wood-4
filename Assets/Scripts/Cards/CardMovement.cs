using UnityEngine;
using BandCproductions;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using System;

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
    private int siblingIndex;

    private Quaternion newRotation;
    private Vector3 newPosition;

    [SerializeField] private float selectScale = 1.1f;
    [SerializeField] private Vector2 cardPlay;
    [SerializeField] private Vector3 playPosition;

    private float lastClickTime = 0f; 
    private float doubleClickThreshold = 0.3f;

    public string tweenId; // Unique ID for DOTween

    private CardMovement lastReorderedCard = null; 
    private float lastReorderTime = -Mathf.Infinity;
    private float reorderCooldown = 0.2f;

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


        // Kill all tweens for this object before starting new ones
        DOTween.Kill(tweenId);

        // Smooth transition back to the original state
        Debug.Log("Moving to originalposition: " + originalPosition);
        
        if (currentState == 2)
        {
            rectTransform.DOLocalMove(newPosition, 0.1f)
            .SetEase(Ease.OutBack)
            .SetId(tweenId);

            rectTransform.DOLocalRotateQuaternion(newRotation, 0.1f)
                .SetEase(Ease.OutBack)
                .SetId(tweenId);

            rectTransform.DOScale(originalScale, 0.1f)
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
        else
        {
            rectTransform.DOLocalMove(originalPosition, 0.1f)
                .SetEase(Ease.OutBack)
                .SetId(tweenId);

            rectTransform.DOLocalRotateQuaternion(originalRotation, 0.1f)
                .SetEase(Ease.OutBack)
                .SetId(tweenId);

            rectTransform.DOScale(originalScale, 0.1f)
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
        currentState = 0;
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
            Debug.Log($"Setting original positon from: {originalPosition}");
            originalPosition = rectTransform.localPosition;
            Debug.Log($"Setting original positon to: {originalPosition}");
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
        //DOTween.Kill(tweenId);
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

        siblingIndex = transform.GetSiblingIndex();

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

            lastReorderedCard = null;
            lastReorderTime = -Mathf.Infinity;
        }
    }



    public void OnDrag(PointerEventData eventData)
    {
        // If we're in the deck, must be top card
        // Or if we're in the hand, it's allowed to drag
        // If you do *not* want hand-drag at all, you could also check IsInHand() here.

        if (currentState == 2)
        {
            HoverManager hoverManager = FindFirstObjectByType<HoverManager>();
            hoverManager.SetDragState(true);
            Vector2 localPointerPosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera,
                out localPointerPosition))
            {
                rectTransform.localPosition = originalPanelLocalPosition
                    + (Vector3)(localPointerPosition - originalLocalPointerPosition);

                BoxCollider2D centerCollider = GetComponent<BoxCollider2D>();
                if (centerCollider != null)
                {
                    Vector2 boxCenter = centerCollider.bounds.center;
                    Vector2 boxSize = centerCollider.bounds.size;

                    Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f);
                    foreach (Collider2D hit in hits)
                    {
                        if (hit.gameObject == this.gameObject)
                            continue;
                        CardMovement otherCard = hit.GetComponentInParent<CardMovement>();
                        if (otherCard != null && otherCard != this)
                        {
                            if (otherCard == lastReorderedCard)
                                continue;

                            if (Time.time - lastReorderTime < reorderCooldown)
                                continue;

                            ReorderCardInHand(otherCard);
                            lastReorderedCard = otherCard;
                            lastReorderTime = Time.time;
                            break;
                        }
                    }    
                }
            }
        }
    }

    private void ReorderCardInHand(CardMovement otherCard)
    {
        int myIndex = handManager.cardsInHand.IndexOf(this.gameObject);
        int otherIndex = handManager.cardsInHand.IndexOf(otherCard.gameObject);

        if (myIndex < 0 || otherIndex < 0) return;

        if (myIndex < otherIndex)
        {
            handManager.cardsInHand.RemoveAt(myIndex);
            otherIndex = handManager.cardsInHand.IndexOf(otherCard.gameObject);

            int newIndex = Mathf.Min(otherIndex + 1, handManager.cardsInHand.Count);
            handManager.cardsInHand.Insert(newIndex, this.gameObject);

            handManager.UpdateHand();
        }
        else if (myIndex > otherIndex)
        {
            handManager.cardsInHand.RemoveAt(myIndex);
            otherIndex = handManager.cardsInHand.IndexOf(otherCard.gameObject);

            int newIndex = Mathf.Max(otherIndex, 0);
            handManager.cardsInHand.Insert(newIndex, this.gameObject);
            handManager.UpdateHand();
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
            newPosition = rectTransform.localPosition;
            newRotation = rectTransform.localRotation;
            // Otherwise, just return to your idle state
            //transform.SetSiblingIndex(siblingIndex);
            TransitionToState0();
        }
        lastReorderedCard = null;

        HoverManager hoverManager = FindFirstObjectByType<HoverManager>();
        hoverManager.SetDragState(false);
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

    public bool IsDragging()
    {
        return (currentState == 2);
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
