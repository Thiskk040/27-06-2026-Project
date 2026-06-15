using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // เพิ่มไลบรารีนี้เพื่อใช้สำหรับเปลี่ยน Scene

public class PlayerScript : MonoBehaviour
{
    public float jumpforce;
    float score;
    [SerializeField] bool isGrounded = false;
    bool isAlive = true;

    public Text scoreTxt;
    Rigidbody2D RB;

    [Header("SoundSensor")]
    public int jumpSoundThreshold;

    [Header("Game Over UI")]
    public GameObject gameOverPanel; // ประกาศตัวแปรรับหน้าต่าง Game Over

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        score = 0;

        // คืนค่าเวลาให้เป็นปกติทุกครั้งที่เริ่มเกมใหม่
        Time.timeScale = 1;

        // ซ่อนหน้าต่าง Game Over ไว้ก่อนตอนเริ่มเกม
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    void Update()
    {
        bool isSoundLoud = false;
        if (ArduinoController.instance != null)
        {
            isSoundLoud = (ArduinoController.instance.soundLevel >= jumpSoundThreshold);
        }
        if (Input.GetKeyDown(KeyCode.Space) || isSoundLoud)
        {
            if (isGrounded == true)
            {
                RB.AddForce(Vector2.up * jumpforce);
                isGrounded = false;
            }
        }

        if (isAlive)
        {
            score += Time.deltaTime * 4;
            scoreTxt.text = "SCORE: " + score.ToString("F");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            if (isGrounded == false)
            {
                isGrounded = true;
            }
        }

        // เช็ค tag ว่าเป็น "spike" (หรือถ้าต้นไม้ใช้ tag อื่นให้เปลี่ยนตรงนี้ เช่น "tree")
        // เช็ค tag ว่าเป็น "spike"
        if (collision.gameObject.CompareTag("spike"))
        {
            isAlive = false;

            // 1. สั่งเซฟ CSV
            if (GameDataLogger.instance != null) GameDataLogger.instance.SaveToCSV();

            // 2. แสดงหน้าต่าง Game Over
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);

                // 3. สั่งวาดกราฟ
                SimpleGraphUI graphUI = gameOverPanel.GetComponent<SimpleGraphUI>();
                if (graphUI != null) graphUI.DrawGraph();
            }

            Time.timeScale = 0; // หยุดเกม
        }
    }

    // ฟังก์ชันนี้จะถูกเรียกใช้เมื่อกดปุ่ม Restart
    public void RestartGame()
    {
        // โหลด Scene ที่ 0 เพื่อเริ่มเกมใหม่
        SceneManager.LoadScene(0);
    }
}