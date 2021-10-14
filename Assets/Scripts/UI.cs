using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    public Button recordButton;
    public Button replayButton;
    public Button quitButton;

    public TMP_Dropdown stimulieToggle; // Synchronous or asynchronous stroking
    public TMP_Dropdown threatToggle;  // Attack or break a bone

    public TMP_InputField inputField;

    public TextMeshProUGUI applicationStatus;

    private MocapRecorder mocapRecorder;
    private MocapReplay motionReplay;
    private BiometricRecorder biometricRecorder;

    // Start is called before the first frame update
    void Start()
    {
        mocapRecorder = GameObject.FindGameObjectWithTag("Teslasuit").GetComponent<MocapRecorder>();
        motionReplay = GameObject.FindGameObjectWithTag("Teslasuit").GetComponent<MocapReplay>();
        biometricRecorder = GameObject.FindGameObjectWithTag("Teslasuit").GetComponent<BiometricRecorder>();

        Button btn_record_start = recordButton.GetComponent<Button>();
        btn_record_start.onClick.AddListener(OnStartStopRecording);

        Button btn_replay_start = replayButton.GetComponent<Button>();
        btn_replay_start.onClick.AddListener(OnStartStopReplaying);

        Button btn_quit = quitButton.GetComponent<Button>();
        btn_quit.onClick.AddListener(OnQuitApplication);

        inputField.text = "MoCap";
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnStartStopRecording()
    {
        if (!mocapRecorder.IsRecording())
        {
            applicationStatus.text = "Recording...";
            recordButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop recording";
            replayButton.gameObject.SetActive(false);
            inputField.gameObject.SetActive(false);
            quitButton.gameObject.SetActive(false);

            GameObject[] paintbrushes = GameObject.FindGameObjectsWithTag("Paintbrush");
            bool sync = stimulieToggle.value == 0 ? true : false;
            foreach (GameObject paintbrush in paintbrushes)
            {
                paintbrush.GetComponent<PaintbrushAnimator>().OnAnimatePaintbrush(sync);
            }
            stimulieToggle.gameObject.SetActive(false);
            threatToggle.gameObject.SetActive(false);

            mocapRecorder.StartStopRecording();
            biometricRecorder.StartStopRecording();
        }
        else {
            applicationStatus.text = "";
            recordButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start recording";
            replayButton.gameObject.SetActive(true);
            inputField.gameObject.SetActive(true);
            quitButton.gameObject.SetActive(true);
            stimulieToggle.gameObject.SetActive(true);
            threatToggle.gameObject.SetActive(true);

            mocapRecorder.StartStopRecording();
            biometricRecorder.StartStopRecording();
            mocapRecorder.Save();
            biometricRecorder.Save();
        }
    }

    void OnStartStopReplaying()
    {
        if (!motionReplay.IsReplaying())
        {
            applicationStatus.text = "Replaying...";
            replayButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop replaying";
            recordButton.gameObject.SetActive(false);
            quitButton.gameObject.SetActive(false);
            stimulieToggle.gameObject.SetActive(false);
            threatToggle.gameObject.SetActive(false);

            motionReplay.Load(inputField.text);
            motionReplay.StartStopReplay();
        }
        else
        {
            applicationStatus.text = "";
            replayButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start replaying";
            recordButton.gameObject.SetActive(true);
            quitButton.gameObject.SetActive(true);
            stimulieToggle.gameObject.SetActive(true);
            threatToggle.gameObject.SetActive(true);

            motionReplay.StartStopReplay();
        }
    }

    public void OnQuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit ();
#endif

        Debug.Log("Quit Application");
    }
}
