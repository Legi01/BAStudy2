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

    public SuitAPIObject suitApi;
    private MocapSkeleton skeleton;

    private readonly Stopwatch stopwatch = new Stopwatch();

    //private Transform _teslasuitMan;

    // The current rotation of the spine.
    // Upon a TsMocapUpdate this is set by the SuitMocapSkeletonNode
    // instead of updating the transform. This is done to avoid jittering
    // when the spine z rotation is converted to pelvis rotation.
    //private Quaternion tsSpine;
    //private Vector3 _defaultSpineRotation;

    //private Transform _pelvis;
    //private Vector3 _defaultPelvisRotation;

    private Transform root;
    //private Transform _rightWrist;

    //private SkinnedMeshRenderer _meshRenderer;

    private Transform indicator;

    //private Transform _jointPositionReferenceFrame;

    public Dictionary<MocapBone, Vector3> jointRotations;

    private Animator animator;

    private void Start()
    {
        recording = false;
        recordedMocapData = new List<MocapData>();
        fileManager = new FileManager();
        stopwatch.Start();

        StartCoroutine(UpdateMocapOptions());

        animator = this.GetComponent<Animator>();
        //_teslasuitMan = GameObject.Find("Teslasuit_Man").transform;

        root = GameObject.Find("Maniken_skeletool:root").transform;
        /*_pelvis = GameObject.Find("Maniken_skeletool:pelvis").transform;
        _defaultSpineRotation = animator.GetBoneTransform(MocapBones.TeslasuitToUnityBones[MocapBone.Spine]).rotation.eulerAngles;
        _defaultPelvisRotation = _pelvis.rotation.eulerAngles;
        _defaultPelvisRotation.z = _defaultPelvisRotation.z + 15;
        _rightWrist = animator.GetBoneTransform(MocapBones.TeslasuitToUnityBones[MocapBone.RightHand]);*/

        //_meshRenderer = _teslasuitMan.GetComponentInChildren<SkinnedMeshRenderer>();

        indicator = GameObject.Find("Indicator").transform;

        //_jointPositionReferenceFrame = GameObject.Find("ReferenceFrame").transform;

        // Must be same as in MocapData.cs
        jointRotations = new Dictionary<MocapBone, Vector3>
            {
                {MocapBone.Spine,          Vector3.zero},
                {MocapBone.Chest,          Vector3.zero},
                {MocapBone.RightUpperArm,  Vector3.zero},
                {MocapBone.RightLowerArm,  Vector3.zero},
                {MocapBone.LeftUpperArm,   Vector3.zero},
                {MocapBone.LeftLowerArm,   Vector3.zero},
                {MocapBone.RightUpperLeg,  Vector3.zero},
                {MocapBone.RightLowerLeg,  Vector3.zero},
                {MocapBone.LeftUpperLeg,   Vector3.zero},
                {MocapBone.LeftLowerLeg,   Vector3.zero},
            };
    }

    private void Update()
    {
        // Converts Spine Z rotation to pelvis z rotation.
        /*if (float.IsNaN(tsSpine.w)) return;
        float spineRotation = tsSpine.eulerAngles.z - _defaultSpineRotation.z;
        Vector3 newPelvisRotation = _pelvis.rotation.eulerAngles;
        newPelvisRotation.z = _defaultPelvisRotation.z - spineRotation;
        _pelvis.rotation = Quaternion.Euler(newPelvisRotation);*/


        // Updating TeslasuitMan Position, so its always grounded
        /*Bounds bounds = _meshRenderer.bounds;
        float height = bounds.center.y - bounds.extents.y;
        Vector3 teslasuitManPosition = _teslasuitMan.position;
        teslasuitManPosition.y = teslasuitManPosition.y - height;
        _teslasuitMan.position = teslasuitManPosition;*/

        // Updating direction indicator
        /*Vector3 indicatorRotation = indicator.rotation.eulerAngles;
        indicatorRotation.y = _defaultPelvisRotation.y + 90; // newPelvisRotation
        Quaternion indicatorQuaternion = Quaternion.Euler(indicatorRotation);*/
        //indicator.rotation = indicatorQuaternion;


        //Quaternion heading = TransformExtensions.HeadingOffset(Quaternion.identity, root.transform.rotation);
        indicator.rotation = root.transform.rotation;

        // Setting reference frame and calculating relative joint position
        /*_jointPositionReferenceFrame.position = _rightWrist.position;
        _jointPositionReferenceFrame.rotation = indicatorQuaternion;*/

        foreach (MocapBone jointName in jointRotations.Keys)
        {
            Transform transform = animator.GetBoneTransform(MocapBones.TeslasuitToUnityBones[jointName]);
            jointRotations[jointName] = /*_jointPositionReferenceFrame.InverseTransformPoint*/(transform.eulerAngles);
        }
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
        MocapData suitData = new MocapData(stopwatch.Elapsed.TotalMilliseconds, slicedData, jointRotations.Values.ToArray());

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
