using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Content to display within the tooltip.
    [TextArea] public string description;

    // Offset to display tooltip, relative to the gameobjects local position.
    [SerializeField] private Vector2 offset;

    // Reference to this gameobjects Rect Transform.
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetDescriptionAndOffset(string content, Vector2 newOffset)
    {
        description = content;
        offset = newOffset;
    }

    // When Mouse Pointer Enters this gameObject Show Tooltip, using the description and offset.
    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipController.Instance.ShowTooltip(description, new Vector2(rectTransform.localPosition.x + offset.x, rectTransform.localPosition.y + offset.y));
    }

    // When Mouse Pointer Exits this gameObject Hide Tooltip.
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipController.Instance.HideTooltip();
    }    
}
