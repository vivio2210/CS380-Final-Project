//Pada Cherdchoothai

using System;
using System.IO;
using UnityEngine;

public class DataRecorder : MonoBehaviour
{
    public static DataRecorder Instance;
    private bool isRecording = false;
    private string timeStamp = "";

    StreamWriter SessionResultFile = null;
    StreamWriter SessionTimestampFile = null;
    // Start is called before the first frame update

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }
    
    private void Start()
    {
        //NewSession();
        //Invoke(nameof(EndSession),3f);
    }

    private void newFile(ref StreamWriter writer, string path)
    {
        if (!File.Exists(path))
        {
            writer = new StreamWriter(path);
            string header = "";
        
            // foreach (var d in data)
            // {
            //     header += $"{d.Name},";
            // }
            
            writer.WriteLineAsync(header);
        }
        else
        {
            writer = new StreamWriter(path, true);
        }
    }

    public void NewSession()
    {
        if (isRecording)
        {
            EndSession();
        }
        
        isRecording = true;
        
        timeStamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        string directoryPath = Path.Combine(Application.dataPath,$"Telemetry/GameResult/{timeStamp}");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        
        string sessionResultPath = Path.Combine(directoryPath, $"Result.csv");
        string sessionTimestampPath = Path.Combine(directoryPath, $"Timestamp.csv");
        
        newFile(ref SessionResultFile, sessionResultPath);
        newFile(ref SessionTimestampFile, sessionTimestampPath);
    }
    
    public void EndSession()
    {
        if (!isRecording) return;
        
        isRecording = false;
        
        timeStamp = "";
        
        SessionResultFile.Close();
        SessionResultFile = null;
        
        SessionTimestampFile.Close();
        SessionTimestampFile = null;
    }
    
}
