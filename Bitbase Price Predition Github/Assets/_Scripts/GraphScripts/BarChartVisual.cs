using UnityEngine;
using UnityEngine.UI;

public class BarChartVisual : IGraphVisual
{
    // RectTransform of the Graph Container the Graph visual will display in.
    private RectTransform graphContainer;

    // Colour of the Bars.
    private Color barColour;

    // Set up Graph cotainer and Bar Colours.
    public BarChartVisual(RectTransform setUpGraphContainer, Color setUpBarColour)
    {
        graphContainer = setUpGraphContainer;
        barColour = setUpBarColour;
    }

    // Creates Bar GameObject. Places them based on thier value and spaces them out based on the graphPositionWidth.
    public GameObject AddGraphVisual(Vector2 graphPosition, float graphPositionWidth, string price)
    {
        GameObject barGameObject = CreateBar(graphPosition, graphPositionWidth * 0.8f, price); // 0.8f as we want space between each bar. 
        return barGameObject;
    }

    // Create Bar GameObject And Assign ToolTip Interaction.
    private GameObject CreateBar(Vector2 graphPosition, float barWidth, string price)
    {
        GameObject barGameObject = new GameObject("Bar", typeof(Image));
        barGameObject.transform.SetParent(graphContainer, false);
        barGameObject.GetComponent<Image>().color = barColour;
        barGameObject.AddComponent<TooltipInteraction>().SetDescriptionAndOffset(price, new Vector2(0, graphPosition.y));
        RectTransform barRectTransform = barGameObject.GetComponent<RectTransform>();
        barRectTransform.anchoredPosition = new Vector2(graphPosition.x, 0f);
        barRectTransform.sizeDelta = new Vector2(barWidth, graphPosition.y);
        barRectTransform.anchorMin = Vector2.zero;
        barRectTransform.anchorMax = Vector2.zero;
        barRectTransform.pivot = new Vector2(0.5f, 0f);
        return barGameObject;
    }
}
