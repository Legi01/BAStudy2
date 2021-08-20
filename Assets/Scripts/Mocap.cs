using System;
using System.Collections.Generic;
using TeslasuitAPI;
using UnityEngine;

public class Mocap : MonoBehaviour
{
    private Transform _teslasuitMan;
    private MocapJoints _mocapJoints;
    
    // The current rotation of the spine.
    // Upon a TsMocapUpdate this is set by the SuitMocapSkeletonNode
    // instead of updating the transform. This is done to avoid jittering
    // when the spine z rotation is converted to pelvis rotation.
    public Quaternion tsSpine;
    private Vector3 _defaultSpineRotation;
    
    private Transform _pelvis;
    private Vector3 _defaultPelvisRotation;

    private Transform _rightWrist;

    private SkinnedMeshRenderer _meshRenderer;
    
    private Transform indicator;
    
    private Transform _jointPositionReferenceFrame;
    
    public Dictionary<string, Vector3> JointData => _jointData;
    private Dictionary<String, Vector3> _jointData;
    

    // Start is called before the first frame update
    void Start()
    {
        _mocapJoints = MocapJoints.GetInstance();
        _teslasuitMan = GameObject.Find("Teslasuit_Man").transform;

        _pelvis = _mocapJoints.GetJoint("pelvis");
        _defaultSpineRotation = _mocapJoints.GetJoint("lowerSpine").rotation.eulerAngles;
        _defaultPelvisRotation = _pelvis.rotation.eulerAngles;
        _defaultPelvisRotation.z = _defaultPelvisRotation.z + 15;
        _rightWrist = _mocapJoints.GetJoint("rightWrist");
        
        _meshRenderer = _teslasuitMan.GetComponentInChildren<SkinnedMeshRenderer>();

        indicator = GameObject.Find("Indicator").transform;
        
        _jointPositionReferenceFrame = GameObject.Find("ReferenceFrame").transform;
        
        _jointData = MocapJoints.GetInstance().JointZeroPositions;
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
        Bounds bounds = _meshRenderer.bounds;
        float height = bounds.center.y - bounds.extents.y;
        Vector3 teslasuitManPosition = _teslasuitMan.position;
        teslasuitManPosition.y = teslasuitManPosition.y - height;
        _teslasuitMan.position = teslasuitManPosition;
        
        // Updating direction indicator
        Vector3 indicatorRotation = indicator.rotation.eulerAngles;
        indicatorRotation.y = _defaultPelvisRotation.y + 90; // newPelvisRotation
        Quaternion indicatorQuaternion = Quaternion.Euler(indicatorRotation);
        indicator.rotation = indicatorQuaternion;
        
        // Setting reference frame and calculating relative joint position
        _jointPositionReferenceFrame.position = _rightWrist.position;
        _jointPositionReferenceFrame.rotation = indicatorQuaternion;
        
        foreach (var joint in _mocapJoints.JointNames)
        {
            _jointData[joint] = _jointPositionReferenceFrame.InverseTransformPoint(_mocapJoints.GetJoint(joint).position);
        }
    }
}
