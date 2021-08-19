using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum GameStatus { Recording, Replaying, None };

public class Buttons : MonoBehaviour
{
    public Button recordButtonStart;
    public Button recordButtonStop;

    public Button replayButtonStart;
    public Button replayButtonStop;

    public Button quitButton;

    public Text applicationStatus;

    private GameStatus status;

    // Start is called before the first frame update
    void Start()
    {
        status = GameStatus.None;

        Button btn_record_start = recordButtonStart.GetComponent<Button>();
        btn_record_start.onClick.AddListener(OnStartRecording);
        Button btn_record_stop = recordButtonStop.GetComponent<Button>();
        btn_record_stop.onClick.AddListener(OnStopRecording);

        Button btn_replay_start = replayButtonStart.GetComponent<Button>();
        btn_replay_start.onClick.AddListener(OnStartReplaying);

        Button btn_replay_stop = replayButtonStop.GetComponent<Button>();
        btn_replay_stop.onClick.AddListener(OnStopReplaying);

        Button btn_quit = quitButton.GetComponent<Button>();
        btn_quit.onClick.AddListener(OnQuitApplication);
    }

    // Update is called once per frame
    void Update()
    {

        switch (status) {
            case GameStatus.None:
                break;
            case GameStatus.Recording:
                
                break;
            case GameStatus.Replaying:
                
                break;
            default:
                break;
        }
    }

    void OnStartRecording()
    {
        status = GameStatus.Recording;
        applicationStatus.text = "Recording";
        recordButtonStart.gameObject.SetActive(false);
        recordButtonStop.gameObject.SetActive(true);
        replayButtonStart.gameObject.SetActive(false);
        replayButtonStop.gameObject.SetActive(false);
    }

    void OnStopRecording()
    {
        status = GameStatus.None;
        applicationStatus.text = "";
        recordButtonStart.gameObject.SetActive(true);
        recordButtonStop.gameObject.SetActive(false);
        replayButtonStart.gameObject.SetActive(true);
        replayButtonStop.gameObject.SetActive(false);
    }

    void OnStartReplaying()
    {
        status = GameStatus.Replaying;
        applicationStatus.text = "Replaying";
        recordButtonStart.gameObject.SetActive(false);
        recordButtonStop.gameObject.SetActive(false);
        replayButtonStart.gameObject.SetActive(false);
        replayButtonStop.gameObject.SetActive(true);
    }

    void OnStopReplaying()
    {
        status = GameStatus.None;
        applicationStatus.text = "";
        recordButtonStart.gameObject.SetActive(true);
        recordButtonStop.gameObject.SetActive(false);
        replayButtonStart.gameObject.SetActive(true);
        replayButtonStop.gameObject.SetActive(false);
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
