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

    private SurrogateSelector _surrogateSelector;
    private string seperator = ";";

    public FileManager()
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

    public void SaveToCSV(List<SuitData> data)
    {
        StringBuilder sb = new StringBuilder();

        // To let Excel know
        sb.Append("SEP=").Append(seperator).Append("\n");
        sb.Append(data[0].GetCsvHeader(seperator));
        foreach (var suitData in data)
        {
            sb.Append(suitData.ToCSV(seperator)).Append("\n");
        }

        string path = Application.dataPath + "/mocap.csv";

        using (var writer = new StreamWriter(path, false))
        {
            writer.Write(sb.ToString());
        }

        Debug.Log($"Saved {data.Count} entries.");
    }

    public void Save(List<SuitData> data)
    {
        BinaryFormatter formatter = new BinaryFormatter
        {
            SurrogateSelector = _surrogateSelector
        };
        string path = Application.dataPath + "/MoCap.mocap";
        FileStream stream = new FileStream(path, FileMode.Create);

        // Serialize to a file
        formatter.Serialize(stream, data);
        stream.Close();
    }
    public List<SuitData> Load()
    {
        string path = Application.dataPath + "/MoCap.mocap";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter()
            {
                SurrogateSelector = _surrogateSelector
            };
            FileStream stream = new FileStream(path, FileMode.Open);

            List<SuitData> data = (List<SuitData>)formatter.Deserialize(stream);
            stream.Close();

            return data;
        }
        else
        {
            Debug.Log("Mocap FIle not Found");
            return null;
        }
    }
}
