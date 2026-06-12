using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spikeGenerator : MonoBehaviour
{
    public GameObject spike;
    public float MinSpeed;
    public float MaxSpeed;
    public float currentSpeed;

    public float speedMultiplayer;

    void Awake()
    {
        currentSpeed = MinSpeed;
        generateSpike();
    }

    public void GenerateNextSpikeWithGap()
    {
        float randint = Random.Range(0.1f, 1.2f);
        Invoke("generateSpike", randint);
    }
    public void generateSpike()
    {
        GameObject SpikeIns = Instantiate(spike, transform.position, transform.rotation);
        SpikeIns.GetComponent<spikeScript>().spikeGenerator = this;
    }

    void Update()
    {
        if(currentSpeed < MaxSpeed)
        {
            currentSpeed += speedMultiplayer;
        }
    }
}
    

