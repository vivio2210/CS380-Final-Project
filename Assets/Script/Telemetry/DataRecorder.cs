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
        timeStamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

        DontDestroyOnLoad(this);
    }
    
    private void Start()
    {
        //NewSession();
        //Invoke(nameof(EndSession),3f);
    }

    private bool newFile(ref StreamWriter writer, string path)
    {
        if (!File.Exists(path))
        {
            writer = new StreamWriter(path);
            return true;
        }
        else
        {
            writer = new StreamWriter(path, true);
        }

        return false;
    }

    float timer = 0;
    private void Update()
    {
        if (isRecording)
        {
            timer += Time.unscaledDeltaTime;
        }
        else if (timer > 0)
        {
            timer = 0;
        }
    }

    public void NewSession()
    {
        if (isRecording)
        {
            EndSession(false);
        }
        
        isRecording = true;
        
        string directoryPath = Path.Combine(Application.dataPath,$"Telemetry/GameResult/{timeStamp}");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        
        string sessionResultPath = Path.Combine(directoryPath, $"Result.csv");
        //string sessionTimestampPath = Path.Combine(directoryPath, $"Timestamp.csv");

        if (newFile(ref SessionResultFile, sessionResultPath))
        {
            SessionResultFile.WriteLineAsync("Time,GotCaught,Scene,PlayerMode,EnemyMode,EnemyCaptureMode,EnemyVisionMode");
        }
        //newFile(ref SessionTimestampFile, sessionTimestampPath);

    }
    
    public void EndSession(bool isWin = false)
    {
        if (!isRecording) return;
        
        isRecording = false;

        //covnert to mm:ss:ms
        string time = $"{Mathf.FloorToInt(timer % 60)}.{Mathf.FloorToInt((timer % 1) * 1000)}";

        var gm = GameManager.Instance;
        SessionResultFile.WriteLineAsync($"{time},{isWin},{gm.Scene},{gm.PlayerMode},{gm.EnemyMode},{gm.EnemyCaptureMode},{gm.EnemyVisionMode}");
        
        SessionResultFile.Close();
        SessionResultFile = null;

        gm.Reload();

    }
    
}
