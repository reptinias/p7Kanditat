using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandPointer : MonoBehaviour
{
    public LineRenderer line;
    
    public float maxLineLength = 1.0f;

    public bool toggle;

    public ReadInputDevices InputDevices;
    private string predString;

    private string name;
    private OVRSkeleton hand;

    private int index;

    // Start is called before the first frame update
    void Start()
    {
        Vector3[] startLinePositions = new Vector3[2] { Vector3.zero, Vector3.zero, };
        line.SetPositions(startLinePositions);
        name = gameObject.name;
        if (name == "LeftHandAnchor")
        {
            hand = InputDevices.m_hands[0];
            index = 0;
        }
        else
        {
            hand = InputDevices.m_hands[1];
            index = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        predString = InputDevices.predString[index];
        if (predString == "1-finger point")
        {
            toggle = true;
            line.enabled = true;
        }
        else
        {
            line.enabled = false;
            toggle = false;
        }

        //Debug.Log(InputDevices.m_hands[1].Bones.ElementAt(20).Transform.position - InputDevices.m_hands[1].Bones.ElementAt(8).Transform.position);
        if (toggle)
        {
            Telekenesis(hand.Bones.ElementAt(20).Transform.position,hand.Bones.ElementAt(20).Transform.position 
                                                                    - hand.Bones.ElementAt(8).Transform.position, maxLineLength);
        }
    }

    private void Telekenesis(Vector3 transformPosition, Vector3 transformForward, float length)
    {
        RaycastHit hit;
        Ray ray = new Ray(transformPosition, transformForward);
        Vector3 endPosition = transformPosition + (length * transformForward);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.CompareTag("AnimateAble"))
            {
                Collider hitObject = hit.transform.gameObject.GetComponent<Collider>();
                endPosition = hitObject.bounds.center;
            }
        }
        line.SetPositions(new Vector3[2]{transformPosition, endPosition});
    }
}
