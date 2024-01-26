using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using TsSDK;
using System.IO;
using System.Threading;

public class FileManager
{
	public static FileManager _instance = null;

	private SurrogateSelector _surrogateSelector;
	private const string seperator = ";";

	public bool savingData;

	/* singleton */
	public static FileManager Instance()
	{
		if (_instance == null)
		{
			_instance = new FileManager();

		}
		return _instance;
	}

	private FileManager()
	{
		savingData = false;
	}

	public void SaveECGData(List<ECGData> data)
	{
		if (data == null || data.Count == 0) return;

		string subjectID = GameObject.Find("UI").GetComponent<UI>().GetSubjectID();
		string path = Application.dataPath + "/" + subjectID;

		CreateDirectory(path);

		Thread thread = new Thread(() =>
		{
			StringBuilder sb = new StringBuilder();

			// To let Excel know
			sb.Append("SEP=").Append(seperator).Append("\n");
			sb.Append(data[0].GetCSVHeader(seperator));
			foreach (var ecgData in data)
			{
				sb.Append(ecgData.ToCSV(seperator)).Append("\n");
			}

			using (var writer = new StreamWriter(path + "/ECG_" + data[0].GetTimestamp() + ".csv", false))
			{
				writer.Write(sb.ToString());
			}

			Debug.Log($"Saved {data.Count} ECG entries.");
		});
		thread.Start();
	}

	public void SaveGSRData(List<GSRData> data)
	{
		if (data == null || data.Count == 0) return;

		string subjectID = GameObject.Find("UI").GetComponent<UI>().GetSubjectID();
		string path = Application.dataPath + "/" + subjectID;

		CreateDirectory(path);

		Thread thread = new Thread(() =>
		{
			StringBuilder sb = new StringBuilder();

			// To let Excel know
			sb.Append("SEP=").Append(seperator).Append("\n");
			sb.Append(data[0].GetCSVHeader(seperator));
			foreach (var gsrData in data)
			{
				sb.Append(gsrData.ToCSV(seperator)).Append("\n");
			}

			using (var writer = new StreamWriter(path + "/GSR_" + data[0].GetTimestamp() + ".csv", false))
			{
				writer.Write(sb.ToString());
			}

			Debug.Log($"Saved {data.Count} GSR entries.");
		});
		thread.Start();
	}

	public void SaveLabels(Label label)
	{
		if (label == null) return;

		StringBuilder sb = new StringBuilder();

		string subjectID = GameObject.Find("UI").GetComponent<UI>().GetSubjectID();
		string path = Application.dataPath + "/" + subjectID;

		CreateDirectory(path);

		string fileName = path + "/labels.csv";
		if (!File.Exists(fileName))
		{
			// Create a header
			sb.Append("SEP=").Append(seperator).Append("\n");
			sb.Append(label.GetCSVHeader(seperator));
		}

		// Append
		sb.Append(label.ToCSV(seperator)).Append("\n");

		using (var writer = new StreamWriter(fileName, true))
		{
			writer.Write(sb.ToString());
			writer.Flush();
		}

		//Debug.Log($"Saved: {label.GetTimestamp()} ; {label.GetLabel()}");
	}

	private void CreateDirectory(string path)
	{
		bool exists = Directory.Exists(path);
		if (!exists)
			Directory.CreateDirectory(path);
	}
}
