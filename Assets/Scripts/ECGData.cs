using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TsSDK;
using UnityEngine;

[Serializable]
public class ECGData
{
	// Data consists of a delta timestamp and amplitude, measured in millivolts [mV]
	private DateTime timestamp;
	private ulong deltaTime;
	private float hr;
	private bool hrValid;

	public ECGData(DateTime timestamp, ulong deltaTime, float hr, bool hrValid)
	{
		this.timestamp = timestamp;
		this.deltaTime = deltaTime;
		this.hr = hr;
		this.hrValid = hrValid;
	}

	public float GetHR()
	{
		return hr;
	}

	public ulong GetDeltaTime()
	{
		// TODO: Bug or always zero?
		return deltaTime;
	}

	public long GetTimestamp()
	{
		long time = new DateTimeOffset(timestamp).ToUnixTimeMilliseconds();
		return time;
	}

	public string ToCSV(string seperator)
	{
		StringBuilder sb = new StringBuilder();
		sb.Append(timestamp.ToString(Config.timestampFormat));
		sb.Append(seperator);
		sb.Append(deltaTime);
		sb.Append(seperator);
		sb.Append(hr);
		sb.Append(seperator);
		sb.Append(hrValid);

		return sb.ToString();
	}

	public string GetCSVHeader(string seperator)
	{
		StringBuilder sb = new StringBuilder();
		sb.Append("timestamp");
		sb.Append(seperator);
		sb.Append("deltaTime");
		sb.Append(seperator);
		sb.Append("hr");
		sb.Append(seperator);
		sb.Append("hrValid");

		sb.Append("\n");
		return sb.ToString();
	}
}
