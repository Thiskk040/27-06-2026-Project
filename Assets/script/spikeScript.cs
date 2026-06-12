using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spikeScript : MonoBehaviour
{
    public spikeGenerator spikeGenerator;
  
    void Update()
    {
        transform.Translate(Vector2.left * spikeGenerator.currentSpeed * Time.deltaTime);      
    }
        
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("nextline"))
        {
            spikeGenerator.GenerateNextSpikeWithGap();
        }
        if (collision.gameObject.CompareTag("finishline"))
        {
            Destroy(this.gameObject);
        }
    }
}
