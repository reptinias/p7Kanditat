using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PointingRay : MonoBehaviour
{
    private ReadInputDevices InputDevices;
    public GameObject[] Cubelist = new GameObject[2];
    private LineRenderer _lineRenderer;

    private int index = 0;
    // Start is called before the first frame update
    void Start()
    {
        InputDevices = GameObject.Find("ReadInputs").GetComponent<ReadInputDevices>();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
        foreach (OVRSkeleton hand in InputDevices.m_hands)
        {
            Debug.Log(hand.GetCurrentNumBones());
            if (hand.GetCurrentNumBones() != 0)
            {
                Vector3 point = hand.Bones.ElementAt(6).Transform.position;
                Vector3 direction = hand.Bones.ElementAt(6).Transform.position - hand.Bones.ElementAt(20).Transform.position;
                Vector3[] test = new Vector3[2]{point, direction};
                _lineRenderer.SetPositions(test);
                //Debug.Log(direction);
                Debug.DrawRay(point,direction, Color.green);
            }

            index++;
        }
        index = 0;
    }
}
