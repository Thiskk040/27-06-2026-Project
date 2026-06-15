using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SimpleGraphUI : MonoBehaviour
{
    [Header("ตั้งค่า UI กราฟ")]
    public RectTransform graphContainer;
    public GameObject dotPrefab;
    public GameObject linePrefab; // [เพิ่มใหม่] Prefab สำหรับเส้น

    [Header("ตั้งค่าขอบเขตเซนเซอร์")]
    public float maxSoundExpected = 1000f;

    [Header("ตัวอักษรบอกค่า (Text)")]
    public Text maxTimeText;  // [เพิ่มใหม่] ตัวหนังสือบอกเวลาขวาสุดแกน X
    public Text maxSoundText; // [เพิ่มใหม่] ตัวหนังสือบอกค่าสูงสุดแกน Y

    public void DrawGraph()
    {
        List<GameDataLogger.DataPoint> data = GameDataLogger.instance.sessionData;
        if (data.Count == 0) return;

        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;
        float maxTime = data[data.Count - 1].time;

        // อัปเดตตัวหนังสือบอกข้อมูลบน UI
        if (maxTimeText != null) maxTimeText.text = maxTime.ToString("F1") + " s";
        if (maxSoundText != null) maxSoundText.text = maxSoundExpected.ToString();

        Vector2 lastPos = Vector2.zero; // เก็บตำแหน่งจุดก่อนหน้าเพื่อไว้ลากเส้น

        for (int i = 0; i < data.Count; i++)
        {
            var dp = data[i];
            float xPos = (dp.time / maxTime) * graphWidth;
            float yPos = (dp.sound / maxSoundExpected) * graphHeight;
            Vector2 currentPos = new Vector2(xPos, yPos);

            // 1. วาดเส้นเชื่อมจุด (เริ่มวาดตั้งแต่จุดที่ 2 เป็นต้นไป)
            if (i > 0)
            {
                DrawLine(lastPos, currentPos);
            }

            // 2. วาดจุดแดงทับอีกที
            GameObject dot = Instantiate(dotPrefab, graphContainer);
            RectTransform dotRect = dot.GetComponent<RectTransform>();
            dotRect.anchoredPosition = currentPos;

            lastPos = currentPos; // จำตำแหน่งจุดนี้ไว้เพื่อลากเส้นเชื่อมในรอบต่อไป
        }
    }

    // ฟังก์ชันคำนวณระยะทางและองศาเพื่อตีเส้น
    void DrawLine(Vector2 posA, Vector2 posB)
    {
        GameObject line = Instantiate(linePrefab, graphContainer);
        RectTransform lineRect = line.GetComponent<RectTransform>();

        lineRect.anchoredPosition = posA; // เอาปลายเส้นไปวางที่จุด A

        Vector2 dir = posB - posA;
        float distance = dir.magnitude;

        lineRect.sizeDelta = new Vector2(distance, 2f); // ความยาวเส้นตามระยะห่าง, ความหนาเส้น = 2

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        lineRect.localEulerAngles = new Vector3(0, 0, angle); // หมุนเส้นชี้ไปหาจุด B
    }
}