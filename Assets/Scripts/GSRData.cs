using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TeslasuitAPI;
using UnityEngine;

[Serializable]
public class GSRData
{
    GSRBufferedData[] data;
    public double timestamp;

    public GSRData(GSRBufferedData[] data, double timestamp)
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
            GSRBufferedData gsrData = data[i];
            sb.Append(gsrData.data.GetValue(i));
        }

        return sb.ToString();
    }

    public string GetCSVHeader(string seperator)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("value").Append(seperator);

        sb.Append("\n");
        return sb.ToString();
    }
}
