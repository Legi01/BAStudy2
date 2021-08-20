using System;
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
    private Boolean doReplay = false;
    public bool DoReplay => doReplay;

    private Boolean replayPaused = true;
    public bool ReplayPaused => replayPaused;
    
    private double nextReplayTime;
    private double firstReplayPointOffset;
    private double repReplayStartTime;

    private int labelStartIndex = -1;
    private string _label;
    
    private FileManager _fileManager = new FileManager();

    private Stopwatch _stopwatch = new Stopwatch();

    // private void Update()
    // {
    //     if (!replayPaused)
    //     {
    //         double elapsedTime = _stopwatch.Elapsed.TotalMilliseconds - repReplayStartTime;
    //         Debug.Log($"Elapsed {elapsedTime} next {nextReplayTime}");
    //         
    //         if (elapsedTime >= nextReplayTime)
    //         {
    //             replayIndex++;
    //
    //             if (replayIndex == replayData.Count - 1)
    //             {
    //                 replayIndex = 0;
    //                 repReplayStartTime = elapsedTime;
    //             }
    //
    //             nextReplayTime = replayData[replayIndex + 1].timestamp - firstReplayPointOffset;
    //         }
    //     }
    // }

    public void load()
    {
        replayData = _fileManager.load();
    }

    public void startStopReplay()
    {
        replayIndex = 0;
        doReplay = !doReplay;
        firstReplayPointOffset = replayData[0].timestamp;
        nextReplayTime = replayData[replayIndex + 1].timestamp - firstReplayPointOffset;
        repReplayStartTime = 0;
    }

    public void pauseResumeReplay()
    {
        replayPaused = !replayPaused;
        
        if (replayPaused)
            _stopwatch.Stop();
        else
        {
            _stopwatch.Start();
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
            if ((long) this.replayData[replayIndex].data[index].mocap_bone_index == (long) boneIndex)
                return index;
        }

        return -1;
    }

    public void sliderValueChanged(float value)
    {
        replayIndex = (int) (value * replayData.Count);
    }
}