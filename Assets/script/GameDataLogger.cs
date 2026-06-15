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
        dp.time = Time.timeSinceLevelLoad; // เวลาตั้งแต่เริ่มด่าน
        dp.sound = ArduinoController.instance.soundLevel;
        dp.temp = ArduinoController.instance.temperature;
        dp.hum = ArduinoController.instance.humidity;
        sessionData.Add(dp);
    }

    // ฟังก์ชันนี้จะถูกเรียกตอน Game Over เพื่อสร้างไฟล์ CSV
    public void SaveToCSV()
    {
        isRecording = false; // หยุดเก็บข้อมูล

        // ตั้งชื่อไฟล์ตามวันเวลา จะได้ไม่ทับกัน
        string filename = "GameData_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
        string filePath = Path.Combine(Application.dataPath, filename); // เซฟไว้ในโฟลเดอร์ Assets

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Time(s),SoundLevel,Temperature,Humidity"); // หัวตาราง
            foreach (DataPoint dp in sessionData)
            {
                writer.WriteLine($"{dp.time:F2},{dp.sound},{dp.temp:F2},{dp.hum:F2}");
            }
        }
        Debug.Log("เซฟไฟล์ CSV เรียบร้อยที่: " + filePath);
    }
}