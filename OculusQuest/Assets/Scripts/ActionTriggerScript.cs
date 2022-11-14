using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTriggerScript : MonoBehaviour
{
    private NewReadInputs readInputs;
    private GestureRecognition[] gestureRecognition;
    public string[] currentGestures = {" ", " "};
    public string[] prevGestures = {" ", " "};
    private OVRSkeleton[] m_hands;
    
    // Start is called before the first frame update
    void Start()
    {
        readInputs = GameObject.Find("ReadInputs").GetComponent<NewReadInputs>();
        m_hands = readInputs.m_hands;
        gestureRecognition = new GestureRecognition[]
        {
            GameObject.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor").GetComponent<GestureRecognition>(),
            GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor").GetComponent<GestureRecognition>()
        };
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 2; i++)
        {
            string[] newGesture = gestureRecognition[i].getGesture();
            Debug.Log(i);
            if (newGesture[0] != "open hand" && currentGestures[i] != newGesture[0] && currentGestures[i] != newGesture[1])
            {
                opdateCurrentGesture();
                break;
            }
        }
        //startPointing();
       
    }

    void opdateCurrentGesture()
    {
        for (int i = 0; i < 2; i++)
        {
            prevGestures[i] = currentGestures[i];
            
            string sGesture = gestureRecognition[i].getGesture()[0];
            string dGesture = gestureRecognition[i].getGesture()[1];
            if (dGesture != " ")
            {
                currentGestures[i] = dGesture;
            }
            else
            {
                currentGestures[i] = sGesture;
            }
            gestureRecognition[i].setGesture();
        }
        Debug.Log("Current Gestures: left " + currentGestures[0] + " right " + currentGestures[1]);
    }
    
    // Call this when ever an "action" has been perform, such as select or start recording
    void resetGestures()
    {
        currentGestures[0] = " ";
        currentGestures[1] = " ";
    }

    /*void startPointing()
    {
        for (int i  = 0; i < 2; i++)
        {
            if (gestureRecognition[i].GesturePrediction[0] == "1-finger point")
            {
                opdateCurrentGesture();
                break;
            }
        }
    }*/
}
