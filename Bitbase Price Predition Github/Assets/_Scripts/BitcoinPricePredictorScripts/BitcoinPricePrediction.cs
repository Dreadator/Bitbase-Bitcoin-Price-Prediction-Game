using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BitcoinPricePrediction : MonoBehaviour
{
    // Strings to display in price prediction text field.
    private const string predictBitcoinString = "Can you predict the price of Bitcoin?";
    private const string predictUpString = "You are predicting that Bitcoin will go <color=green>UP</color> in price";
    private const string predictDownString = "You are predicting that Bitcoin will go <color=red>DOWN</color> in price";
    private const string succesfulPredictionString = "You <color=green>Succesfully</color> predicted the movement of Bitcoins price!";
    private const string unsuccesfulPredictionString = "Ohh! You <color=red>Unsuccessfully</color> predicted the movement of Bitcoins price!";

    // State to handle prediction text based on what the user has selected.
    public enum PricePredictingState { WaitingToPredict, PredictingUp, PredictingDown };
    public PricePredictingState PredictingState = PricePredictingState.WaitingToPredict;

    // Prediction text, buttons and timer script references.
    [SerializeField] private TextMeshProUGUI predictBitcoinText;
    [SerializeField] private Button predictUpButton;
    [SerializeField] private Button predictDownButton;
    [SerializeField] private TimerScript timerScript;

    // Cached color blocks to control the colours of the prediction buttons.
    private ColorBlock noPredictionUpButtonColourBlock;
    private ColorBlock noPredictionDownButtonColourBlock;
    private ColorBlock predictUpButtonColourBlock;
    private ColorBlock predictDownButtonColourBlock;

    private void OnEnable()
    {
        // Subscribe to timer scripts Ten Seconds Reached, triggers when there is 10 seconds left on the timer.
        timerScript.TenSecondsReached += DisablePredictionButtonInteraction;
    }

    private void OnDisable()
    {
        // Unsubscribe to timer scripts Ten Seconds Reached Event.
        timerScript.TenSecondsReached -= DisablePredictionButtonInteraction;
    }

    private void Start()
    {
        // Set up the colour blocks for the buttons.
        noPredictionUpButtonColourBlock = predictUpButton.colors;
        noPredictionDownButtonColourBlock = predictDownButton.colors;

        predictUpButtonColourBlock = noPredictionUpButtonColourBlock;
        predictUpButtonColourBlock.normalColor = noPredictionUpButtonColourBlock.selectedColor;
        predictUpButtonColourBlock.disabledColor = noPredictionUpButtonColourBlock.selectedColor;

        predictDownButtonColourBlock = noPredictionDownButtonColourBlock;
        predictDownButtonColourBlock.normalColor = noPredictionDownButtonColourBlock.selectedColor;
        predictDownButtonColourBlock.disabledColor = noPredictionDownButtonColourBlock.selectedColor;

        // Set State to waiting to Predict.
        WaitingToPredict();
    }

    // State is Active when the player has not selected a prediction.
    public void WaitingToPredict()
    {
        predictBitcoinText.text = predictBitcoinString;
        PredictingState = PricePredictingState.WaitingToPredict;
        predictUpButton.colors = noPredictionUpButtonColourBlock;
        predictDownButton.colors = noPredictionDownButtonColourBlock;
    }

    // Switches State when the player is predicting the price will go up. Called from the Up prediction button.
    public void PredictingUp()
    {
        predictBitcoinText.text = predictUpString;
        PredictingState = PricePredictingState.PredictingUp;
        predictUpButton.colors = predictUpButtonColourBlock;
        predictDownButton.colors = noPredictionDownButtonColourBlock;
    }

    // Switches State when the player is predicting the price will go down. Called from the Down prediction button.
    public void PredictingDown()
    {
        predictBitcoinText.text = predictDownString;
        PredictingState = PricePredictingState.PredictingDown;
        predictDownButton.colors = predictDownButtonColourBlock;
        predictUpButton.colors = noPredictionUpButtonColourBlock;
    }

    // Called When the player is Successfull with thier prediction. Called from the BitcoinPriceMangerScript.
    public void SuccessfullPrediction() => StartCoroutine(LetPlayerKnowTheyWereSuccessful());
    
    private IEnumerator LetPlayerKnowTheyWereSuccessful()
    {
        predictBitcoinText.text = succesfulPredictionString;

        BitcoinPredictionScoreManager.Instance.IncreaseScoreAndStreak();

        yield return new WaitForSeconds(5f);

        WaitingToPredict();
    }

    // Called When the player is Unsuccessfull with thier prediction. Called from the BitcoinPriceMangerScript.
    public void UnsuccessfullPrediction() => StartCoroutine(LetPlayerKnowTheyWereUnsuccessful());
    
    private IEnumerator LetPlayerKnowTheyWereUnsuccessful()
    {
        predictBitcoinText.text = unsuccesfulPredictionString;

        BitcoinPredictionScoreManager.Instance.DecreaseScoreAndResetStreak();

        yield return new WaitForSeconds(5f);

        WaitingToPredict();
    }

    // Methods to enable and disable the price prediction button's Interaction.
    public void EnablePredictionButtonInteraction()
    {
        predictUpButton.interactable = true;
        predictDownButton.interactable = true;
    }

    public void DisablePredictionButtonInteraction()
    {
        predictUpButton.interactable = false;
        predictDownButton.interactable = false;
    }
}
