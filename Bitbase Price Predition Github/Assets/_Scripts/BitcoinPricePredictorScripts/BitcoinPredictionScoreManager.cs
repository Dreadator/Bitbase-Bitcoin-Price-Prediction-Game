using UnityEngine;
using TMPro;

public class BitcoinPredictionScoreManager : MonoBehaviour
{
    public static BitcoinPredictionScoreManager Instance { get; private set; }

    // Strings to display in score text fields.
    private const string currentScoreTextString = "Score: <color=#f5a101>";
    private const string highScoreTextString = "Best score: <color=#f5a101>";
    private const string streakTextString = "Streak: <color=#f5a101>";
    private const string bestStreakTextString = "Best Streak: <color=#f5a101>";
    private const string timesCorrectTextString = "Times Correct: ";

    // Strings for player prefs.
    private const string highScorePrefsString = "HighScore";
    private const string currentScorePrefsString = "CurrentScore";
    private const string streakPrefsString = "Streak";
    private const string bestStreakPrefsString = "BestStreak";
    private const string correctUpsPrefsString = "CorrectUps";
    private const string correctDownsPrefsString = "CorrectDowns";

    // Scores display TMP references.
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI streakText;
    [SerializeField] private TextMeshProUGUI bestStreakText;
    [SerializeField] private TextMeshProUGUI correctUpsText;
    [SerializeField] private TextMeshProUGUI correctDownsText;

    // Current score values.
    private int currentScore;
    private int highScore;
    private int currentStreak;
    private int bestStreak;
    private int correctUps;
    private int correctDowns;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        // check if player pref key exists, then assign player pref data to current score values
        if (PlayerPrefs.HasKey(currentScorePrefsString))
            currentScore = PlayerPrefs.GetInt(currentScorePrefsString);
       
        if (PlayerPrefs.HasKey(highScorePrefsString))
            highScore = PlayerPrefs.GetInt(highScorePrefsString);
      
        if (PlayerPrefs.HasKey(streakPrefsString))
            currentStreak = PlayerPrefs.GetInt(streakPrefsString);

        if (PlayerPrefs.HasKey(bestStreakPrefsString))
            bestStreak = PlayerPrefs.GetInt(bestStreakPrefsString);
       
        if (PlayerPrefs.HasKey(correctUpsPrefsString))
            correctUps = PlayerPrefs.GetInt(correctUpsPrefsString);
       
        if (PlayerPrefs.HasKey(correctDownsPrefsString))
            correctDowns = PlayerPrefs.GetInt(correctDownsPrefsString);
   
        // Update UI to reflect player pref data
        UpdateUI(currentScoreText, currentScoreTextString, currentScore);
        UpdateUI(highScoreText, highScoreTextString, highScore);
        UpdateUI(streakText, streakTextString, currentStreak);
        UpdateUI(bestStreakText, bestStreakTextString, bestStreak);
        UpdateUI(correctUpsText, timesCorrectTextString, correctUps);
        UpdateUI(correctDownsText, timesCorrectTextString, correctDowns);    
    }

    public void IncreaseScoreAndStreak()
    {
        IncreaseScore(1);
        IncreaseStreak();
    }

    public void DecreaseScoreAndResetStreak()
    {
        if (currentScore > 0)
            DecreaseScore(1);

        if (currentStreak > 0)
            ResetStreak();
    }

    public void IncreaseCorrectUps()
    {
        correctUps++;
        PlayerPrefs.SetInt(correctUpsPrefsString, correctUps);
        UpdateUI(correctUpsText, timesCorrectTextString, correctUps);

    }

    public void IncreaseCorrectDowns()
    {
        correctDowns++;
        PlayerPrefs.SetInt(correctDownsPrefsString, correctDowns);
        UpdateUI(correctDownsText, timesCorrectTextString, correctDowns);
    }

    private void SaveScores()
    {
        PlayerPrefs.SetInt(currentScorePrefsString, currentScore);
    }

    private void IncreaseScore(int scoreAmount)
    {
        currentScore += scoreAmount;
        SaveScores();
        CheckHighscore();
        UpdateUI(currentScoreText, currentScoreTextString, currentScore);
    }

    private void DecreaseScore(int scoreAmount)
    {
        currentScore -= scoreAmount;

        if (currentScore <= 0)
            currentScore = 0;

        SaveScores();
        UpdateUI(currentScoreText, currentScoreTextString, currentScore);
    }

    private void IncreaseStreak()
    {
        currentStreak++;
        PlayerPrefs.SetInt(streakPrefsString, currentStreak);
        UpdateUI(streakText, streakTextString, currentStreak);
        CheckBestStreak();

    }

    private void ResetStreak()
    {
        currentStreak = 0;
        PlayerPrefs.SetInt(streakPrefsString, currentStreak);
        UpdateUI(streakText, streakTextString, currentStreak);
    }

    private void CheckHighscore()
    {
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt(highScorePrefsString, highScore);
            UpdateUI(highScoreText, highScoreTextString, highScore);
        }
    }

    private void CheckBestStreak()
    {
        if (currentStreak > bestStreak)
        {
            bestStreak = currentStreak;
            PlayerPrefs.SetInt(bestStreakPrefsString, bestStreak);
            UpdateUI(bestStreakText, bestStreakTextString, bestStreak);
        }
    }

    private void UpdateUI(TextMeshProUGUI textObject, string updatedTextString, int updatedScore)
    {
        textObject.text = updatedTextString + updatedScore;
    }

}
