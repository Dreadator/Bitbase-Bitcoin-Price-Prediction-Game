using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GraphWindow : MonoBehaviour
{
    // Text that displays when the chart is loading or if API data cannot be retrieved.
    [SerializeField] private TextMeshProUGUI loadingChartsTMP;

    // Graph Resources.
    [SerializeField] private Sprite graphDotSprite;
    [SerializeField] private LineRenderer graphLineRenderer;
    [SerializeField] private Color colourOfBars;

    [SerializeField] private RectTransform graphContainer;
    [SerializeField] private RectTransform labelTemplateX;
    [SerializeField] private RectTransform labelTemplateY;
    [SerializeField] private RectTransform lineTemplateX;
    [SerializeField] private RectTransform lineTemplateY;

    [SerializeField] private BitcoinPriceRequest bitcoinPriceRequest;

    // List of all gameobjects that make up the graph.
    private List<GameObject> graphGameObjectList;

    // Used to see if we should draw the line Renderer or not. 
    private bool useLineGraph = true;

    // Cached data we can reuse between graphs.
    private List<float> lastUsedValueList;
    private int lastUsedMaxVisibleAmount;
    private int lastUsedSeperatorCount;
    private IGraphVisual lastUsedGraphVisual;
    private IGraphVisual lineGraphVisual;
    private IGraphVisual barGraphVisual;

    private string currencyString = "$";

    private void OnEnable()
    {
        // Subscribe to bitcoinPriceRequest Events. Triggered when the prices are retrieved and when they fail to be retrieved.
        bitcoinPriceRequest.PricesRetrievedSuccessful += ShowGraphIfDataRetrieved;
        bitcoinPriceRequest.PricesRetrievedUnsuccessful += ShowErrorMessageIfDataNotRetrieved;
    }

    private void OnDisable()
    {
        // Unsubscribe from bitcoinPriceRequest Events.
        bitcoinPriceRequest.PricesRetrievedSuccessful -= ShowGraphIfDataRetrieved;
        bitcoinPriceRequest.PricesRetrievedUnsuccessful -= ShowErrorMessageIfDataNotRetrieved;
    }

    private void Start()
    {
        // Set Up graphVisual objects to use by Show Graph Method.
        IGraphVisual lineChartVisual = new LineGraphVisual(graphContainer, graphDotSprite, Color.white);
        lineGraphVisual = lineChartVisual;

        IGraphVisual barChartVisual = new BarChartVisual(graphContainer, colourOfBars); 
        barGraphVisual = barChartVisual;

        graphGameObjectList = new List<GameObject>();      
    }

    // Called when the BitcoinPriceRequest has Successfully retrieved the price data.
    public void ShowGraphIfDataRetrieved()
    {
        loadingChartsTMP.gameObject.SetActive(false);
        ShowGraph(bitcoinPriceRequest.historicalUSDPriceFloatList, lineGraphVisual, 30, 6);
    }

    // Called when the BitcoinPriceRequest has failed to retrieve the price data.
    public void ShowErrorMessageIfDataNotRetrieved()
    {
        loadingChartsTMP.text = "Error Retrieving Price Data. Please Refresh or try again later...";
    }

    // Draws Graph Using the GraphVisual Object.
    private void ShowGraph(List<float> valueList, IGraphVisual graphVisual, int maxVisibleValueAmount = -1, int seperatorCount = 6)
    {
        // Destroy Previous gameObjects in graphGameObjectList as we dont want them being created on top of the previous graph gameObjects.
        foreach (GameObject _gameObject in graphGameObjectList)
        {
            Destroy(_gameObject);
        }
        //clear the graphGameobject List. 
        graphGameObjectList.Clear();

        // Cache value list and graph visual used.
        lastUsedValueList = valueList;
        lastUsedGraphVisual = graphVisual;

        // Check To make sure Display Variables are in Correct Range that we want.
        if (maxVisibleValueAmount < 0)
            maxVisibleValueAmount = valueList.Count;

        if(maxVisibleValueAmount < 2)
            maxVisibleValueAmount = 2;
       
        if (maxVisibleValueAmount > valueList.Count)
            maxVisibleValueAmount = valueList.Count;

        if(seperatorCount < 2)
            seperatorCount = 2;
       
        if (seperatorCount > 15)
            seperatorCount = 15;

        // Cache the Checked Display Variables.
        lastUsedSeperatorCount = seperatorCount;
        lastUsedMaxVisibleAmount = maxVisibleValueAmount;

        // Get Graph Width and Height From the Container Its Displaying In.
        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;

        // Setup Minimum and Maximum Values to display on the Y Axis.
        float yMaximum = valueList[0];
        float yMinimum = valueList[0];

        for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++)
        {
            float value = valueList[i];

            if (value > yMaximum)
            {
                yMaximum = value;
            }
            if (value < yMinimum)
            {
                yMinimum = value;
            }
        }

        // Give the graph some Padding based on the minimum and maximum Y Axis Values.
        float yDifference = yMaximum - yMinimum;

        if (yDifference <= 0)
        {
            yDifference = 5f;
        }

        yMaximum += (yDifference * 0.2f);
        yMinimum -= (yDifference * 0.2f);

        // Determine how far each step will be spaced out on the X Axis.
        float xSize = graphWidth / (maxVisibleValueAmount + 1);      

        // Enable or Disable Line Renderer.
        if (useLineGraph)
        {
            graphLineRenderer.enabled = true;
            graphLineRenderer.positionCount = maxVisibleValueAmount;
        }
        else
        {
            graphLineRenderer.enabled = false;
        }

        int xIndex = 0;

        for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++)
        {
            float xPosition = xSize + xIndex * xSize;
            float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;

            string currencyAndPrice = currencyString + valueList[i].ToString();

            // Create the Graph gameObject Depending on which graph Visual we are using.
            GameObject graphVisualGameObject = graphVisual.AddGraphVisual(new Vector2(xPosition, yPosition), xSize, currencyAndPrice);
            graphGameObjectList.Add(graphVisualGameObject);

            // If we are drawing a line graph, then Set the position of each segment of the line graph.
            if (useLineGraph)
            {
                graphLineRenderer.SetPosition(i - (valueList.Count - maxVisibleValueAmount), new Vector3(xPosition, yPosition, 0));
            }

            // Create price label for the X axis.
            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer.transform, false);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPosition, -5f);
            labelX.GetComponent<TextMeshProUGUI>().text = bitcoinPriceRequest.last30DatesList[i]; 
            graphGameObjectList.Add(labelX.gameObject);

            // Create Grid Line for the X axis.
            RectTransform lineX = Instantiate(lineTemplateX);
            lineX.SetParent(graphContainer.transform, false);
            lineX.gameObject.SetActive(true);
            lineX.anchoredPosition = new Vector2(xPosition, 0);
            graphGameObjectList.Add(lineX.gameObject);

            xIndex++;
        }

        for (int i = 0; i <= seperatorCount; i++)
        {
            // Create price label for the Y axis.
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer.transform, false);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / seperatorCount;
            labelY.anchoredPosition = new Vector2(-3.6f, normalizedValue * graphHeight);
            labelY.GetComponent<TextMeshProUGUI>().text = currencyString + Mathf.RoundToInt(yMinimum + (normalizedValue * (yMaximum - yMinimum))).ToString(); // I wanna set the btc prices range, no need to round as ill need a float.
            graphGameObjectList.Add(labelY.gameObject);

            // Create Grid Line for the Y axis.
            RectTransform lineY = Instantiate(lineTemplateY);
            lineY.SetParent(graphContainer.transform, false);
            lineY.gameObject.SetActive(true);
            lineY.anchoredPosition = new Vector2(0, normalizedValue * graphHeight);
            graphGameObjectList.Add(lineY.gameObject);
        }
    }

    // Switch between Graph Visuals With Same Data To draw graph.
    private void SetGraphVisual(IGraphVisual graphVisual)
    {
        ShowGraph(lastUsedValueList, graphVisual, lastUsedMaxVisibleAmount, lastUsedSeperatorCount);
    }

    //Chart Button Functions. 
    public void SetBarChartButton()
    {
        useLineGraph = false;
        SetGraphVisual(barGraphVisual);
    }

    public void SetLineChartButton()
    {
        useLineGraph = true;
        SetGraphVisual(lineGraphVisual);
    }

    public void IncreaseVisibleAmount()
    {
        ShowGraph(lastUsedValueList, lastUsedGraphVisual, lastUsedMaxVisibleAmount + 1, lastUsedSeperatorCount);
    }

    public void DecreaseVisibleAmount()
    {
        ShowGraph(lastUsedValueList, lastUsedGraphVisual, lastUsedMaxVisibleAmount - 1, lastUsedSeperatorCount);
    }

    public void IncreaseSeperatorCount()
    {
        ShowGraph(lastUsedValueList, lastUsedGraphVisual, lastUsedMaxVisibleAmount, lastUsedSeperatorCount + 1);
    }

    public void DecreaseSeperatorCount()
    {
        ShowGraph(lastUsedValueList, lastUsedGraphVisual, lastUsedMaxVisibleAmount, lastUsedSeperatorCount - 1);
    }

    public void SetGBPValueList()
    {
        currencyString = "£";
        ShowGraph(bitcoinPriceRequest.historicalGBPPriceFloatList, lastUsedGraphVisual, lastUsedMaxVisibleAmount, lastUsedSeperatorCount);
    }

    public void SetUSDValueList()
    {
        currencyString = "$";
        ShowGraph(bitcoinPriceRequest.historicalUSDPriceFloatList, lastUsedGraphVisual, lastUsedMaxVisibleAmount, lastUsedSeperatorCount);
    }
}
