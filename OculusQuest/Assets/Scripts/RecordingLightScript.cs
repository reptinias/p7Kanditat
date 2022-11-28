using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordingLightScript : MonoBehaviour
{
    public Light light;
    public int handIndex;
    private OVRHand trackedHand;
    private int difHand;
    private NewReadInputs inputDevices;
    private int collisionAmount = 0;

    public bool pressed = false;

    // Start is called before the first frame update
    void Start()
    {
        if (handIndex == 0)
        {
            difHand = 1;
        }
        else
        {
            difHand = 0;
        }

        inputDevices = GameObject.Find("ReadInputs").GetComponent<NewReadInputs>();
        trackedHand = inputDevices.trackedHands[difHand];
        light.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name.Contains("_CapsuleCollider"))
        {
            if (collider.transform.IsChildOf(trackedHand.transform))
            {
                if (collisionAmount == 0)
                {
                    pressed = true;
                    Debug.Log("Pressed");
                    if (light.enabled == false)
                    {
                        light.enabled = true;
                    }
                    else if(light.enabled == true)
                    {
                        light.enabled = false;
                    }
                }
                collisionAmount++;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Contains("_CapsuleCollider"))
        {
            if (other.transform.IsChildOf(trackedHand.transform))
            {
                collisionAmount--;
            }
        }
    }
}
