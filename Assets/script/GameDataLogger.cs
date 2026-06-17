using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class GameDataLogger : MonoBehaviour
{
    public static GameDataLogger instance;

    // โครงสร้างข้อมูลที่จะเก็บในแต่ละวินาที
    [System.Serializable]
    public class DataPoint
    {
        public string timestamp;
        public float time;
        public int sound;
        public float temp;
        public float hum;
    }

    public List<DataPoint> sessionData = new List<DataPoint>();

    float timer = 0;
    public float recordInterval = 0.5f; // บันทึกข้อมูลทุกๆ 0.5 วินาที
    bool isRecording = true;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Update()
    {
        if (!isRecording || ArduinoController.instance == null) return;

        timer += Time.deltaTime;
        if (timer >= recordInterval)
        {
            RecordData();
            timer = 0;
        }
    }

    void RecordData()
    {
        DataPoint dp = new DataPoint();
        dp.timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        dp.time = Time.timeSinceLevelLoad; 
        dp.sound = ArduinoController.instance.soundLevel;
        dp.temp = ArduinoController.instance.temperature;
        dp.hum = ArduinoController.instance.humidity;
        sessionData.Add(dp);
    }

    // ฟังก์ชันนี้จะถูกเรียกตอน Game Over เพื่อสร้างไฟล์ CSV
    public void SaveToCSV()
    {
        isRecording = false; // หยุดเก็บข้อมูล
        string folderpath = Path.Combine(Application.dataPath, "CSV");
        string filename = "GameData_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv";

        if (!Directory.Exists(folderpath))
        {
            Directory.CreateDirectory(folderpath);
        }
        string filepath = Path.Combine(folderpath, filename);


        using (StreamWriter writer = new StreamWriter(filepath))
        {
            writer.WriteLine("Time(s),SoundLevel,Temperature,Humidity"); // หัวตาราง
            foreach (DataPoint dp in sessionData)
            {
                writer.WriteLine($"{dp.timestamp:F1},{dp.time:F2},{dp.sound},{dp.temp:F2},{dp.hum:F2}");
            }
        }
        Debug.Log("CSV savecomplete" + filepath);
    }
}