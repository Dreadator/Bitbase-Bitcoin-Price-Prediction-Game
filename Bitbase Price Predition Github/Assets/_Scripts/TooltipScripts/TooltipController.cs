using UnityEngine;
using TMPro;

public class TooltipController : MonoBehaviour
{
    public static TooltipController Instance { get; private set; }

    // Tooltip Resources
    [SerializeField] private GameObject tooltipGameObject;
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private RectTransform tooltipRectTransform; 
    [SerializeField] private RectTransform tooltipBackground;

    private const float tooltipBackgroundPaddingSize = 4f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void Update()
    {
        // Make sure when tooltip is active, set as last sibling so it renders on top of everthing else.
        if (tooltipGameObject.activeSelf)
        {
            tooltipGameObject.transform.SetAsLastSibling();
        }
    }

    // Is called by MousePointerEnter when the Mouse moves over an object with a Tooltip Interaction script attatched to it. 
    public void ShowTooltip(string tooltipTextString, Vector2 anchoredPosition)
    {
        tooltipRectTransform.anchoredPosition = anchoredPosition;
        tooltipGameObject.SetActive(true);
        tooltipText.text = tooltipTextString;
        Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth, tooltipText.preferredHeight + tooltipBackgroundPaddingSize * 4f);
        tooltipBackground.sizeDelta = backgroundSize;
        tooltipGameObject.transform.SetAsLastSibling();
    }

    // Is called by MousePointerExit when the Mouse moves away from an object with a Tooltip Interaction script attatched to it. 
    public void HideTooltip()
    {
        tooltipGameObject.SetActive(false);
    }
}
