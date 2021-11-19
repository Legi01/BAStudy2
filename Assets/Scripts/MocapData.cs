using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TeslasuitAPI;
using UnityEngine;

[Serializable]
public class MocapData
{
	private TSMocapData[] data;
	private DateTime timestamp;
	private Vector3[] eulerJointAngles;
	private List<string> jointNames;

	public MocapData(DateTime timestamp, TSMocapData[] data, List<string> jointNames, Vector3[] eulerJointAngles)
	{
		this.timestamp = timestamp;
		this.data = data;
		this.jointNames = jointNames;
		this.eulerJointAngles = eulerJointAngles;
	}

	public TSMocapData[] GetData()
	{
		return data;
	}

	public Dictionary<string, Vector3> GetJointRotations()
	{
		Dictionary<string, Vector3> joints = new Dictionary<string, Vector3>();
		for (int i = 0; i < eulerJointAngles.Length; i++)
		{
			joints.Add(jointNames[i], eulerJointAngles[i]);
		}

		return joints;
	}

	public long GetTimestamp()
	{
		long time = new DateTimeOffset(timestamp).ToUnixTimeMilliseconds();
		return time;
	}

	public string ToCSV(string seperator, bool filtered = false)
	{
		StringBuilder sb = new StringBuilder();
		sb.Append(timestamp.ToString(Config.timestampFormat)).Append(seperator);

		for (int i = 0; i < data.Length; i++)
		{
			TSMocapData tsMocapData = data[i];

			if (!filtered)
			{
				sb.Append(tsMocapData.mocap_bone_index.ToString()).Append(seperator);
			}

			if (Config.streamedProperties["quat9x"])
				sb.Append(QuatToString(tsMocapData.quat9x, seperator));
			if (Config.streamedProperties["quat6x"])
				sb.Append(QuatToString(tsMocapData.quat6x, seperator));
			if (Config.streamedProperties["gyroscope"])
				sb.Append(Vector3sToString(tsMocapData.gyroscope, seperator));
			if (Config.streamedProperties["magnetometer"])
				sb.Append(Vector3sToString(tsMocapData.magnetometer, seperator));
			if (Config.streamedProperties["accelerometer"])
				sb.Append(Vector3sToString(tsMocapData.accelerometer, seperator));
			if (Config.streamedProperties["linearAccel"])
				sb.Append(Vector3sToString(tsMocapData.linear_accel, seperator));
			if (Config.streamedProperties["temperature"])
				sb.Append(tsMocapData.temperature.ToString()).Append(seperator);
		}

		for (int i = 0; i < eulerJointAngles.Length; i++)
		{
			sb.Append(Vector3ToString(eulerJointAngles[i], seperator, endLine: i == eulerJointAngles.Length - 1));
		}

		return sb.ToString();
	}

	private string QuatToString(Quat4f quat4F, string seperator)
	{
		StringBuilder sb = new StringBuilder();
		sb.Append(quat4F.w.ToString(CultureInfo.InvariantCulture)).Append(seperator)
			.Append(quat4F.x.ToString(CultureInfo.InvariantCulture)).Append(seperator)
			.Append(quat4F.y.ToString(CultureInfo.InvariantCulture)).Append(seperator)
			.Append(quat4F.z.ToString(CultureInfo.InvariantCulture)).Append(seperator);

		return sb.ToString();
	}

	private string Vector3sToString(Vector3s vector3S, string separator)
	{
		StringBuilder sb = new StringBuilder();
		sb.Append(vector3S.x.ToString(CultureInfo.InvariantCulture)).Append(separator)
			.Append(vector3S.y.ToString(CultureInfo.InvariantCulture)).Append(separator)
			.Append(vector3S.z.ToString(CultureInfo.InvariantCulture)).Append(separator);

		return sb.ToString();
	}

	private string Vector3ToString(Vector3 vector, string separator, bool endLine = false)
	{
		StringBuilder sb = new StringBuilder();
		sb.Append(vector.x.ToString(CultureInfo.InvariantCulture)).Append(separator)
			.Append(vector.y.ToString(CultureInfo.InvariantCulture)).Append(separator)
			.Append(vector.z.ToString(CultureInfo.InvariantCulture));

		if (!endLine)
		{
			sb.Append(separator);
		}

		return sb.ToString();
	}

	public string GetCSVHeader(string seperator)
	{
		StringBuilder sb = new StringBuilder();
		sb.Append("timestamp").Append(seperator);

		foreach (var tsMocapData in data)
		{
			string nodeName = Enum.GetName(typeof(MocapBone), tsMocapData.mocap_bone_index);
			sb.Append(nodeName + "_boneIndex").Append(seperator);

			foreach (var property in Config.propertyNames)
			{
				// Property not streamed -> Continue
				if (!Config.streamedProperties[property])
					continue;

				// Temperature only has a single value
				if (property.Equals("temperature"))
				{
					sb.Append(nodeName + "_" + property).Append(seperator);
					continue;
				}

				// For Quats, add w component
				if (property.Equals("quat9x") || property.Equals("quat6x"))
				{
					sb.Append(nodeName + "_" + property + "_w").Append(seperator);
				}

				// Everything else has x, y, z.
				sb.Append(nodeName + "_" + property + "_x").Append(seperator);
				sb.Append(nodeName + "_" + property + "_y").Append(seperator);
				sb.Append(nodeName + "_" + property + "_z").Append(seperator);
			}
		}

		foreach (string boneName in jointNames)
		{
			sb.Append(boneName + "_x").Append(seperator);
			sb.Append(boneName + "_y").Append(seperator);
			sb.Append(boneName + "_z").Append(seperator);
		}

		sb.Append("\n");
		return sb.ToString();
	}
}