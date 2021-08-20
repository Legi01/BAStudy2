using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TeslasuitAPI;
using TeslasuitAPI.Utils;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MocapReplay : MonoBehaviour
{
    [SerializeField] private SuitMocapSkeleton suitMocapSkeleton;

    private List<SuitData> replayData;
    private int replayIndex;
    private Boolean replaying = false;

    private Boolean replayPaused = true;
    public bool ReplayPaused => replayPaused;

    private double nextReplayTime;
    private double firstReplayPointOffset;
    private double repReplayStartTime;

    private int labelStartIndex = -1;
    private string _label;

    private FileManager _fileManager = new FileManager();

    public Mocap motionCapture;

    public SuitAPIObject suitApi;
    private MocapSkeleton skeleton;

    private Stopwatch stopwatch = new Stopwatch();

    private void Start()
    {
        replaying = false;
        MocapJoints mocapJoints = MocapJoints.GetInstance();
        stopwatch.Start();

        StartCoroutine(UpdateMocapOptions());
    }

    private IEnumerator UpdateMocapOptions()
    {
        yield return new WaitUntil(() => suitApi.Mocap is { isStarted: true });

        //suitApi.Mocap.Updated += OnMocapUpdate;

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

        SuitData replayData = GetCurrentReplayData();
        suitData = new SuitData(replayData.data, stopwatch.Elapsed.TotalMilliseconds, replayData.jointData);
    }

    private void Update()
    {
        if (replaying)
        {
            // TODO: Doesnt work
            SuitData suitData;

            SuitData replayData = GetCurrentReplayData();
            suitData = new SuitData(replayData.data, stopwatch.Elapsed.TotalMilliseconds, replayData.jointData);

            skeleton.mocapData = suitData.data;
        }

    }

    public void Load()
    {
        replayData = _fileManager.Load();
    }

    public void StartStopReplay()
    {
        replayIndex = 0;
        replaying = !replaying;
        firstReplayPointOffset = replayData[0].timestamp;
        nextReplayTime = replayData[replayIndex + 1].timestamp - firstReplayPointOffset;
        repReplayStartTime = 0;
    }

    public void pauseResumeReplay()
    {
        replayPaused = !replayPaused;

        if (replayPaused)
        {
            stopwatch.Stop();
        }
        else
        {
            stopwatch.Start();
        }
    }

    public void goBackOneSecond()
    {
        replayIndex = replayIndex - 50;
    }

    public void markLabelStart(string label)
    {
        _label = label;
        labelStartIndex = replayIndex;
    }

    public void markLabelStop()
    {
        int labelStopIndex = replayIndex;

        for (int i = labelStartIndex; i <= labelStopIndex; i++)
        {
            replayData[i].label = _label;
        }
    }

    public Quat4f GetCurrentReplayRotation(ulong boneIndex)
    {
        int nodeIndex = FindNodeIndex(boneIndex);
        if (nodeIndex == -1) return Quat4f.Identity;
        return replayData[replayIndex].data[nodeIndex].quat9x;
    }

    public SuitData GetCurrentReplayData()
    {
        SuitData data = replayData[replayIndex];
        replayIndex = (replayIndex + 1) % replayData.Count;
        return data;
    }

    private int FindNodeIndex(ulong boneIndex)
    {
        for (int index = 0; index < this.replayData[replayIndex].data.Length; ++index)
        {
            if ((long)this.replayData[replayIndex].data[index].mocap_bone_index == (long)boneIndex)
                return index;
        }

        return -1;
    }

    public void sliderValueChanged(float value)
    {
        replayIndex = (int)(value * replayData.Count);
    }

    public Boolean IsReplaying()
    {
        return replaying;
    }
}