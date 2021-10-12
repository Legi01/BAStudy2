using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TeslasuitAPI;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MocapReplay : MonoBehaviour
{
    private List<MocapData> replayData;
    private int replayIndex;
    private bool replaying;

    public SuitAPIObject suitApi;

    private Stopwatch stopwatch;
    private long nextReplayTime;
    private long replayStartTime;

    private int labelStartIndex = -1;
    private string _label;

    private FileManager fileManager = new FileManager();

    private Animator animator;
    private Transform root;

    private void Start()
    {
        replaying = false;

        stopwatch = new Stopwatch();
        stopwatch.Start();

        animator = this.GetComponent<Animator>();
        root = GameObject.Find("Maniken_skeletool:root").transform;
    }

    private void Update()
    {
        if (replaying)
        {
            double currentTimestamp = stopwatch.Elapsed.TotalMilliseconds;
            //long currentTimestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            Debug.Log($"Elapsed {currentTimestamp} next {nextReplayTime}");

            if (currentTimestamp >= nextReplayTime)
            {
                MocapData currentReplayData = GetCurrentReplayData();

                // TODO: Doesnt work
                /*for (int i = 0; i < replayData.GetData().Length; i++)
                {
                    Quaternion rawRotation = GetCurrentReplayRotation(replayData.GetData()[i].mocap_bone_index).Quaternion();

                    Quaternion heading = root.transform.rotation;

                    animator.GetBoneTransform(bone).rotation = heading * rawRotation;
                    Debug.Log(replayData.data[i].mocap_bone_index + " " + replayData.data[i].quat9x);
                }*/

                // RightUpperArm (Maniken_skeletool:shoulder_r)
                ApplyRotationForBone(currentReplayData, MocapBones.TeslasuitToUnityBones[MocapBone.RightUpperArm]);

                // RightLowerArm (Maniken_skeletool:shoulder_r)
                ApplyRotationForBone(currentReplayData, MocapBones.TeslasuitToUnityBones[MocapBone.RightLowerArm]);

                // LeftUpperArm (Maniken_skeletool:shoulder_l)
                ApplyRotationForBone(currentReplayData, MocapBones.TeslasuitToUnityBones[MocapBone.LeftUpperArm]);

                // LeftLowerArm (Maniken_skeletool:shoulder_l)
                ApplyRotationForBone(currentReplayData, MocapBones.TeslasuitToUnityBones[MocapBone.LeftLowerArm]);

                // Chest (Maniken_skeletool:spine_02)
                ApplyRotationForBone(currentReplayData, MocapBones.TeslasuitToUnityBones[MocapBone.Chest]);

                // Spine (Maniken_skeletool:spine_01)
                ApplyRotationForBone(currentReplayData, MocapBones.TeslasuitToUnityBones[MocapBone.Spine]);
            }
        }

    }

    private void ApplyRotationForBone(MocapData replayData, HumanBodyBones bone)
    {
        Vector3 eulerAngles = Vector3.one;
        Dictionary<string, Vector3> joints = replayData.GetJointRotations();
        if (joints.ContainsKey(bone.ToString()))
        {
            joints.TryGetValue(bone.ToString(), out eulerAngles);
        }

        animator.GetBoneTransform(bone).eulerAngles = eulerAngles;
    }

    public void Load(string filename)
    {
        replayData = fileManager.Load(filename);
    }

    public void StartStopReplay()
    {
        replaying = !replaying;

        if (replaying)
        {
            if (suitApi.Mocap != null)
            {
                suitApi.Mocap.Stop();
            }

            stopwatch.Restart();
            replayIndex = 0;
            replayStartTime = replayData[0].GetTimestamp();
            nextReplayTime = replayData[replayIndex + 1].GetTimestamp() - replayStartTime;
        }
    }

    private Quat4f GetCurrentReplayRotation(ulong boneIndex)
    {
        int nodeIndex = FindNodeIndex(boneIndex);
        if (nodeIndex == -1) return Quat4f.Identity;
        return replayData[replayIndex].GetData()[nodeIndex].quat9x;
    }

    private MocapData GetCurrentReplayData()
    {
        MocapData data = replayData[replayIndex];
        replayIndex = (replayIndex + 1) % replayData.Count;
        if (replayIndex == 0)
        {
            stopwatch.Restart();
        }
        nextReplayTime = replayData[replayIndex].GetTimestamp() - replayStartTime;
        return data;
    }

    private int FindNodeIndex(ulong boneIndex)
    {
        for (int index = 0; index < this.replayData[replayIndex].GetData().Length; ++index)
        {
            if ((long)this.replayData[replayIndex].GetData()[index].mocap_bone_index == (long)boneIndex)
                return index;
        }

        return -1;
    }

    public void SliderValueChanged(float value)
    {
        replayIndex = (int)(value * replayData.Count);
    }

    public bool IsReplaying()
    {
        return replaying;
    }

}