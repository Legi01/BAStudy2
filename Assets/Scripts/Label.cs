using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TsSDK;
using UnityEngine;

[Serializable]
public class Label
{

	private DateTime timestamp;
	private string label;

	public Label(DateTime timestamp, string label)
	{
		this.timestamp = timestamp;
		this.label = label;
	}
	public long GetTimestamp()
	{
		long time = new DateTimeOffset(timestamp).ToUnixTimeMilliseconds();
		return time;
	}
	public string GetLabel()
	{
		return label;
	}

	public string ToCSV(string seperator, bool filtered = false)
	{
		StringBuilder sb = new StringBuilder();
		sb.Append(timestamp.ToString(Config.timestampFormat)).Append(seperator);
		sb.Append(label).Append(seperator);

		return sb.ToString();
	}

	public string GetCSVHeader(string seperator)
	{
		StringBuilder sb = new StringBuilder();
		sb.Append("timestamp").Append(seperator);
		sb.Append("label").Append("\n");

		return sb.ToString();
	}
}
