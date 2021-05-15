using UnityEngine;
using UnityEngine.UI;

public class LineGraphVisual : IGraphVisual
{
    // RectTransform of the Graph Container the Graph visual will display in.
    private RectTransform graphContainer;

    // Sprite References. 
    private Sprite dotSprite;
    private Color dotColour;

    // Set up Graph cotainer and sprite references.
    public LineGraphVisual(RectTransform setUpGraphContainer, Sprite setUpDotSprite, Color setUpDotColour)
    {
        graphContainer = setUpGraphContainer;
        dotSprite = setUpDotSprite;
        dotColour = setUpDotColour;
    }

    // Creates Dot GameObject. Places them based on thier value and spaces them out based on the graphPositionWidth.
    public GameObject AddGraphVisual(Vector2 graphPosition, float graphPositionWidth, string price)
    {
        GameObject dotGameObject = CreateDot(graphPosition, price);
        return dotGameObject;
    }

    // Create Dot GameObject And Assign ToolTip Interaction.
    private GameObject CreateDot(Vector2 anchoredPosition, string price)
    {
        GameObject dotGameObject = new GameObject("Dot", typeof(Image));
        dotGameObject.transform.SetParent(graphContainer, false);
        dotGameObject.GetComponent<Image>().sprite = dotSprite;
        dotGameObject.GetComponent<Image>().color = dotColour;
        dotGameObject.AddComponent<TooltipInteraction>().SetDescriptionAndOffset(price, new Vector2(0,30));
        RectTransform dotRectTransform = dotGameObject.GetComponent<RectTransform>();
        dotRectTransform.anchoredPosition = anchoredPosition;
        dotRectTransform.sizeDelta = new Vector2(11, 11);
        dotRectTransform.anchorMin = Vector2.zero;
        dotRectTransform.anchorMax = Vector2.zero;
        return dotGameObject;
    }
}
