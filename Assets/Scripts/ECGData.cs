using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TeslasuitAPI;
using UnityEngine;

[Serializable]
public class ECGData
{
    ECG_MV[] data;
    public double timestamp;

    public ECGData(ECG_MV[] data, double timestamp)
    {
        this.data = data;
        this.timestamp = timestamp;
    }

    public string ToCSV(string seperator)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(timestamp.ToString(CultureInfo.InvariantCulture)).Append(seperator);

        for (int i = 0; i < data.Length; i++)
        {
            ECG_MV ecgData = data[i];
            sb.Append(ecgData.deltaTime);
            sb.Append(ecgData.mv);
        }

        return sb.ToString();
    }

    public string GetCSVHeader(string seperator)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("deltaTime").Append(seperator).Append("mv").Append(seperator);

        sb.Append("\n");
        return sb.ToString();
    }
}
