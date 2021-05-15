using System.Collections;
using UnityEngine;
using TMPro;

public class BitcoinPriceManager : MonoBehaviour
{
    // Strings for bitcoin went TMP text field.
    private const string bitcoinWentString = "Bitcoin Went ";
    private const string upString = "UP +$";
    private const string downString = "DOWN -$";
   
    private string priceDifferenceString = string.Empty;

    // Bitcoin Went TMP Reference.
    [SerializeField] private TextMeshProUGUI bitcoinWentText;

    // Bitcoin Price Prediction and Timer script references.
    [SerializeField] private BitcoinPricePrediction bitcoinPricePrediction;
    [SerializeField] private TimerScript timerScript;

    // Floats to store bitcoin prices and calculate the difference in price.
    private float currentBitcoinPrice;
    private float lastBitcoinPrice;
    private float priceDiffreneceFloat;

    private void OnEnable()
    {
        // Subscribe to Timer scripts RunOutOfTime Event, which is called when the timer hits 0.
        timerScript.RunOutOfTime += CheckBitcoinPrices;
    }

    private void OnDisable()
    {
        // Unsubscribe to Timer scripts RunOutOfTime Event.
        timerScript.RunOutOfTime -= CheckBitcoinPrices;
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(5f);

        // Grab Bitcoin Price from BitcoinPriceRequest.
        currentBitcoinPrice = BitcoinPriceRequest.currentBitcoinPrice;
        lastBitcoinPrice = currentBitcoinPrice;
    }

    public void CheckBitcoinPrices()
    {
        // Cache last Bitcoin price and grab the latest Bitcoin price.
        lastBitcoinPrice = currentBitcoinPrice;
        currentBitcoinPrice = BitcoinPriceRequest.currentBitcoinPrice;

        // Calculate the price Difference.
        priceDiffreneceFloat = currentBitcoinPrice - lastBitcoinPrice;
        priceDifferenceString = priceDiffreneceFloat.ToString();

        // Check if Bitcoin Price is lower, returns true if higher, false if lower or same.
        bool priceCheck = CheckIfBitcoinPriceIsHigherOrLower();

        // Check direction of price and ensure player has input a price direction.
        if (priceCheck && bitcoinPricePrediction.PredictingState != BitcoinPricePrediction.PricePredictingState.WaitingToPredict)
        {
            //Bitcoin price is higher and player is predicting the price will go up.
            if (bitcoinPricePrediction.PredictingState == BitcoinPricePrediction.PricePredictingState.PredictingUp)
            {
                // Player was correct.
                bitcoinPricePrediction.SuccessfullPrediction();
                BitcoinPredictionScoreManager.Instance.IncreaseCorrectUps();
            }
            else
            {
                // Player was inccorect.
                bitcoinPricePrediction.UnsuccessfullPrediction();
            }
        }
        else if (!priceCheck && bitcoinPricePrediction.PredictingState != BitcoinPricePrediction.PricePredictingState.WaitingToPredict)
        {
            // Bitcoin price is lower and the player is predicting the price will go down.
            if (bitcoinPricePrediction.PredictingState == BitcoinPricePrediction.PricePredictingState.PredictingDown)
            {
                // Player was correct.
                bitcoinPricePrediction.SuccessfullPrediction();
                BitcoinPredictionScoreManager.Instance.IncreaseCorrectDowns();
            }
            else
            {
                // Player was inccorect.
                bitcoinPricePrediction.UnsuccessfullPrediction();
            }
        }

        UpdateBitcoinWentText();
    }

    private bool CheckIfBitcoinPriceIsHigherOrLower()
    {
        if (currentBitcoinPrice > lastBitcoinPrice)
        {
            // Bitcoin price is higher.
            return true;
        }
        else if (currentBitcoinPrice < lastBitcoinPrice)
        {
            // Bitcoin price is lower.
            return false;
        }
        else
        {
            // Bitcoin price is same, Im gna consider that as down.
            return false;
        }
    }

    // Updates the bitcoin went text with the direction and price diffrence.
    private void UpdateBitcoinWentText()
    {
        if (priceDiffreneceFloat > 0)
        {
            bitcoinWentText.text = bitcoinWentString + $"<color=green>{upString}" + priceDifferenceString;
        }
        else if (priceDiffreneceFloat <= 0)
        {
            // Remove the "-" from the string when the price difference is negative.
            priceDifferenceString = priceDifferenceString.Substring(1);

            bitcoinWentText.text = bitcoinWentString + $"<color=red>{downString}" + priceDifferenceString;
        }

        Invoke("ClearBitcoinWentTextField", 5f);
    }

    private void ClearBitcoinWentTextField()
    {
        bitcoinWentText.text = string.Empty;
        bitcoinPricePrediction.EnablePredictionButtonInteraction();
    }
}
