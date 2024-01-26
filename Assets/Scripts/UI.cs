using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
	public Button startStopButton;
	public Button quitButton;

	public TMP_Dropdown stimulieDropbown;	// Synchronous or asynchronous stroking
	public TMP_Dropdown threatDropdown;		// Attack or break a bone
	public Toggle hapticsToggle;			// With or without haptic feedback

	public TMP_InputField subjectID;

	private BiometricRecorder bioRecorder;

	// Start is called before the first frame update
	void Start()
	{
		bioRecorder = GameObject.FindGameObjectWithTag("Teslasuit").GetComponent<BiometricRecorder>();

		Button btn_start_stop = startStopButton.GetComponent<Button>();
		btn_start_stop.onClick.AddListener(OnStartStop);

		Button btn_quit = quitButton.GetComponent<Button>();
		btn_quit.onClick.AddListener(OnQuitApplication);
	}

	// Update is called once per frame
	void Update()
	{
		if (FileManager.Instance().savingData)
		{
			startStopButton.interactable = false;
			quitButton.interactable = false;
			stimulieDropbown.interactable = false;
			threatDropdown.interactable = false;
			hapticsToggle.interactable = false;
		}
		else
		{
			startStopButton.interactable = true;
			quitButton.interactable = true;
			stimulieDropbown.interactable = true;
			threatDropdown.interactable = true;
			hapticsToggle.interactable = true;
		}

	}

	void OnStartStop()
	{
		if (!bioRecorder.IsRecording())
		{
			startStopButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop";
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

			bioRecorder.StartStopRecording();
		}
		else
		{
			startStopButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
			quitButton.gameObject.SetActive(true);
			stimulieDropbown.gameObject.SetActive(true);
			threatDropdown.gameObject.SetActive(true);
			hapticsToggle.gameObject.SetActive(true);

			bioRecorder.StartStopRecording();
			bioRecorder.Save();
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
