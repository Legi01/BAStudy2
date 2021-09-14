using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using TeslasuitAPI;
using Debug = UnityEngine.Debug;
using System.Linq;

public class BiometricRecorder : MonoBehaviour
{
    private bool recording;
    private List<ECGData> recordedECGData;
    private List<GSRData> recordedGSRData;

    private FileManager fileManager;

    public SuitAPIObject suitApi;

    private readonly Stopwatch stopwatch = new Stopwatch();

    // Start is called before the first frame update
    void Start()
    {
        /*IBiometry biometry = suitApi.Biometry;
        biometry.StartECG();
        biometry.StartGSR();*/

        recording = false;
        recordedECGData = new List<ECGData>();
        recordedGSRData = new List<GSRData>();
        fileManager = new FileManager();
        stopwatch.Start();

        StartCoroutine(UpdateBiometriyOptions());
    }

    // Update is called once per frame
    void Update()
    {

     
    }

    private IEnumerator UpdateBiometriyOptions()
    {
        yield return new WaitUntil(() => suitApi.Biometry is { ECGStarted: true });
        yield return new WaitUntil(() => suitApi.Biometry is { GSRStarted: true });

        suitApi.Biometry.ECGUpdated += OnECGUpdate;
        suitApi.Biometry.GSRUpdated += OnGSRUpdate;

        ECGFrequency ecgFrequency = ECGFrequency.TS_ECG_FPS_20;
        GSRFrequency gsrFrequency = GSRFrequency.TS_GSR_FPS_60;
        suitApi.Biometry.UpdateECGFrequency(ECGFrequency.TS_ECG_FPS_20);
        suitApi.Biometry.UpdateGSRFrequency(GSRFrequency.TS_GSR_FPS_60);

        Debug.Log($"Updated biometry options: ECG frequency is {ecgFrequency}. GSR frequency is {gsrFrequency}.");

        //OnECGUpdated();
    }

    private void OnECGUpdate(ref ECGBuffer_MV ECGBuffer, IntPtr opaque, ResultCode resultCode)
    {
        ECGData ecgData = new ECGData(ECGBuffer.data, stopwatch.Elapsed.TotalMilliseconds);
        if (recording) recordedECGData.Add(ecgData);

        Debug.Log(ECGBuffer.data.GetValue(ECGBuffer.data.Length - 1));
    }

    private void OnGSRUpdate(ref GSRBuffer GSRBuffer, IntPtr opaque, ResultCode resultCode)
    {
        GSRData gsrData = new GSRData(GSRBuffer.data, stopwatch.Elapsed.TotalMilliseconds);
        if (recording) recordedGSRData.Add(gsrData);

        Debug.Log(GSRBuffer.data.GetValue(GSRBuffer.data.Length - 1));
    }

    public void StartStopRecording()
    {
        recording = !recording;
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

    /*void OnApplicationQuit()
    {
        Debug.Log("Stopping ECG and GSR");
        suitApi.Biometry.StopECG();
        suitApi.Biometry.StopGSR();
    }*/
}
