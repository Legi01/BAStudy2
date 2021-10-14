using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using TeslasuitAPI;
using System.IO;

public class FileManager
{

    public static FileManager _instance = null;

    private SurrogateSelector _surrogateSelector;
    private const string seperator = ";";

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
    }

    public void SaveToCSV(List<MocapData> data)
    {
        if (data == null || data.Count == 0) return;

        StringBuilder sb = new StringBuilder();

        // To let Excel know
        sb.Append("SEP=").Append(seperator).Append("\n");
        sb.Append(data[0].GetCSVHeader(seperator));
        foreach (var mocapData in data)
        {
            sb.Append(mocapData.ToCSV(seperator)).Append("\n");
        }

        string path = Application.dataPath + "/MoCap_" + data[0].GetTimestamp() + ".csv";

        using (var writer = new StreamWriter(path, false))
        {
            writer.Write(sb.ToString());
        }

        Debug.Log($"Saved {data.Count} mocap entries.");
    }

    public void Save(List<MocapData> data)
    {
        if (data == null) return;

        BinaryFormatter formatter = new BinaryFormatter
        {
            SurrogateSelector = _surrogateSelector
        };
        string path = Application.dataPath + "/MoCap_" + data[0].GetTimestamp() + ".mocap";
        FileStream stream = new FileStream(path, FileMode.Create);

        // Serialize to a file
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public void SaveToCSV(List<ECGData> data)
    {
        if (data == null || data.Count == 0) return;

        StringBuilder sb = new StringBuilder();

        // To let Excel know
        sb.Append("SEP=").Append(seperator).Append("\n");
        sb.Append(data[0].GetCSVHeader(seperator));
        foreach (var ecgData in data)
        {
            sb.Append(ecgData.ToCSV(seperator)).Append("\n");
        }

        string path = Application.dataPath + "/ECG_" + data[0].GetTimestamp() + ".csv";

        using (var writer = new StreamWriter(path, false))
        {
            writer.Write(sb.ToString());
        }

        Debug.Log($"Saved {data.Count} ECG entries.");
    }

    public void SaveToCSV(List<GSRData> data)
    {
        if (data == null || data.Count == 0) return;

        StringBuilder sb = new StringBuilder();

        // To let Excel know
        sb.Append("SEP=").Append(seperator).Append("\n");
        sb.Append(data[0].GetCSVHeader(seperator));
        foreach (var gsrData in data)
        {
            sb.Append(gsrData.ToCSV(seperator)).Append("\n");
        }

        string path = Application.dataPath + "/GSR_" + data[0].GetTimestamp() + ".csv";

        using (var writer = new StreamWriter(path, false))
        {
            writer.Write(sb.ToString());
        }

        Debug.Log($"Saved {data.Count} GSR entries.");
    }

    public void SaveToCSV(Label label)
    {
        StringBuilder sb = new StringBuilder();
        
        string path = Application.dataPath + "/labels.csv";
        if (!File.Exists(path))
        {
            // Create a header
            sb.Append("SEP=").Append(seperator).Append("\n");
            sb.Append(label.GetCSVHeader(seperator));
        }

        // Append
        sb.Append(label.ToCSV(seperator)).Append("\n");

        using (var writer = new StreamWriter(path, true))
        {
            writer.Write(sb.ToString());
            writer.Flush();
        }

        //Debug.Log($"Saved: {label.GetTimestamp()} ; {label.GetLabel()}");
    }

    public List<MocapData> Load(string filename)
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
}
