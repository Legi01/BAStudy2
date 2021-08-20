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
        //_surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All),
        //new Vector3Surrogate());
    }

    public void saveToCSV(List<SuitData> data)
    {
        StringBuilder sb = new StringBuilder();

        // To let Excel know
        sb.Append("SEP=").Append(seperator).Append("\n");
        sb.Append(data[0].GetCsvHeader(seperator));
        foreach (var suitData in data)
        {
            Debug.Log(suitData);
            sb.Append(suitData.ToCSV(seperator)).Append("\n");
        }

        string path = Application.dataPath + "/suitData.csv";

        using (var writer = new StreamWriter(path, false))
        {
            writer.Write(sb.ToString());
        }

        Debug.Log($"Saved {data.Count} entries.");
    }

    public List<SuitData> load()
    {
        string path = Application.dataPath + "/suitData.tsdat";
        FileStream file;

        if (File.Exists(path))
        {
            file = File.OpenRead(path);
        }
        else
        {
            Debug.LogError("Suit data file not found");
            return null;
        }

        BinaryFormatter bf = new BinaryFormatter
        {
            SurrogateSelector = _surrogateSelector
        };

        List<SuitData> data = (List<SuitData>)bf.Deserialize(file);
        file.Close();

        return data;
    }
}
