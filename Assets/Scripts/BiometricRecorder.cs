using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TsSDK;
using Debug = UnityEngine.Debug;
using System.Linq;
using System.Diagnostics;

public class BiometricRecorder : MonoBehaviour
{
	public TsPpgProvider ppgProvider;

	private Dictionary<int, TsProcessedPpgViewItem> viewItems = new Dictionary<int, TsProcessedPpgViewItem>();

	private bool recording;
	private List<ECGData> recordedECGData;

	//private SuitAPIObject suitApi;

	// Start is called before the first frame update
	void Start()
	{
		recording = false;
		recordedECGData = new List<ECGData>();

		/*suitApi = GameObject.FindGameObjectWithTag("Teslasuit").GetComponentInChildren<SuitAPIObject>();
		if (suitApi.Biometry != null)
		{
			StartCoroutine(UpdateECGBiometriyOptions());
			StartCoroutine(UpdateGSRBiometriyOptions());
		}*/
	}

	// Update is called once per frame
	void Update()
	{
		if (ppgProvider.IsRunning)
		{
			var data = ppgProvider.GetData();
			if (data == null)
			{
				return;
			}

			foreach (var nodeData in data.NodesData)
			{
				OnECGUpdate(nodeData);
			}
		}
	}
	private void OnECGUpdate(ProcessedPpgNodeData nodeData)
	{
		ECGData ecgData = new ECGData(DateTime.Now, nodeData.timestamp, nodeData.heartRate, nodeData.isHeartrateValid);

		//Debug.Log("ecgData: hr = " + nodeData.heartRate + " valid " + nodeData.isHeartrateValid);
		if (recording) recordedECGData.Add(ecgData);
	}

	public void StartStopRecording()
	{
		recording = !recording;

		if (recording)
		{
			recordedECGData.Clear();
		}
	}

	public void Save()
	{
		FileManager.Instance().SaveECGData(recordedECGData);
	}

	public bool IsRecording()
	{
		return recording;
	}

	void OnDisable()
	{
		/*if (suitApi != null && suitApi.Biometry != null)
		{
			Debug.Log("Stopping ECG and GSR");

			suitApi.Biometry.StopECG();
			suitApi.Biometry.StopGSR();

			suitApi.Biometry.ECGUpdated -= OnECGUpdate;
			suitApi.Biometry.GSRUpdated -= OnGSRUpdate;

			Save();
		}*/
	}
}
