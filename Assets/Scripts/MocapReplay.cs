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
    private List<MocapData> replayData;
    private int replayIndex;
    private bool replaying;

    private SuitAPIObject suitApi;

    private double nextReplayTime;
    private double firstReplayPointOffset;
    private double repReplayStartTime;

    private int labelStartIndex = -1;
    private string _label;

    private FileManager fileManager = new FileManager();

    private Stopwatch stopwatch = new Stopwatch();

    private Animator animator;
    private Transform root;

    private void Start()
    {
        replaying = false;
        stopwatch.Start();

        suitApi = this.GetComponent<SuitAPIObject>();

        animator = this.GetComponent<Animator>();
        root = GameObject.Find("Maniken_skeletool:root").transform;
    }

    private void Update()
    {
        if (replaying)
        {
            double elapsedTime = stopwatch.Elapsed.TotalMilliseconds - repReplayStartTime;
            //Debug.Log($"Elapsed {elapsedTime} next {nextReplayTime}");

            if (elapsedTime >= nextReplayTime)
            {
               MocapData replayData = GetCurrentReplayData();

                for (int i = 0; i < replayData.GetData().Length; i++)
                {
                    //Debug.Log(replayData.data[i].mocap_bone_index + " " + replayData.data[i].quat9x);

                    HumanBodyBones bone = HumanBodyBones.LastBone;
                    MocapBone mocapBone = MocapBone.LastBone;

                    switch (FindNodeIndex(replayData.GetData()[i].mocap_bone_index))
                    {
                        case 0:
                            // RightUpperArm (Maniken_skeletool:shoulder_r)
                            bone = MocapBones.TeslasuitToUnityBones[MocapBone.RightUpperArm];
                            mocapBone = MocapBone.RightUpperArm;
                            break;
                        case 1:
                            // RightLowerArm (Maniken_skeletool:shoulder_r)
                            bone = MocapBones.TeslasuitToUnityBones[MocapBone.RightLowerArm];
                            mocapBone = MocapBone.RightLowerArm;
                            break;
                        case 2:
                            // LeftUpperArm (Maniken_skeletool:shoulder_l)
                            bone = MocapBones.TeslasuitToUnityBones[MocapBone.LeftUpperArm];
                            mocapBone = MocapBone.LeftUpperArm;
                            break;
                        case 3:
                            // LeftLowerArm (Maniken_skeletool:shoulder_l)
                            bone = MocapBones.TeslasuitToUnityBones[MocapBone.LeftLowerArm];
                            mocapBone = MocapBone.LeftLowerArm;
                            break;
                        case 4:
                            // Chest (Maniken_skeletool:spine_02)
                            bone = MocapBones.TeslasuitToUnityBones[MocapBone.Chest];
                            mocapBone = MocapBone.Chest;
                            break;
                        case 5:
                            // Spine (Maniken_skeletool:spine_01)
                            bone = MocapBones.TeslasuitToUnityBones[MocapBone.Spine];
                            mocapBone = MocapBone.Spine;
                            break;
                        default:
                            break;
                    }

                    if (bone != HumanBodyBones.LastBone)
                    {
                        // TODO: Doesnt work
                        Quaternion rawRotation = GetCurrentReplayRotation(replayData.GetData()[i].mocap_bone_index).Quaternion();

                        /*Quaternion heading = TransformExtensions.HeadingOffset(Quaternion.identity, root.transform.rotation);
                        //Quaternion heading = root.transform.rotation;

                        animator.GetBoneTransform(bone).rotation = heading * rawRotation;*/

                        animator.GetBoneTransform(bone).localEulerAngles = GetEulerJointAngles(mocapBone); // working

                        /*if (mocapBone == MocapBone.Spine)
                        {
                            Quaternion test = animator.GetBoneTransform(bone).localRotation;
                            Debug.Log(bone.ToString() + " quaternion " + rawRotation + " euler " + rawRotation.eulerAngles + " eulerangle " + test);

                        }*/
                    }
                }
            }
        }

    }

    public void Load()
    {
        replayData = fileManager.Load();
    }

    public void StartStopReplay()
    {
        replayIndex = 0;
        replaying = !replaying;
        firstReplayPointOffset = replayData[0].GetTimestamp();
        nextReplayTime = replayData[replayIndex + 1].GetTimestamp() - firstReplayPointOffset;
        repReplayStartTime = 0;

        if (replaying)
        {
            stopwatch.Start();
            suitApi.Mocap.Stop();
        }
        else
        {
            stopwatch.Stop();
            suitApi.Mocap.Start();
        }
    }

    private Quat4f GetCurrentReplayRotation(ulong boneIndex)
    {
        int nodeIndex = FindNodeIndex(boneIndex);
        if (nodeIndex == -1) return Quat4f.Identity;
        return replayData[replayIndex].GetData()[nodeIndex].quat9x;
    }

    private Vector3 GetEulerJointAngles(MocapBone boneName)
    {
        if (!replayData[replayIndex].GetEulerJointAngles().ContainsKey(boneName.ToString())) return Vector3.zero;
        return replayData[replayIndex].GetEulerJointAngles()[boneName.ToString()].Vector3f().Vector3();
    }

    private MocapData GetCurrentReplayData()
    {
        MocapData data = replayData[replayIndex];
        replayIndex = (replayIndex + 1) % replayData.Count;
        nextReplayTime = replayData[replayIndex + 1].GetTimestamp() - firstReplayPointOffset;
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