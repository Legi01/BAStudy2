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

	public TMP_Dropdown stimulieDropbown;	// Synchronous or asynchronous stroking
	public TMP_Dropdown threatDropdown;		// Attack or break a bone
	public Toggle hapticsToggle;			// With or without haptic feedback

	public TMP_InputField replayFilename;
	public TMP_InputField subjectID;

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

		replayFilename.text = "MoCap";
	}

	// Update is called once per frame
	void Update()
	{
		if (FileManager.Instance().savingData)
		{
			recordButton.interactable = false;
			replayButton.interactable = false;
			quitButton.interactable = false;
			stimulieDropbown.interactable = false;
			threatDropdown.interactable = false;
			hapticsToggle.interactable = false;
			replayFilename.interactable = false;
		}
		else
		{
			recordButton.interactable = true;
			replayButton.interactable = true;
			quitButton.interactable = true;
			stimulieDropbown.interactable = true;
			threatDropdown.interactable = true;
			hapticsToggle.interactable = true;
			replayFilename.interactable = true;
		}

	}

	void OnStartStopRecording()
	{
		if (!mocapRecorder.IsRecording())
		{
			applicationStatus.text = "Recording...";
			recordButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop recording";
			replayButton.gameObject.SetActive(false);
			replayFilename.gameObject.SetActive(false);
			quitButton.gameObject.SetActive(false);

			GameObject[] paintbrushes = GameObject.FindGameObjectsWithTag("Paintbrush");
			bool sync = stimulieDropbown.value == 0 ? true : false;
			bool haptics = hapticsToggle.isOn;
			foreach (GameObject paintbrush in paintbrushes)
			{
				paintbrush.GetComponent<PaintbrushAnimator>().OnAnimatePaintbrush(sync, haptics);
			}
			stimulieDropbown.gameObject.SetActive(false);
			threatDropdown.gameObject.SetActive(false);
			hapticsToggle.gameObject.SetActive(false);

			mocapRecorder.StartStopRecording();
			biometricRecorder.StartStopRecording();
		}
		else
		{
			applicationStatus.text = "";
			recordButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start recording";
			replayButton.gameObject.SetActive(true);
			replayFilename.gameObject.SetActive(true);
			quitButton.gameObject.SetActive(true);
			stimulieDropbown.gameObject.SetActive(true);
			threatDropdown.gameObject.SetActive(true);
			hapticsToggle.gameObject.SetActive(true);

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
			stimulieDropbown.gameObject.SetActive(false);
			threatDropdown.gameObject.SetActive(false);
			hapticsToggle.gameObject.SetActive(false);

			motionReplay.Load(replayFilename.text);
			motionReplay.StartStopReplay();
		}
		else
		{
			applicationStatus.text = "";
			replayButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start replaying";
			recordButton.gameObject.SetActive(true);
			quitButton.gameObject.SetActive(true);
			stimulieDropbown.gameObject.SetActive(true);
			threatDropdown.gameObject.SetActive(true);
			hapticsToggle.gameObject.SetActive(true);

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

	public string GetSubjectID()
	{
		if (subjectID.text == "")
			return "TestSubject";
		else
			return subjectID.text;
	}
}
