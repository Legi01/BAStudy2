using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
	public TsSuitBehaviour tsSuitBehaviour;

	public Button loadAvatarButton;
	public Button startStopButton;
	public Button quitButton;

	public TMP_Text suitStatusLabel;
	public TMP_Text debugLabel;

	public TMP_Dropdown avatarDropdown;		// Female or male
	public TMP_Dropdown stimuliDropdown;	// Synchronous or asynchronous stroking
	public TMP_Dropdown threatDropdown;		// Attack or break a bone
	public Toggle hapticsToggle;			// With or without haptic feedback

	public TMP_InputField subjectID;

	private BiometricRecorder bioRecorder;
	private AnimatorController animController;

	private SphereCollider paintbrushHapticsCollider;

	private bool startStimulation = false;

	public GameObject xBot_sync;
	public GameObject xBot_async;
	public GameObject yBot_sync;
	public GameObject yBot_async;

	public GameObject IndicatorForBot;

	// Start is called before the first frame update
	void Start()
	{
		bioRecorder = GameObject.FindGameObjectWithTag("Teslasuit").GetComponent<BiometricRecorder>();
		animController = GameObject.FindGameObjectWithTag("Attacker").GetComponent<AnimatorController>();
		paintbrushHapticsCollider = GameObject.FindGameObjectWithTag("HapticsPaintbrush").GetComponent<SphereCollider>();

		hapticsToggle.onValueChanged.AddListener(OnHapticsToggle);
		loadAvatarButton.onClick.AddListener(LoadAvatar);
		startStopButton.onClick.AddListener(OnStartStop);
		quitButton.onClick.AddListener(OnQuitApplication);
}

// Update is called once per frame
void Update()
	{
		if (tsSuitBehaviour != null)
		{
			if (tsSuitBehaviour.IsConnected)
			{
				suitStatusLabel.text = "Connected: " + tsSuitBehaviour.Suit.Ssid;
			}
			else
			{
				suitStatusLabel.text = "Disconnected";
			}
		}

		if (FileManager.Instance().savingData)
		{
			startStopButton.interactable = false;
			quitButton.interactable = false;
			avatarDropdown.interactable = false;
			stimuliDropdown.interactable = false;
			threatDropdown.interactable = false;
			hapticsToggle.interactable = false;
		}
		else
		{
			startStopButton.interactable = true;
			quitButton.interactable = true;
			avatarDropdown.interactable = true;
			stimuliDropdown.interactable = true;
			threatDropdown.interactable = true;
			hapticsToggle.interactable = true;
		}

	}

	void LoadAvatar()
	{
		bool female = avatarDropdown.value == 0 ? true : false;

		switch (stimuliDropdown.value)
		{
			case 0:
				// Sync
				if (female) xBot_sync.SetActive(true);
				else yBot_sync.SetActive(true);
				break;

			case 1:
				// Async
				if (female) xBot_async.SetActive(true);
				else yBot_async.SetActive(true);
				break;

			case 2:
				if (female) xBot_sync.SetActive(true);
				else yBot_sync.SetActive(true);
				break;

		}
		//bool sync = stimulieDropbown.value == 0 ? true : false;
		//bool haptics = hapticsToggle.isOn;

		IndicatorForBot.gameObject.SetActive(true);
	}

	void OnStartStop()
	{
		startStimulation = !startStimulation;
		if (startStimulation)
		{
			// Start recording only if subject ID was entered
			/*if (IsSubjectIDEmpty())
			{
				debugLabel.text = "Enter subject ID";
				return;
			}*/

			startStopButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop";
			quitButton.gameObject.SetActive(false);
			subjectID.gameObject.SetActive(false);
			avatarDropdown.gameObject.SetActive(false);
			stimuliDropdown.gameObject.SetActive(false);
			threatDropdown.gameObject.SetActive(false);
			hapticsToggle.gameObject.SetActive(false);

			animController.StartStopwatch();

			//bioRecorder.StartStopRecording();
		}
		else
		{
			startStopButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
			quitButton.gameObject.SetActive(true);
			subjectID.gameObject.SetActive(true);
			stimuliDropdown.gameObject.SetActive(true);
			threatDropdown.gameObject.SetActive(true);
			hapticsToggle.gameObject.SetActive(true);

			//bioRecorder.StartStopRecording();
			//bioRecorder.Save();
		}
	}

	void OnHapticsToggle(bool toggle)
	{
		paintbrushHapticsCollider.enabled = toggle;
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

	private bool IsSubjectIDEmpty()
	{
		if (subjectID.text == "")
			return true;
		return false;
	}

	public string GetSubjectID()
	{
		if (subjectID.text == "")
			return "TestSubject";
		return subjectID.text;
	}
}
