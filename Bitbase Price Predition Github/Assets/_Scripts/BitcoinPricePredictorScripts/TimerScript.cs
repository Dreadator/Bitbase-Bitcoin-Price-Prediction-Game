using UnityEngine;
using System;
using TMPro;

public class TimerScript : MonoBehaviour
{
    // Actions called at ten seconds remaining and when the timer runs out of time.
    public event Action TenSecondsReached = null;
    public event Action RunOutOfTime = null;

    // Timer.
    private float timeRemaining = 65;

    // Bool to ensure TenSecondReached Action only fires once in Update loop.
    private bool tenSecondsReached;

    [SerializeField] private TextMeshProUGUI timeRemainingText;

    private void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        else
        {
            if (RunOutOfTime != null)
                RunOutOfTime.Invoke();

            timeRemaining = 60;
            tenSecondsReached = false;
        }

        if (Mathf.RoundToInt(timeRemaining) == 10 && !tenSecondsReached)
        {
            if (TenSecondsReached != null)
                TenSecondsReached.Invoke();

            tenSecondsReached = true;
        }

        UpdateTimerUI(timeRemaining);
    }

    // Format Time to Display like timer.eg 0:00.
    private void UpdateTimerUI(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeRemainingText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
