using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using TeslasuitAPI;
using Debug = UnityEngine.Debug;
using System.Linq;

public class MocapRecorder : MonoBehaviour
{
    private bool recording;
    private List<MocapData> recordedMocapData;
    
    private FileManager fileManager;

    public Mocap motionCapture;

    public SuitAPIObject suitApi;
    private MocapSkeleton skeleton;

    private readonly Stopwatch stopwatch = new Stopwatch();

    private void Start()
    {
        recording = false;
        recordedMocapData = new List<MocapData>();
        fileManager = new FileManager();
        stopwatch.Start();

        StartCoroutine(UpdateMocapOptions());
    }

    private IEnumerator UpdateMocapOptions()
    {
        yield return new WaitUntil(() => suitApi.Mocap is { isStarted: true });

        suitApi.Mocap.Updated += OnMocapUpdate;

        TSMocapOptions options = new TSMocapOptions();
        options.frequency = TSMocapFrequency.TS_MOCAP_FPS_100;
        options.sensors_mask = Config.TsMocapSensorMask();

        suitApi.Mocap.UpdateOptions(options);
        skeleton = suitApi.Mocap.Skeleton;
        Debug.Log($"Updated mocap options: MoCap frequency is {options.frequency}. Sensor mask is {options.sensors_mask}.");
    }

    private void OnMocapUpdate()
    {   
        TSMocapData[] data = skeleton.mocapData;
        TSMocapData[] slicedData = new TSMocapData[10];
        Array.Copy(data, slicedData, 10);
        MocapData suitData = new MocapData(slicedData, stopwatch.Elapsed.TotalMilliseconds, motionCapture.jointRotations.Values.ToArray());

        if (recording) recordedMocapData.Add(suitData);
    }

    public void StartStopRecording()
    {
        recording = !recording;
    }

    public void Save()
    {
        fileManager.SaveToCSV(recordedMocapData);
        fileManager.Save(recordedMocapData);
    }

    public bool IsRecording()
    {
        return recording;
    }

    /*void OnApplicationQuit()
    {
        Debug.Log("Stopping MoCap");
        suitApi.Mocap.Stop();
    }*/
}
