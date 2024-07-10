using System.Collections;
using UnityEngine;
using TMPro;

public class UITimer : MonoBehaviour
{
    public TMP_Text timerText; // Referenz zum TextMeshPro-Text-Objekt
    private float timerDuration = 120f; // Timer-Dauer in Sekunden (2 Minuten)
    private float timeRemaining;
    private bool isRunning = false;

    void Start()
    {
        timeRemaining = timerDuration;
        UpdateTimerDisplay();
    }

    void Update()
    {
        if (isRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay();
            }
            else
            {
                timeRemaining = 0;
                isRunning = false;
                TimerEnded();
            }
        }
    }

    public void StartTimer()
    {
        if (!isRunning)
        {
            timeRemaining = timerDuration;
            isRunning = true;
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void TimerEnded()
    {
        // Aktionen ausführen, wenn der Timer endet
        Debug.Log("Timer ended!");
    }
}
