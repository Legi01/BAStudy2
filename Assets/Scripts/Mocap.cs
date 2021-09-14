using System;
using System.Collections.Generic;
using TeslasuitAPI;
using TeslasuitAPI.Utils;
using UnityEngine;

public class Mocap : MonoBehaviour
{
    private Transform _teslasuitMan;
    
    // The current rotation of the spine.
    // Upon a TsMocapUpdate this is set by the SuitMocapSkeletonNode
    // instead of updating the transform. This is done to avoid jittering
    // when the spine z rotation is converted to pelvis rotation.
    public Quaternion tsSpine;
    private Vector3 _defaultSpineRotation;
    
    private Transform _pelvis;
    private Vector3 _defaultPelvisRotation;

    private Transform root;
    private Transform _rightWrist;

    private SkinnedMeshRenderer _meshRenderer;
    
    private Transform indicator;
    
    private Transform _jointPositionReferenceFrame;
    
    public Dictionary<string, Vector3> jointRotations;
    public Dictionary<string, Transform> joints;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        _teslasuitMan = GameObject.Find("Teslasuit_Man").transform;

        root = GameObject.Find("Maniken_skeletool:root").transform;
        _pelvis = GameObject.Find("Maniken_skeletool:pelvis").transform;
        _defaultSpineRotation = animator.GetBoneTransform(MocapBones.TeslasuitToUnityBones[MocapBone.Spine]).rotation.eulerAngles;
        _defaultPelvisRotation = _pelvis.rotation.eulerAngles;
        _defaultPelvisRotation.z = _defaultPelvisRotation.z + 15;
        _rightWrist = animator.GetBoneTransform(MocapBones.TeslasuitToUnityBones[MocapBone.RightHand]);
        
        _meshRenderer = _teslasuitMan.GetComponentInChildren<SkinnedMeshRenderer>();

        indicator = GameObject.Find("Indicator").transform;
        
        _jointPositionReferenceFrame = GameObject.Find("ReferenceFrame").transform;
        
        jointRotations = new Dictionary<string, Vector3>();
        joints = new Dictionary<string, Transform>
            {
                {"Spine", animator.GetBoneTransform(MocapBones.TeslasuitToUnityBones[MocapBone.Spine])},
                {"Chest", animator.GetBoneTransform(MocapBones.TeslasuitToUnityBones[MocapBone.Chest])},
                {"RightUpperArm", animator.GetBoneTransform(MocapBones.TeslasuitToUnityBones[MocapBone.RightUpperArm])},
                {"RightLowerArm", animator.GetBoneTransform(MocapBones.TeslasuitToUnityBones[MocapBone.RightLowerArm])},
                {"LeftUpperArm", animator.GetBoneTransform(MocapBones.TeslasuitToUnityBones[MocapBone.LeftUpperArm])},
                {"LeftLowerArm", animator.GetBoneTransform(MocapBones.TeslasuitToUnityBones[MocapBone.LeftLowerArm])},
                {"RightUpperLeg", animator.GetBoneTransform(MocapBones.TeslasuitToUnityBones[MocapBone.RightUpperLeg])},
                {"RightLowerLeg", animator.GetBoneTransform(MocapBones.TeslasuitToUnityBones[MocapBone.RightLowerLeg])},
                {"LeftUpperLeg", animator.GetBoneTransform(MocapBones.TeslasuitToUnityBones[MocapBone.LeftUpperLeg])},
                {"LeftLowerLeg", animator.GetBoneTransform(MocapBones.TeslasuitToUnityBones[MocapBone.LeftLowerLeg])},
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
        Vector3 indicatorRotation = indicator.rotation.eulerAngles;
        indicatorRotation.y = _defaultPelvisRotation.y + 90; // newPelvisRotation
        Quaternion indicatorQuaternion = Quaternion.Euler(indicatorRotation);
        //indicator.rotation = indicatorQuaternion;

        
        //Quaternion heading = TransformExtensions.HeadingOffset(Quaternion.identity, root.transform.rotation);
        indicator.rotation = root.transform.rotation;

        // (0.0, 180.0, 0.0) (0.0, 180.0, 270.0)

        // Setting reference frame and calculating relative joint position
        _jointPositionReferenceFrame.position = _rightWrist.position;
        _jointPositionReferenceFrame.rotation = indicatorQuaternion;
        
        foreach (string jointName in joints.Keys)
        {
            jointRotations[jointName] = /*_jointPositionReferenceFrame.InverseTransformPoint*/(GetJoint(jointName).eulerAngles);
        }
    }

    public Transform GetJoint(String jointName)
    {
        return joints[jointName];
    }

}
