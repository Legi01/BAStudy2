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

    private Stopwatch stopwatch;

    private FileManager fileManager;

    public SuitAPIObject suitApi;

    // Start is called before the first frame update
    void Start()
    {
        recording = false;
        recordedECGData = new List<ECGData>();
        recordedGSRData = new List<GSRData>();
        fileManager = new FileManager();

        stopwatch = new Stopwatch();
        stopwatch.Start();

        if (suitApi.Biometry != null)
        {
            suitApi.Biometry.StartECG();
            suitApi.Biometry.StartGSR();
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
        yield return new WaitUntil(() => suitApi.Biometry is { ECGStarted: true });

        suitApi.Biometry.ECGUpdated += OnECGUpdate;
        
        ECGFrequency ecgFrequency = ECGFrequency.TS_ECG_FPS_10; // TS_ECG_FPS_20 -> 100 ms
        suitApi.Biometry.UpdateECGFrequency(ecgFrequency);

        Debug.Log($"Updated biometry options: ECG frequency is {ecgFrequency}.");
    }

    private IEnumerator UpdateGSRBiometriyOptions()
    {
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
            double elapsedTime = stopwatch.Elapsed.TotalMilliseconds;
            uint[] deltaTime = new uint[ECGBuffer.data.Length];
            float[] amplitude = new float[ECGBuffer.data.Length];

            for (int i = 0; i < ECGBuffer.data.Length; i++)
            {
                deltaTime[i] = ECGBuffer.data[i].deltaTime;
                amplitude[i] = ECGBuffer.data[i].mv;
                //Debug.Log($"Index: {i}, elapsedTime {elapsedTime}, deltaTime: {ECGBuffer.data[i].deltaTime.ToString()}, amplitude [mV]: {ECGBuffer.data[i].mv.ToString()}");
            }

            ECGData ecgData = new ECGData(elapsedTime, deltaTime, amplitude);
            if (recording) recordedECGData.Add(ecgData);
        }
    }

    private void OnGSRUpdate(ref GSRBuffer GSRBuffer, IntPtr opaque, ResultCode resultCode)
    {
        Debug.Log("OnGSRUpdate");

        if (resultCode == ResultCode.TS_SUCCESS)
        {
            double elapsedTime = stopwatch.Elapsed.TotalMilliseconds;

            for (int i = 0; i < GSRBuffer.data.Length; i++)
            {
                uint count = GSRBuffer.data[i].count;
                int[] data = GSRBuffer.data[i].data;

                GSRData gsrData = new GSRData(elapsedTime, count, data);
                if (recording) recordedGSRData.Add(gsrData);

                for (int j = 0; j < data.Length; j++)
                {
                    Debug.Log($" Index (i,j): {i}, {j}, elapsedTime {elapsedTime}, count: {GSRBuffer.data[i].count}, data: {GSRBuffer.data[i].data[j]}");
                }
            }
        }
    }

    public void StartStopRecording()
    {
        recording = !recording;
        if (recording) {
            stopwatch.Restart();
        }
    }

    public void Save()
    {
        fileManager.SaveToCSV(recordedECGData);
        fileManager.SaveToCSV(recordedGSRData);
    }

    public bool IsRecording()
    {
        return recording;
    }

    void OnDisable()
    {
        Debug.Log("Stopping ECG and GSR");

        suitApi.Biometry.StopECG();
        suitApi.Biometry.StopGSR();

        suitApi.Biometry.ECGUpdated -= OnECGUpdate;
        suitApi.Biometry.GSRUpdated -= OnGSRUpdate;
    }
}
