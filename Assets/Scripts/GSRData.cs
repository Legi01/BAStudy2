using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TeslasuitAPI;
using UnityEngine;

[Serializable]
public class GSRData
{
    private double timestamp;
    private uint count;
    private int[] data;

    public GSRData(double timestamp, uint count, int[] data)
    {
        this.timestamp = timestamp;
        this.count = count;
        this.data = data;
    }

    public int[] GetData()
    {
        return data;
    }

    public uint GetCount()
    {
        return count;
    }

    public double GetTimestamp()
    {
        return timestamp;
    }

    public string ToCSV(string seperator)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(timestamp.ToString(CultureInfo.InvariantCulture)).Append(seperator);

        sb.Append(count).Append(seperator);

        for (int i = 0; i < data.Length; i++)
        {
            sb.Append(data[i]);
            sb.Append(seperator);
        }

        return sb.ToString();
    }

    public string GetCSVHeader(string seperator)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("timestemp").Append(seperator).Append("value").Append(seperator);

        sb.Append("count").Append(seperator);

        for (int i = 0; i < data.Length; i++)
        {
            sb.Append("data_" + i);
            sb.Append(seperator);
        }

        sb.Append("\n");
        return sb.ToString();
    }
}
