using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using TMPro;

public class BitcoinPriceRequest : MonoBehaviour
{
    // URLS for Bitcoin Price and Exchange Rate API's.
    private const string coindeskBitcoinPriceUrl = "https://api.coindesk.com/v1/bpi/currentprice/USD.json";
    private const string coindeskHistoricalDataUrl = "https://api.coindesk.com/v1/bpi/historical/close.json";
    private const string exchangeRateDataUrl = "https://www.floatrates.com/daily/usd.json";

    // Actions For When the Prices are retrieved and when it has failed retrieving the data.
    public event Action PricesRetrievedSuccessful = null;
    public event Action PricesRetrievedUnsuccessful = null;

    private const string usdCurrencyString = "$";

    public static float currentBitcoinPrice;

    // Lists for storing Bitcoins historical price data and last 30 dates strings. Acessed from the Graph
    public List<float> historicalUSDPriceFloatList;
    public List<float> historicalGBPPriceFloatList;
    public List<string> last30DatesList;

    // Array to store historical prices strings retrieved from API. Stored So data can be parsed than stored in historical float list.
    [SerializeField] private string[] historicalPricesStrings = new string[30];

    // Current Bitcoin Price display TMP Object Refence.
    [SerializeField] private TextMeshProUGUI bitcoinPriceText;

    // JSON Node to temporarily store data retrieved from API.
    private JSONNode currentJSONInfo;

    // Float to Store USD to GBP exchange rate data from API.
    private float usdToGBPExchangeRate;

    private void Awake()
    {
        historicalUSDPriceFloatList = new List<float>();
        historicalGBPPriceFloatList = new List<float>();
        last30DatesList = new List<string>();
    }

    private IEnumerator Start()
    {
        yield return RequestExchangeRateData();
        yield return RequestHistoricalPriceData();
        InvokeRepeating("GetBitcoinPriceData", 0.1f, 60f);
    }

    // Called Once Every 60 Seconds to Grab the Current Bitcoin Price, Api updates once a minute so thats what we'll do too.
    private void GetBitcoinPriceData() => StartCoroutine(RequestCurrentBitcoinPriceData());
    
    private IEnumerator RequestJSONData(string url)
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);

        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
        {
            // Error With Current Web Request
            if (PricesRetrievedUnsuccessful != null)
                PricesRetrievedUnsuccessful.Invoke();

            yield break;
        }

        currentJSONInfo = JSON.Parse(unityWebRequest.downloadHandler.text);
    }

    private IEnumerator RequestCurrentBitcoinPriceData()
    {
        yield return RequestJSONData(coindeskBitcoinPriceUrl);

        if (currentJSONInfo != null)
        {
            string bitcoinPrice = currentJSONInfo["bpi"]["USD"]["rate"];
            currentBitcoinPrice = float.Parse(bitcoinPrice);
            bitcoinPriceText.text = usdCurrencyString + currentBitcoinPrice;
            currentJSONInfo = null;
        }
    }

    private IEnumerator RequestExchangeRateData()
    {
        yield return RequestJSONData(exchangeRateDataUrl);

        if (currentJSONInfo != null)
        {
            string exchangeRatePrice = currentJSONInfo["gbp"]["rate"];
            usdToGBPExchangeRate = float.Parse(exchangeRatePrice);
            currentJSONInfo = null;
        }
    }

    private IEnumerator RequestHistoricalPriceData()
    {
        yield return RequestJSONData(coindeskHistoricalDataUrl);

        if (currentJSONInfo != null)
        {
            int j = 0;

            for (int i = -30; i < 0; i++)
            {
                string dateString = DateTime.UtcNow.AddDays(i).ToString("yyyy-MM-dd");
                historicalPricesStrings[j] = currentJSONInfo["bpi"][dateString];
                last30DatesList.Add(dateString);
                historicalUSDPriceFloatList.Add(float.Parse(historicalPricesStrings[j]));
                historicalGBPPriceFloatList.Add(historicalUSDPriceFloatList[j] * usdToGBPExchangeRate);
                j++;
            }

            if (PricesRetrievedSuccessful != null)
                PricesRetrievedSuccessful.Invoke();

            currentJSONInfo = null;
        }
    }    
}
