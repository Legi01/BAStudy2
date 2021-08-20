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
    private readonly List<SuitData> recordedSuitData = new List<SuitData>();
    
    private readonly FileManager fileManager = new FileManager();

    public Mocap motionCapture;

    public SuitAPIObject suitApi;
    private MocapSkeleton skeleton;

    private Stopwatch stopwatch = new Stopwatch();

    private void Start()
    {
        recording = false;
        MocapJoints mocapJoints = MocapJoints.GetInstance();
        stopwatch.Start();

        StartCoroutine(UpdateMocapOptions());
    }

    private IEnumerator UpdateMocapOptions()
    {
        yield return new WaitUntil(() => suitApi.Mocap is { isStarted: true });

        suitApi.Mocap.Updated += OnMocapUpdate;

        TSMocapOptions options = new TSMocapOptions();
        options.frequency = TSMocapFrequency.TS_MOCAP_FPS_50;
        options.sensors_mask = Config.TsMocapSensorMask();

        suitApi.Mocap.UpdateOptions(options);
        skeleton = suitApi.Mocap.Skeleton;
        Debug.Log($"Updated Mocap Options. Sensor mask is {options.sensors_mask}");
    }

    private void OnMocapUpdate()
    {
        SuitData suitData;
        
        TSMocapData[] data = skeleton.mocapData;
        TSMocapData[] slicedData = new TSMocapData[10];
        Array.Copy(data, slicedData, 10);
        suitData = new SuitData(slicedData, stopwatch.Elapsed.TotalMilliseconds, motionCapture.JointData.Values.ToArray());

        if (recording) recordedSuitData.Add(suitData);
    }

    public void StartStopRecording()
    {
        recording = !recording;
    }

    public void Save()
    {
        fileManager.SaveToCSV(recordedSuitData);
        fileManager.Save(recordedSuitData);
    }

    public bool IsRecording()
    {
        return recording;
    }
}
