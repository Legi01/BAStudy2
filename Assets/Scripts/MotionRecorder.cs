using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using TeslasuitAPI;
using Debug = UnityEngine.Debug;
using System.Linq;

public class MotionRecorder : MonoBehaviour
{
    private bool _isRecording = false;
    private readonly List<SuitData> _recordedSuitData = new List<SuitData>();
    
    private readonly FileManager _fileManager = new FileManager();

    public MotionCapture motionCapture;
    public MocapReplay mocapReplay;

    public SuitAPIObject suitApi;
    private MocapSkeleton skeleton;

    private Stopwatch stopwatch = new Stopwatch();

    private void Start()
    {
        MocapJoints mocapJoints = MocapJoints.GetInstance();
        stopwatch.Start();

        StartCoroutine(UpdateMocapOptions());
    }

    private IEnumerator UpdateMocapOptions()
    {
        yield return new WaitUntil(() => suitApi.Mocap is { isStarted: true });

        suitApi.Mocap.Updated += OnMocapUpdate;

        TSMocapOptions thesisOptions = new TSMocapOptions();
        thesisOptions.frequency = TSMocapFrequency.TS_MOCAP_FPS_50;
        thesisOptions.sensors_mask = Config.TsMocapSensorMask();

        suitApi.Mocap.UpdateOptions(thesisOptions);
        skeleton = suitApi.Mocap.Skeleton;
        Debug.Log($"Updated Mocap Options. Sensor mask is {thesisOptions.sensors_mask}");
    }

    private void OnMocapUpdate()
    {
        // PerformanceAnalyzer.GetInstance().TSOnMocapDataUpdate(dataIndex);
        SuitData suitData;

        if (mocapReplay.DoReplay)
        {
            SuitData replayData = mocapReplay.GetCurrentReplayData();
            suitData = new SuitData(replayData.data, stopwatch.Elapsed.TotalMilliseconds, replayData.jointData);
        }
        else
        {
            TSMocapData[] data = skeleton.mocapData;
            TSMocapData[] slicedData = new TSMocapData[10];
            Array.Copy(data, slicedData, 10);
            suitData = new SuitData(slicedData, stopwatch.Elapsed.TotalMilliseconds, motionCapture.JointData.Values.ToArray());
        }

        if (!_isRecording) return;
        _recordedSuitData.Add(suitData);
    }

    public void StartStopRecording(bool shouldRecord)
    {
        _isRecording = shouldRecord;
    }

    public void Save()
    {
        _fileManager.saveToCSV(_recordedSuitData);
    }
    
    
}
