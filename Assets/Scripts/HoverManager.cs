using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using DG.Tweening; // if needed

public class HoverManager : MonoBehaviour
{
    private GameObject currentlyHoveredCard = null;

    void Update()
    {
        // If the pointer is over the Unity UI system...
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // We'll do a UI raycast to see what we're over
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            GameObject newHoveredCard = null;

            // Find the first CardMovement in the raycast results
            foreach (RaycastResult r in results)
            {
                CardMovement cardMovement = r.gameObject.GetComponentInParent<CardMovement>();
                if (cardMovement != null)
                {
                    newHoveredCard = cardMovement.gameObject;
                    break;
                }
            }

            // Compare to our old hovered card
            if (newHoveredCard != currentlyHoveredCard)
            {
                // If we had an old hovered card, tell it we exited
                if (currentlyHoveredCard != null)
                {
                    var oldMovement = currentlyHoveredCard.GetComponent<CardMovement>();
                    if (oldMovement != null) oldMovement.ManualHoverExit();
                }

                // If we have a new hovered card, tell it we entered
                if (newHoveredCard != null)
                {
                    var newMovement = newHoveredCard.GetComponent<CardMovement>();
                    if (newMovement != null) newMovement.ManualHoverEnter();
                }

                currentlyHoveredCard = newHoveredCard;
            }
        }
        else
        {
            // Pointer is not over any UI. If we had a hovered card, exit it.
            if (currentlyHoveredCard != null)
            {
                var oldMovement = currentlyHoveredCard.GetComponent<CardMovement>();
                if (oldMovement != null) oldMovement.ManualHoverExit();

                currentlyHoveredCard = null;
            }
        }
    }
}
