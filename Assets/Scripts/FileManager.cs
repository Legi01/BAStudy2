using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using TeslasuitAPI;
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
		_surrogateSelector = new SurrogateSelector();
		_surrogateSelector.AddSurrogate(typeof(TSMocapData), new StreamingContext(StreamingContextStates.All),
			new TSMocapDataSurrogate());
		_surrogateSelector.AddSurrogate(typeof(Quat4f), new StreamingContext(StreamingContextStates.All),
			new Quat4fSurrogate());
		_surrogateSelector.AddSurrogate(typeof(Vector3s), new StreamingContext(StreamingContextStates.All),
			new Vector3sSurrogate());
		_surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All),
			new Vector3Surrogate());

		savingData = false;
	}

	public void SaveMoCapData(List<MocapData> data)
	{
		if (data == null || data.Count == 0) return;

		string subjectID = GameObject.Find("UI").GetComponent<UI>().GetSubjectID();
		string path = Application.dataPath + "/" + subjectID;

		CreateDirectory(path);

		savingData = true;

		Thread thread = new Thread(() =>
		{
			// Save to a CSV
			StringBuilder sb = new StringBuilder();

			// To let Excel know
			sb.Append("SEP=").Append(seperator).Append("\n");
			sb.Append(data[0].GetCSVHeader(seperator));
			foreach (var mocapData in data)
			{
				sb.Append(mocapData.ToCSV(seperator)).Append("\n");
			}

			using (var writer = new StreamWriter(path + "/MoCap_" + data[0].GetTimestamp() + ".csv", false))
			{
				writer.Write(sb.ToString());
			}

			// Serialize to a file
			BinaryFormatter formatter = new BinaryFormatter
			{
				SurrogateSelector = _surrogateSelector
			};

			FileStream stream = new FileStream(path + "/MoCap_" + data[0].GetTimestamp() + ".mocap", FileMode.Create);
			formatter.Serialize(stream, data);
			stream.Close();

			savingData = false;
			Debug.Log($"Saved {data.Count} MoCap entries.");
		});
		thread.Start();
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

	public List<MocapData> LoadMoCapData(string filename)
	{
		string path = Application.dataPath + "/" + filename + ".mocap";
		if (File.Exists(path))
		{
			BinaryFormatter formatter = new BinaryFormatter()
			{
				SurrogateSelector = _surrogateSelector
			};
			FileStream stream = new FileStream(path, FileMode.Open);

			List<MocapData> data = (List<MocapData>)formatter.Deserialize(stream);
			stream.Close();

			return data;
		}
		else
		{
			Debug.Log("Mocap file not found: " + path);
			return null;
		}
	}

	private void CreateDirectory(string path)
	{
		bool exists = Directory.Exists(path);
		if (!exists)
			Directory.CreateDirectory(path);
	}
}
