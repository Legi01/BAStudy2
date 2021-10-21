using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeslasuitAPI;
using Debug = UnityEngine.Debug;
using System.Linq;
using System.Diagnostics;

public class BiometricRecorder : MonoBehaviour
{
    private bool recording;
    private List<ECGData> recordedECGData;
    private List<GSRData> recordedGSRData;

    private SuitAPIObject suitApi;

    // Start is called before the first frame update
    void Start()
    {
        recording = false;
        recordedECGData = new List<ECGData>();
        recordedGSRData = new List<GSRData>();

        suitApi = GameObject.FindGameObjectWithTag("Teslasuit").GetComponentInChildren<SuitAPIObject>();
        if (suitApi.Biometry != null)
        {
            StartCoroutine(UpdateECGBiometriyOptions());
            StartCoroutine(UpdateGSRBiometriyOptions());
        }
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    private IEnumerator UpdateECGBiometriyOptions()
    {
        suitApi.Biometry.StartECG();
        yield return new WaitUntil(() => suitApi.Biometry is { ECGStarted: true });

        // Call the delegate that is used to track changes of ECG data.
        suitApi.Biometry.ECGUpdated += OnECGUpdate;

        ECGFrequency ecgFrequency = ECGFrequency.TS_ECG_FPS_20;
        suitApi.Biometry.UpdateECGFrequency(ecgFrequency);

        Debug.Log($"Updated biometry options: ECG frequency is {ecgFrequency}.");
    }

    private IEnumerator UpdateGSRBiometriyOptions()
    {
        suitApi.Biometry.StartGSR();
        yield return new WaitUntil(() => suitApi.Biometry is { GSRStarted: true });

        suitApi.Biometry.GSRUpdated += OnGSRUpdate;

        GSRFrequency gsrFrequency = GSRFrequency.TS_GSR_FPS_60;
        suitApi.Biometry.UpdateGSRFrequency(gsrFrequency);

        Debug.Log($"Updated biometry options: GSR frequency is {gsrFrequency}.");
    }

    private void OnECGUpdate(ref ECGBuffer_MV ECGBuffer, IntPtr opaque, ResultCode resultCode)
    {
        if (resultCode == ResultCode.TS_SUCCESS)
        {
            uint[] deltaTime = new uint[ECGBuffer.data.Length];
            float[] amplitude = new float[ECGBuffer.data.Length];

            for (int i = 0; i < ECGBuffer.data.Length; i++)
            {
                // Interval between measurement, measured in nanoseconds
                deltaTime[i] = ECGBuffer.data[i].deltaTime;

                // Amplitude, measured in millivolts
                //float mv = (ECGBuffer.data[i].mv - 2048.0f) / (4096.0f) * 80.0f;
                amplitude[i] = ECGBuffer.data[i].mv;
                //Debug.Log($"Index: {i}, deltaTime: {ECGBuffer.data[i].deltaTime}, amplitude [mV]: {ECGBuffer.data[i].mv}");
            }

            ECGData ecgData = new ECGData(DateTime.Now, deltaTime, amplitude);
            if (recording) recordedECGData.Add(ecgData);
        }
    }

    private void OnGSRUpdate(ref GSRBuffer GSRBuffer, IntPtr opaque, ResultCode resultCode)
    {
        if (resultCode == ResultCode.TS_SUCCESS)
        {

            for (int i = 0; i < GSRBuffer.data.Length; i++)
            {
                uint count = GSRBuffer.data[i].count;
                int[] data = GSRBuffer.data[i].data;

                GSRData gsrData = new GSRData(DateTime.Now, count, data);
                if (recording) recordedGSRData.Add(gsrData);

                for (int j = 0; j < data.Length; j++)
                {
                    Debug.Log($" Index (i,j): {i}, {j}, count: {GSRBuffer.data[i].count}, data: {GSRBuffer.data[i].data[j]}");
                }
            }
        }
    }

    public void StartStopRecording()
    {
        recording = !recording;

        if (recording)
        {
            recordedECGData.Clear();
            recordedGSRData.Clear();
        }
    }

    public void Save()
    {
        FileManager.Instance().SaveECGData(recordedECGData);
        FileManager.Instance().SaveGSRData(recordedGSRData);
    }

    public bool IsRecording()
    {
        return recording;
    }

    void OnDisable()
    {
        if (suitApi != null && suitApi.Biometry != null)
        {
            Debug.Log("Stopping ECG and GSR");

            suitApi.Biometry.StopECG();
            suitApi.Biometry.StopGSR();

            suitApi.Biometry.ECGUpdated -= OnECGUpdate;
            suitApi.Biometry.GSRUpdated -= OnGSRUpdate;

            Save();
        }
    }
}
