using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TeslasuitAPI;
using UnityEngine;

[Serializable]
public class ECGData
{
    // Data consists of a delta timestamp and amplitude, measured in millivolts [mV]
    private double timestamp;
    private float[] amplitude;
    private uint[] deltaTime;

    private float[] amplitudes;

    public ECGData(double timestamp, uint[] deltaTime, float[] amplitude)
    {
        this.timestamp = timestamp;
        this.deltaTime = deltaTime;
        this.amplitude = amplitude;
    }

    public float[] GetAmplitude()
    {
        return amplitude;
    }

    public uint[] GetDeltaTime()
    {
        // TODO: Bug or always zero?
        return deltaTime;
    }

    public double GetTimestamp()
    {
        return timestamp;
    }

    public string ToCSV(string seperator)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(timestamp.ToString(CultureInfo.InvariantCulture)).Append(seperator);

        for (int i = 0; i < amplitude.Length; i++)
        {
            sb.Append(deltaTime[i]);
            sb.Append(seperator);
            sb.Append(amplitude[i]);
            sb.Append(seperator);
        }

        return sb.ToString();
    }

    public string GetCSVHeader(string seperator)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("timestemp").Append(seperator);

        for (int i = 0; i < amplitude.Length; i++) {
            sb.Append("deltaTime_" + i);
            sb.Append(seperator);
            sb.Append("amplitude[mv]_" + i);
            sb.Append(seperator);
        }
        
        sb.Append("\n");
        return sb.ToString();
    }
}
