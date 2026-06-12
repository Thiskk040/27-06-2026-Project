using UnityEngine;
using System.IO.Ports;
using System;

public class ArduinoController : MonoBehaviour
{
    public static ArduinoController instance;

    [Header("Arduino Port Setup")]
    public string portName = "COM4";
    public int baudRate = 115200;
    SerialPort data_stream;

    [Header("Sensor parameter Real-time")]
    public int soundLevel;
    public float temperature;
    public float humidity;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        data_stream = new SerialPort(portName, baudRate);
        data_stream.ReadTimeout = 5; // ลดเวลา Timeout ให้เช็กไวขึ้นอีก
        try
        {
            data_stream.Open();
        }
        catch (Exception e)
        {
            Debug.LogWarning("Arduino Connected Error msg: " + e.Message);
        }
    }

    void Update()
    {
        if (data_stream.IsOpen)
        {
            string latestValue = null;

            try
            {
                while (data_stream.BytesToRead > 0)
                {
                    latestValue = data_stream.ReadLine();
                }
            }
            catch (TimeoutException) { }
            catch (Exception) { }

            // ถ้าได้ค่าล่าสุดมาแล้ว ค่อยนำมาแยกข้อมูล
            if (!string.IsNullOrEmpty(latestValue))
            {
                try
                {
                    string[] data = latestValue.Split(',');

                    if (data.Length == 3)
                    {
                        soundLevel = int.Parse(data[0]);
                        temperature = float.Parse(data[1]);
                        humidity = float.Parse(data[2]);
                        print(soundLevel);
                    }
                }
                catch (Exception) { }
            }
        }
    }

    void OnApplicationQuit()
    {
        if (data_stream != null && data_stream.IsOpen) data_stream.Close();
    }
}