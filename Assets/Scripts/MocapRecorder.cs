using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeslasuitAPI;
using Debug = UnityEngine.Debug;
using System.Linq;
using System.Diagnostics;

public class MocapRecorder : MonoBehaviour
{
    private bool recording;
    private List<MocapData> recordedMocapData;

    private SuitAPIObject suitApi;
    private MocapSkeleton skeleton;

    //private Transform root;

    public Dictionary<HumanBodyBones, Vector3> jointRotations;
    private List<string> jointNames;

    private Animator animator;

    private void Start()
    {
        recording = false;
        recordedMocapData = new List<MocapData>();

        suitApi = GameObject.FindGameObjectWithTag("Teslasuit").GetComponentInChildren<SuitAPIObject>();
        StartCoroutine(UpdateMocapOptions());

        animator = this.GetComponent<Animator>();

        //root = GameObject.Find("Maniken_skeletool:root").transform;

        // The order must be same as in MocapData.cs
        jointRotations = new Dictionary<HumanBodyBones, Vector3>
            {
                {HumanBodyBones.Spine,          Vector3.zero},
                {HumanBodyBones.Chest,          Vector3.zero},
                {HumanBodyBones.RightUpperArm,  Vector3.zero},
                {HumanBodyBones.RightLowerArm,  Vector3.zero},
                {HumanBodyBones.LeftUpperArm,   Vector3.zero},
                {HumanBodyBones.LeftLowerArm,   Vector3.zero},
                {HumanBodyBones.RightUpperLeg,  Vector3.zero},
                {HumanBodyBones.RightLowerLeg,  Vector3.zero},
                {HumanBodyBones.LeftUpperLeg,   Vector3.zero},
                {HumanBodyBones.LeftLowerLeg,   Vector3.zero},
            };
        jointNames = jointRotations.Select(boneName => boneName.Key.ToString()).ToList();
    }

    private void Update()
    {
        foreach (var jointName in jointRotations.Keys.ToList())
        {
            Transform transform = animator.GetBoneTransform(jointName);
            jointRotations[jointName] = transform.eulerAngles;
        }
    }

    private IEnumerator UpdateMocapOptions()
    {
        suitApi.Start();
        yield return new WaitUntil(() => suitApi.Mocap is { isStarted: true });

        suitApi.Mocap.Updated += OnMocapUpdate;

        TSMocapOptions options = new TSMocapOptions();
        options.frequency = TSMocapFrequency.TS_MOCAP_FPS_200;
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
        MocapData suitData = new MocapData(DateTime.Now, slicedData, jointNames, jointRotations.Values.ToArray());

        if (recording) {
            recordedMocapData.Add(suitData);
        }
    }

    public void StartStopRecording()
    {
        recording = !recording;

        if (recording) {
            recordedMocapData.Clear();

            if (suitApi.Mocap != null)
            {
                suitApi.Mocap.Start();
            }
        }
    }

    public void Save()
    {
        FileManager.Instance().SaveMoCapData(recordedMocapData);
    }

    public bool IsRecording()
    {
        return recording;
    }

    void OnDisable()
    {
        if (suitApi != null && suitApi.Mocap != null)
        {
            Debug.Log("Stopping MoCap");

            suitApi.Mocap.Stop();
            suitApi.Mocap.Updated -= OnMocapUpdate;

            Save();
        }
    }
}
