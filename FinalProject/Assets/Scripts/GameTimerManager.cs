using TMPro;
using UnityEngine;

public class GameTimerManager : MonoBehaviour
{
    public float totalTime = 180f;
    public TMP_Text timerText;

    private float timeRemaining;
    private bool timerRunning = false;
    private PaintingProgressManager progressManager;

    void Start()
    {
        timeRemaining = totalTime;
        timerRunning = true;
        progressManager = FindFirstObjectByType<PaintingProgressManager>();
        UpdateTimerDisplay();
    }

    void Update()
    {
        if (!timerRunning) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            timerRunning = false;
            UpdateTimerDisplay();

            if (progressManager != null && !progressManager.IsGameEnded())
                progressManager.LoseGame();

            return;
        }

        UpdateTimerDisplay();
    }

    void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = string.Format("{0}:{1:00}", minutes, seconds);

        if (timeRemaining <= 30f)
            timerText.color = new Color(1f, 0.3f, 0.3f);
        else if (timeRemaining <= 60f)
            timerText.color = new Color(1f, 0.8f, 0.2f);
        else
            timerText.color = Color.white;
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    public float GetTimeRemaining()
    {
        return timeRemaining;
    }
}
