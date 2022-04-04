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
	private SuitMocapSkeleton suitMocapSkeletonComponent;

	private Stopwatch stopwatch;
	private long nextReplayTime;
	private long replayStartTime;

	private int labelStartIndex = -1;
	private string _label;

	private Animator animator;
	private Transform root;

	private GetHeading getHeading;

	private void Start()
	{
		replaying = false;

		stopwatch = new Stopwatch();
		stopwatch.Start();

		animator = this.GetComponent<Animator>();
		
		root = GameObject.Find("Teslasuit_Man_Haptics").transform;

		suitApi = GameObject.FindGameObjectWithTag("Teslasuit").GetComponentInChildren<SuitAPIObject>();
		suitMocapSkeletonComponent = GameObject.FindGameObjectWithTag("Teslasuit").GetComponentInChildren<SuitMocapSkeleton>();

		getHeading = GameObject.Find("Maniken_skeletool:pelvis").GetComponent<GetHeading>();
	}

	private void Update()
	{
		if (replaying)
		{
			double currentTimestamp = stopwatch.Elapsed.TotalMilliseconds;
			//Debug.Log($"Elapsed {currentTimestamp} next {nextReplayTime}");

			if (currentTimestamp >= nextReplayTime)
			{
				MocapData currentReplayData = GetCurrentReplayData();

				for (int i = 0; i < currentReplayData.GetData().Length; i++)
                {
					Quaternion rawRotation = GetCurrentReplayRotation(currentReplayData.GetData()[i].mocap_bone_index);

					//Quaternion heading = root.transform.rotation;
					Quaternion heading = TransformExtensions.HeadingOffset(Quaternion.identity, root.transform.rotation);

					HumanBodyBones boneID = HumanBodyBones.LastBone;
					MocapBones.TeslasuitToUnityBones.TryGetValue((MocapBone)currentReplayData.GetData()[i].mocap_bone_index, out boneID);

					// Get joint offsets
					Quaternion userDefinedOffset = Quaternion.identity;
					Quaternion defaultOffset = Quaternion.identity;
					foreach (SuitMocapSkeletonNode node in suitMocapSkeletonComponent.mocapNodes)
					{
						if ((ulong)node.MocapBoneIndex == currentReplayData.GetData()[i].mocap_bone_index) {
							userDefinedOffset = node.userDefinedOffset;
							defaultOffset = node.defaultOffset;
						}
					}

					var res = rawRotation * userDefinedOffset.Inversed() * defaultOffset.Inversed();
					animator.GetBoneTransform(boneID).rotation = heading * res;
					
				}

				// RightUpperArm (Maniken_skeletool:shoulder_r)
				/*ApplyRotationForBone(currentReplayData, MocapBones.TeslasuitToUnityBones[MocapBone.RightUpperArm]);

				// RightLowerArm (Maniken_skeletool:shoulder_r)
				ApplyRotationForBone(currentReplayData, MocapBones.TeslasuitToUnityBones[MocapBone.RightLowerArm]);

				// LeftUpperArm (Maniken_skeletool:shoulder_l)
				ApplyRotationForBone(currentReplayData, MocapBones.TeslasuitToUnityBones[MocapBone.LeftUpperArm]);

				// LeftLowerArm (Maniken_skeletool:shoulder_l)
				ApplyRotationForBone(currentReplayData, MocapBones.TeslasuitToUnityBones[MocapBone.LeftLowerArm]);

				// Chest (Maniken_skeletool:spine_02)
				ApplyRotationForBone(currentReplayData, MocapBones.TeslasuitToUnityBones[MocapBone.Chest]);
				getHeading.UpdateEuler();

				// Spine (Maniken_skeletool:spine_01)
				ApplyRotationForBone(currentReplayData, MocapBones.TeslasuitToUnityBones[MocapBone.Spine]);

				// RightUpperLeg (Maniken_skeletool:hip_r)
				ApplyRotationForBone(currentReplayData, MocapBones.TeslasuitToUnityBones[MocapBone.RightUpperLeg]);

				// LeftUpperLeg (Maniken_skeletool:hip_l)
				ApplyRotationForBone(currentReplayData, MocapBones.TeslasuitToUnityBones[MocapBone.LeftUpperLeg]);

				// RightLowerLeg (Maniken_skeletool:foot_l)
				ApplyRotationForBone(currentReplayData, MocapBones.TeslasuitToUnityBones[MocapBone.RightLowerLeg]);

				// LeftLowerLeg (Maniken_skeletool:foot_l)
				ApplyRotationForBone(currentReplayData, MocapBones.TeslasuitToUnityBones[MocapBone.LeftLowerLeg]);*/
			}
		}

	}

	private void ApplyRotationForBone(MocapData replayData, HumanBodyBones bone)
	{
		Vector3 eulerAngles = Vector3.zero;
		Dictionary<string, Vector3> joints = replayData.GetJointRotations();
		if (joints.ContainsKey(bone.ToString()))
		{
			joints.TryGetValue(bone.ToString(), out eulerAngles);
		}

		animator.GetBoneTransform(bone).eulerAngles = eulerAngles;
	}

	public void Load(string filename)
	{
		replayData = FileManager.Instance().LoadMoCapData(filename);
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

	private Quaternion GetCurrentReplayRotation(ulong boneIndex)
	{
		int nodeIndex = FindNodeIndex(boneIndex);
		Quat4f tsQuat;
		if (nodeIndex == -1) tsQuat = Quat4f.Identity;
		else tsQuat = replayData[replayIndex].GetData()[nodeIndex].quat9x;

		return tsQuat.Quaternion();
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