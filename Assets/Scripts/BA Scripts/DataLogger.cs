using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class DataLogger
{
    private List<CubeData> dataEntries = new List<CubeData>();
    private string filePath;
    private string subjectID;
    private string condition;

    public DataLogger(string filename, string subjectID, string condition)
    {
        this.subjectID = subjectID;
        this.condition = condition;

        // Create directory for subject if it doesn't exist
        string subjectFolderPath = Application.dataPath + "/Data/" + subjectID;
        CreateDirectory(subjectFolderPath);
        // Append condition to the filename
        string fileNameWithCondition = $"{subjectID}_{condition}.csv"; // Example: "data_condition.csv"
      
        // Combine path for the CSV file
        filePath = Path.Combine(subjectFolderPath, fileNameWithCondition);

        // Create or overwrite the file and add the header
        using (var writer = new StreamWriter(filePath, false))
        {
            writer.WriteLine(CubeData.GetCSVHeader(";"));
        }
    }

    public void LogData(Vector3 redPos, Quaternion redRot, Vector3 greenPos, Quaternion greenRot)
    {
        var data = new CubeData(subjectID, condition, redPos, redRot, greenPos, greenRot);
        dataEntries.Add(data);
    }

    public void SaveData()
    {
        using (var writer = new StreamWriter(filePath, true))
        {
            foreach (var entry in dataEntries)
            {
                writer.WriteLine(entry.ToCSV(";"));
            }
        }
        dataEntries.Clear();
    }

    private void CreateDirectory(string path)
    {
        bool exists = Directory.Exists(path);
        if (!exists)
            Directory.CreateDirectory(path);
    }
}

