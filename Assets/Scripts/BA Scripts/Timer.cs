using System.Collections;
using UnityEngine;
using TMPro;

public class UITimer : MonoBehaviour
{
    public TMP_Text timerText; // Referenz zum TextMeshPro-Text-Objekt
    public GameObject redCube; // Referenz zum roten Würfel
    public GameObject greenCube; // Referenz zum grünen Würfel

    private float timerDuration = 15f; // Timer-Dauer in Sekunden (2 Minuten)
    private float timeRemaining;
    private bool isRunning = false;
    private bool timerEnded = false;

    public CubeCollisionRecorder cubeCollisionRecorder; // Referenz zur cubeCollisionRecorder-Komponente


    void Start()
    {
        timeRemaining = timerDuration;
        UpdateTimerDisplay();
        // Cubes initial unsichtbar machen
        SetCubesVisibility(false);
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
                timerEnded = true;
                TimerEnded();
            }
        }
        else if (timerEnded)
        {
            UpdateTimerDisplay();
        }
    }

    public void StartTimer()
    {
        if (!isRunning)
        {
            timeRemaining = timerDuration;
            isRunning = true;
            timerEnded = false;
            // Cubes sichtbar machen, wenn der Timer startet
            SetCubesVisibility(true);
        }
    }

    public void ResetAndStopTimer()
    {
        timeRemaining = timerDuration;
        isRunning = false;
        UpdateTimerDisplay();

        // Cubes unsichtbar machen, wenn der Timer zurückgesetzt wird
        SetCubesVisibility(false);
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
        // Cubes unsichtbar machen, wenn der Timer endet
        SetCubesVisibility(false);
        // Speichern der gesammelten Daten
        cubeCollisionRecorder.SaveLoggedData();
    }

    private void SetCubesVisibility(bool isVisible)
    {
        redCube.SetActive(isVisible);
        greenCube.SetActive(isVisible);
    }
}
