using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float hoverAlpha = 0.5f; // Alpha when hovered
    [SerializeField] private float normalAlpha = 1.0f; // Alpha when not hovered
    [SerializeField] private float hoverScale = 1.01f; // Scale when hovered
    [SerializeField] private float normalScale = 1.0f; // Scale when not hovered
    [SerializeField] private float animationDuration = 0.3f; // Duration of animation
    [SerializeField] private RectTransform menu;

    private Vector3 centralPosition = new Vector3(46,-297,0);

    private Image buttonImage; // Reference to the button's image
    private RectTransform rectTransform; // Reference to the button's RectTransform

    void Start()
    {
        buttonImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        if (buttonImage == null)
        {
            Debug.LogError("DoTweenButtonAnimation requires an Image component on the same GameObject.");
        }

        if (rectTransform == null)
        {
            Debug.LogError("DoTweenButtonAnimation requires a RectTransform component on the same GameObject.");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Animate alpha
        buttonImage.DOFade(hoverAlpha, animationDuration);

        // Animate scale
        rectTransform.DOScale(hoverScale, animationDuration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Animate alpha back to normal
        buttonImage.DOFade(normalAlpha, animationDuration);

        // Animate scale back to normal
        rectTransform.DOScale(normalScale, animationDuration);
    }

    public void MoveUpAndFadeOut()
    {
        buttonImage.DOFade(0, animationDuration);
        rectTransform.DOMoveY(400, animationDuration);
    }

    public void AnimateGameMenu()
    {
        menu.DOAnchorPos(centralPosition, .35f).SetEase(Ease.OutQuad);
    }
}
