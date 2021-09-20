using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TeslasuitAPI;
using UnityEngine;

[Serializable]
public class ECGData
{
    // Data consists of a delta timestamp and mV (amplitude or voltage, measured in millivolts)
    private ECG_MV[] data;
    private double timestamp;

    public ECGData(double timestamp, ECG_MV[] data)
    {
        this.timestamp = timestamp;
        this.data = data;
    }

    public ECG_MV[] GetData()
    {
        return data;
    }

    public double GetTimestamp()
    {
        return timestamp;
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
        sb.Append("timestemp").Append(seperator).Append("deltaTime").Append(seperator).Append("mv").Append(seperator);

        sb.Append("\n");
        return sb.ToString();
    }
}
