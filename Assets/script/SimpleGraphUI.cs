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

        float maxSoundInSession = 0;
        foreach (var dp in data)
        {
            if (dp.sound > maxSoundInSession) maxSoundInSession = dp.sound;
        }
        // ถ้าเล่นเงียบมาก ให้ตั้งขั้นต่ำไว้ที่ 100 กันกราฟเบี้ยว
        if (maxSoundInSession < 100f) maxSoundInSession = 100f;
        // --------------------------------------------------

        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;
        float maxTime = data[data.Count - 1].time;

        // เปลี่ยนจากใช้ maxSoundExpected มาใช้ maxSoundInSession แทน
        if (maxTimeText != null) maxTimeText.text = maxTime.ToString("F1") + "Sec";
        if (maxSoundText != null) maxSoundText.text = "SoundMax: " + maxSoundInSession.ToString();

        Vector2 lastPos = Vector2.zero;

        for (int i = 0; i < data.Count; i++)
        {
            var dp = data[i];
            float xPos = (dp.time / maxTime) * graphWidth;

            // เปลี่ยนตรงนี้เป็นแบ่งด้วย maxSoundInSession
            float yPos = (dp.sound / maxSoundInSession) * graphHeight;

            Vector2 currentPos = new Vector2(xPos, yPos);

            if (i > 0) DrawLine(lastPos, currentPos);

            GameObject dot = Instantiate(dotPrefab, graphContainer);
            RectTransform dotRect = dot.GetComponent<RectTransform>();
            dotRect.anchoredPosition = currentPos;

            lastPos = currentPos;
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