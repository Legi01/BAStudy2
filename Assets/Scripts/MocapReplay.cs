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
    private List<SuitData> replayData;
    private int replayIndex;
    private bool replaying;

    private double nextReplayTime;
    private double firstReplayPointOffset;
    private double repReplayStartTime;

    private int labelStartIndex = -1;
    private string _label;

    private FileManager fileManager = new FileManager();

    public Mocap motionCapture;

    private Stopwatch stopwatch = new Stopwatch();

    private Animator animator;
    private Transform root;

    private void Start()
    {
        replaying = false;
        stopwatch.Start();

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
                SuitData replayData = GetCurrentReplayData();

                for (int i = 0; i < replayData.data.Length; i++)
                {
                    //Debug.Log(replayData.data[i].mocap_bone_index + " " + replayData.data[i].quat9x);

                    HumanBodyBones bone = HumanBodyBones.LastBone;
                    MocapBone mocapBone = MocapBone.LastBone;

                    switch (FindNodeIndex(replayData.data[i].mocap_bone_index))
                    {
                        case 0:
                            // RightUpperArm
                            bone = MocapBones.TeslasuitToUnityBones[MocapBone.RightUpperArm];
                            mocapBone = MocapBone.RightUpperArm;
                            break;
                        case 1:
                            // RightLowerArm
                            bone = MocapBones.TeslasuitToUnityBones[MocapBone.RightLowerArm];
                            mocapBone = MocapBone.RightLowerArm;
                            break;
                        case 2:
                            // LeftUpperArm
                            bone = MocapBones.TeslasuitToUnityBones[MocapBone.LeftUpperArm];
                            mocapBone = MocapBone.LeftUpperArm;
                            break;
                        case 3:
                            // LeftLowerArm
                            bone = MocapBones.TeslasuitToUnityBones[MocapBone.LeftLowerArm];
                            mocapBone = MocapBone.LeftLowerArm;
                            break;
                        case 4:
                            // Chest
                            bone = MocapBones.TeslasuitToUnityBones[MocapBone.Chest];
                            mocapBone = MocapBone.Chest;
                            break;
                        case 5:
                            // Spine
                            bone = MocapBones.TeslasuitToUnityBones[MocapBone.Spine];
                            mocapBone = MocapBone.Spine;
                            break;
                        default:
                            break;
                    }

                    if (bone != HumanBodyBones.LastBone)
                    {
                        // TODO: Doesnt work
                        Quaternion rawRotation = GetCurrentReplayRotation(replayData.data[i].mocap_bone_index).Quaternion();

                        //Quaternion heading = TransformExtensions.HeadingOffset(Quaternion.identity, root.transform.rotation);
                        //Quaternion heading = root.transform.rotation;

                        
                        Quaternion rotation = ConvertToBoneRotation(rawRotation, bone);
                        animator.GetBoneTransform(bone).rotation = rotation;

                        //Debug.Log(bone.ToString() + " quaternion " + q + " euler " + q.eulerAngles);

                    }
                }
            }
        }

    }

    private Quaternion Heading
    {
        get
        {
            return TransformExtensions.HeadingOffset(Quaternion.identity, root.transform.rotation);
        }
    }

    private Quaternion ConvertToBoneRotation(Quaternion rotation, HumanBodyBones bone)
    {
        Quaternion userDefinedOffset = Quaternion.identity;
        Quaternion defaultOffset = animator.GetBoneTransform(bone).rotation.Inversed() * (GetOrigin(bone) * NodeAlignmentOffset[bone]);
        Debug.Log(userDefinedOffset + " " + defaultOffset);


        var res = rotation * userDefinedOffset.Inversed() * (defaultOffset).Inversed();

        return Heading * res;
    }

    public void Load()
    {
        replayData = fileManager.Load();
    }

    public void StartStopReplay()
    {
        replayIndex = 0;
        replaying = !replaying;
        firstReplayPointOffset = replayData[0].timestamp;
        nextReplayTime = replayData[replayIndex + 1].timestamp - firstReplayPointOffset;
        repReplayStartTime = 0;

        if (replaying)
        {
            stopwatch.Start();
        }
        else
        {
            stopwatch.Stop();
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

    private Quat4f GetCurrentReplayRotation(ulong boneIndex)
    {
        int nodeIndex = FindNodeIndex(boneIndex);
        if (nodeIndex == -1) return Quat4f.Identity;
        return replayData[replayIndex].data[nodeIndex].quat9x;
    }

    private SuitData GetCurrentReplayData()
    {
        SuitData data = replayData[replayIndex];
        //replayIndex = (replayIndex + 1) % replayData.Count;
        nextReplayTime = replayData[replayIndex + 1].timestamp - firstReplayPointOffset;
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



    private static readonly Dictionary<HumanBodyBones, Quaternion> NodeAlignmentOffset = new Dictionary<HumanBodyBones, Quaternion>
        {
            { HumanBodyBones.RightUpperArm,         new Quaternion(0f, 0f, 0f, 1f) },
            { HumanBodyBones.LeftUpperArm,          new Quaternion(0f, 0f, 0f, 1f) },
            { HumanBodyBones.Spine,                 new Quaternion(0f, 0f, 0.707f, 0.707f) },
            { HumanBodyBones.Chest,                 new Quaternion(0f, 0f, 0.707f, 0.707f) },
            { HumanBodyBones.RightLowerArm,         new Quaternion(0f, 0f, 0.707f, 0.707f) },
            { HumanBodyBones.LeftLowerArm,          new Quaternion(0f, 0f, -0.707f, 0.707f) },
            { HumanBodyBones.RightUpperLeg,         new Quaternion(0f, 0f, 0f, 1f) },
            { HumanBodyBones.LeftUpperLeg,          new Quaternion(0f, 0f, -1f, 0f) },
            { HumanBodyBones.RightLowerLeg,         new Quaternion(0f, 0f, -0.924f, 0.383f) },
            { HumanBodyBones.LeftLowerLeg,          new Quaternion(0f, 0f, -0.383f, 0.924f) },
            { HumanBodyBones.RightHand,             new Quaternion(0f, 0f, 0f, 1f) },
            { HumanBodyBones.LeftHand,              new Quaternion(0f, 0f, 0f, 1f) },
            { HumanBodyBones.RightThumbProximal,    new Quaternion(0f, 0f, -0.609f, 0.793f) },
            { HumanBodyBones.RightIndexProximal,    new Quaternion(0f, 0f, -0.131f, 0.991f) },
            { HumanBodyBones.RightMiddleProximal,   new Quaternion(0f, 0f, 0f, 1f) },
            { HumanBodyBones.RightRingProximal,     new Quaternion(0f, 0f, 0.131f, 0.991f) },
            { HumanBodyBones.RightLittleProximal,   new Quaternion(0f, 0f, 0.259f, 0.966f) },
            { HumanBodyBones.LeftThumbProximal,     new Quaternion(0f, 0f, 0.609f, 0.793f) },
            { HumanBodyBones.LeftIndexProximal,     new Quaternion(0f, 0f, 0.131f, 0.991f) },
            { HumanBodyBones.LeftMiddleProximal,    new Quaternion(0f, 0f, 0f, 1f) },
            { HumanBodyBones.LeftRingProximal,      new Quaternion(0f, 0f, -0.131f, 0.991f) },
            { HumanBodyBones.LeftLittleProximal,    new Quaternion(0f, 0f, -0.259f, 0.966f) },
        };

    private static readonly Dictionary<HumanBodyBones, Vector3> BoneUpwards = new Dictionary<HumanBodyBones, Vector3>
        {
            { HumanBodyBones.RightUpperArm,         Vector3.up },
            { HumanBodyBones.LeftUpperArm,          Vector3.up },
            { HumanBodyBones.Spine,                 Vector3.right },
            { HumanBodyBones.Chest,                 Vector3.right },
            { HumanBodyBones.RightLowerArm,         Vector3.up },
            { HumanBodyBones.LeftLowerArm,          Vector3.up },
            { HumanBodyBones.RightUpperLeg,         Vector3.right },
            { HumanBodyBones.LeftUpperLeg,          Vector3.right },
            { HumanBodyBones.RightLowerLeg,         Vector3.right },
            { HumanBodyBones.LeftLowerLeg,          Vector3.right },
            { HumanBodyBones.RightHand,             Vector3.up },
            { HumanBodyBones.LeftHand,              Vector3.up },
            { HumanBodyBones.RightThumbProximal,    Vector3.up },
            { HumanBodyBones.RightIndexProximal,    Vector3.up },
            { HumanBodyBones.RightMiddleProximal,   Vector3.up },
            { HumanBodyBones.RightRingProximal,     Vector3.up },
            { HumanBodyBones.RightLittleProximal,   Vector3.up },
            { HumanBodyBones.LeftThumbProximal,     Vector3.up },
            { HumanBodyBones.LeftIndexProximal,     Vector3.up },
            { HumanBodyBones.LeftMiddleProximal,    Vector3.up },
            { HumanBodyBones.LeftRingProximal,      Vector3.up },
            { HumanBodyBones.LeftLittleProximal,    Vector3.up },
        };

    private enum Direction { Straight = 1, Reversed = -1 };

    private static readonly Dictionary<HumanBodyBones, Tuple<HumanBodyBones, Direction>> UpOrigins = new Dictionary<HumanBodyBones, Tuple<HumanBodyBones, Direction>>
        {
            { HumanBodyBones.Hips,                      new Tuple<HumanBodyBones, Direction>(HumanBodyBones.Spine,                  Direction.Straight)},
            { HumanBodyBones.Spine,                     new Tuple<HumanBodyBones, Direction>(HumanBodyBones.Chest,                  Direction.Straight)},
            { HumanBodyBones.Chest,                     new Tuple<HumanBodyBones, Direction>(HumanBodyBones.UpperChest,             Direction.Straight)},
            { HumanBodyBones.UpperChest,                new Tuple<HumanBodyBones, Direction>(HumanBodyBones.Neck,                   Direction.Straight)},
            { HumanBodyBones.Neck,                      new Tuple<HumanBodyBones, Direction>(HumanBodyBones.Head,                   Direction.Straight)},
            { HumanBodyBones.Head,                      new Tuple<HumanBodyBones, Direction>(HumanBodyBones.Neck,                   Direction.Reversed)},

            { HumanBodyBones.RightShoulder,             new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightUpperArm,          Direction.Reversed)},
            { HumanBodyBones.RightUpperArm,             new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightLowerArm,          Direction.Reversed)},
            { HumanBodyBones.RightLowerArm,             new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightHand,              Direction.Reversed)},
            { HumanBodyBones.RightHand,                 new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightMiddleProximal,    Direction.Reversed)},

            { HumanBodyBones.LeftShoulder,              new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftUpperArm,           Direction.Reversed)},
            { HumanBodyBones.LeftUpperArm,              new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftLowerArm,           Direction.Reversed)},
            { HumanBodyBones.LeftLowerArm,              new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftHand,               Direction.Reversed)},
            { HumanBodyBones.LeftHand,                  new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftMiddleProximal,     Direction.Reversed)},

            { HumanBodyBones.RightUpperLeg,             new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightLowerLeg,          Direction.Reversed)},
            { HumanBodyBones.RightLowerLeg,             new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightFoot,              Direction.Reversed)},
            { HumanBodyBones.RightFoot,                 new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightToes,              Direction.Reversed)},

            { HumanBodyBones.LeftUpperLeg,              new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftLowerLeg,           Direction.Reversed)},
            { HumanBodyBones.LeftLowerLeg,              new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftFoot,               Direction.Reversed)},
            { HumanBodyBones.LeftFoot,                  new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftToes,               Direction.Reversed)},

            //RH

            { HumanBodyBones.RightThumbProximal,         new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightThumbIntermediate,  Direction.Reversed)},
            { HumanBodyBones.RightThumbIntermediate,     new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightThumbDistal,        Direction.Reversed)},
            { HumanBodyBones.RightThumbDistal,           new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightThumbIntermediate,  Direction.Straight)},

            { HumanBodyBones.RightIndexProximal,         new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightIndexIntermediate,  Direction.Reversed)},
            { HumanBodyBones.RightIndexIntermediate,     new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightIndexDistal,        Direction.Reversed)},
            { HumanBodyBones.RightIndexDistal,           new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightIndexIntermediate,  Direction.Straight)},

            { HumanBodyBones.RightMiddleProximal,        new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightMiddleIntermediate, Direction.Reversed)},
            { HumanBodyBones.RightMiddleIntermediate,    new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightMiddleDistal,       Direction.Reversed)},
            { HumanBodyBones.RightMiddleDistal,          new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightMiddleIntermediate, Direction.Straight)},

            { HumanBodyBones.RightRingProximal,          new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightRingIntermediate,   Direction.Reversed)},
            { HumanBodyBones.RightRingIntermediate,      new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightRingDistal,         Direction.Reversed)},
            { HumanBodyBones.RightRingDistal,            new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightRingIntermediate,   Direction.Straight)},

            { HumanBodyBones.RightLittleProximal,        new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightLittleIntermediate, Direction.Reversed)},
            { HumanBodyBones.RightLittleIntermediate,    new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightLittleDistal,       Direction.Reversed)},
            { HumanBodyBones.RightLittleDistal,          new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightLittleIntermediate, Direction.Straight)},

            //LH

            { HumanBodyBones.LeftThumbProximal,         new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftThumbIntermediate,  Direction.Reversed)},
            { HumanBodyBones.LeftThumbIntermediate,     new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftThumbDistal,        Direction.Reversed)},
            { HumanBodyBones.LeftThumbDistal,           new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftThumbIntermediate,  Direction.Straight)},

            { HumanBodyBones.LeftIndexProximal,         new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftIndexIntermediate,  Direction.Reversed)},
            { HumanBodyBones.LeftIndexIntermediate,     new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftIndexDistal,        Direction.Reversed)},
            { HumanBodyBones.LeftIndexDistal,           new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftIndexIntermediate,  Direction.Straight)},

            { HumanBodyBones.LeftMiddleProximal,        new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftMiddleIntermediate, Direction.Reversed)},
            { HumanBodyBones.LeftMiddleIntermediate,    new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftMiddleDistal,       Direction.Reversed)},
            { HumanBodyBones.LeftMiddleDistal,          new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftMiddleIntermediate, Direction.Straight)},

            { HumanBodyBones.LeftRingProximal,          new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftRingIntermediate,   Direction.Reversed)},
            { HumanBodyBones.LeftRingIntermediate,      new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftRingDistal,         Direction.Reversed)},
            { HumanBodyBones.LeftRingDistal,            new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftRingIntermediate,   Direction.Straight)},

            { HumanBodyBones.LeftLittleProximal,        new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftLittleIntermediate, Direction.Reversed)},
            { HumanBodyBones.LeftLittleIntermediate,    new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftLittleDistal,       Direction.Reversed)},
            { HumanBodyBones.LeftLittleDistal,          new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftLittleIntermediate, Direction.Straight)}

        };

    private Quaternion GetOrigin(HumanBodyBones sourceBone)
    {
        //if (UpOrigins.ContainsKey(sourceBone) && _allBones.ContainsKey(sourceBone) && BoneUpwards.ContainsKey(sourceBone))
        {
            Vector3 forward = GetForward(sourceBone);
            Quaternion origin = Quaternion.LookRotation(forward, BoneUpwards[sourceBone]);
            return origin;
        }

        return Quaternion.identity;
    }

    private Vector3 GetForward(HumanBodyBones sourceBone)
    {
        Transform boneTransform = animator.GetBoneTransform(sourceBone);

        Vector3 bonePos = boneTransform.position;

        Tuple<HumanBodyBones, Direction> originBone = UpOrigins[sourceBone];
        HumanBodyBones targetBone = originBone.Item1;
        int direcion = (int)originBone.Item2;

        //if (_allBones.ContainsKey(targetBone))
        {
            Vector3 targetPos = animator.GetBoneTransform(sourceBone).position;
            return (targetPos - bonePos).normalized * direcion;
        }
        /*else if (boneTransform.childCount > 0)
        {
            Vector3 childPos = boneTransform.GetChild(0).position;
            return (childPos - bonePos).normalized * direcion;
        }*/

        return Vector3.zero;
    }
}