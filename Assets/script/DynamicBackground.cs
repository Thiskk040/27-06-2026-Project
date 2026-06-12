using UnityEngine;

public class DynamicBackground : MonoBehaviour
{
    public Camera mainCamera;

    [Header("ตั้งค่าอุณหภูมิและสี")]
    public float hotThreshold = 30.0f; // องศาเซลเซียส (ถ้าเกินนี้ถือว่าร้อน)

    // โทนสีสามารถไปจิ้มเลือกเองได้ในหน้า Inspector
    public Color coldColor = new Color(0.6f, 0.8f, 1f); // สีฟ้าโทนเย็น (ด่านหิมะ)
    public Color hotColor = new Color(1f, 0.6f, 0.4f);  // สีส้มโทนร้อน (ด่านภูเขาไฟ)

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        // เปลี่ยนโหมดเคลียร์กล้องเป็น Solid Color เพื่อให้เปลี่ยนสีได้
        mainCamera.clearFlags = CameraClearFlags.SolidColor;
    }

    void Update()
    {
        if (ArduinoController.instance != null)
        {
            float currentTemp = ArduinoController.instance.temperature;

            // ป้องกันบั๊กค่า 0 ตอนเปิดเกมช่วงแรกที่เซนเซอร์ยังส่งข้อมูลไม่ทัน
            if (currentTemp > 0)
            {
                // ใช้ Color.Lerp เพื่อให้สีค่อยๆ เฟดเปลี่ยนอย่างนุ่มนวล
                if (currentTemp >= hotThreshold)
                {
                    mainCamera.backgroundColor = Color.Lerp(mainCamera.backgroundColor, hotColor, Time.deltaTime);
                }
                else
                {
                    mainCamera.backgroundColor = Color.Lerp(mainCamera.backgroundColor, coldColor, Time.deltaTime);
                }
            }
        }
    }
}