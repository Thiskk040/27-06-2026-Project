using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public float jumpforce;
    float score;
    [SerializeField] bool isGrounded = false;
    bool isAlive = true;

    public Text scoreTxt;
    Rigidbody2D RB;

    [Header("SoundSensor")]
    public int jumpSoundThreshold; // ความดังเสียงที่ต้องใช้กระโดด (ปรับเพิ่มลดได้ตามความไวของเซนเซอร์)

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        score = 0;
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

        if (collision.gameObject.CompareTag("spike"))
        {
            isAlive = false;
            Time.timeScale = 0;
        }
    }
}