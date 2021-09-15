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
    private string label;
    private double timestamp;

    private Vector3[] eulerJointAngles; // Contains euler angles for each joint
    Dictionary<string, Vector3> eulerJointAnglesDictionary;
    public List<string> bones = new List<string> { // Must be same as in Mocap.cs
        MocapBone.Spine.ToString(),
        MocapBone.Chest.ToString(),
        MocapBone.RightUpperArm.ToString(),
        MocapBone.RightLowerArm.ToString(),
        MocapBone.LeftUpperArm.ToString(),
        MocapBone.LeftLowerArm.ToString(),
        MocapBone.RightUpperLeg.ToString(),
        MocapBone.RightLowerLeg.ToString(),
        MocapBone.LeftUpperLeg.ToString(),
        MocapBone.LeftLowerLeg.ToString(),
    };

    public MocapData(double timestamp, TSMocapData[] data, Vector3[] jointRotations)
    {
        this.timestamp = timestamp;
        this.data = data;
        this.eulerJointAngles = jointRotations;

        eulerJointAnglesDictionary = new Dictionary<string, Vector3>();
        int pos = 0;
        foreach (string jointName in bones)
        {
            eulerJointAnglesDictionary[jointName] = jointRotations[pos];
            ++pos;
        }
    }

    public TSMocapData[] GetData()
    {
        return data;
    }

    public Vector3[] GetJointRotations()
    {
        return eulerJointAngles;
    }

    public Dictionary<string, Vector3> GetEulerJointAngles()
    {
        return eulerJointAnglesDictionary;
    }

    public double GetTimestamp()
    {
        return timestamp;
    }

    public string GetLabel()
    {
        return label;
    }

    public string ToCSV(string seperator, bool filtered = false)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(timestamp.ToString(CultureInfo.InvariantCulture)).Append(seperator);
        if (!filtered) sb.Append(label).Append(seperator);

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
            Vector3 joint = eulerJointAngles[i];
            sb.Append(Vector3ToString(joint, seperator, endLine: i == eulerJointAngles.Length - 1));
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
        sb.Append("index").Append(seperator).Append("label").Append(seperator);

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

        foreach (string boneName in bones)
        {
            sb.Append(boneName + "_x").Append(seperator);
            sb.Append(boneName + "_y").Append(seperator);
            sb.Append(boneName + "_z").Append(seperator);
        }

        sb.Append("\n");
        return sb.ToString();
    }
}