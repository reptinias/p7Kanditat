using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTriggerScript : MonoBehaviour
{
    private NewReadInputs readInputs;
    private GestureRecognition[] gestureRecognition;
    private AnimationRecorder animationRecorder;
    private Selection selector;
    public string[] currentGestures = {" ", " "};
    public string[] prevGestures = {" ", " "};
    private OVRSkeleton[] m_hands;
    private OVRHand[] trackedHands;
    // Start is called before the first frame update
    void Start()
    {
        readInputs = GameObject.Find("ReadInputs").GetComponent<NewReadInputs>();
        m_hands = readInputs.m_hands;
        trackedHands = readInputs.trackedHands;
        gestureRecognition = new GestureRecognition[]
        {
            GameObject.Find("OVRCameraRigCustom/TrackingSpace/LeftHandAnchor").GetComponent<GestureRecognition>(),
            GameObject.Find("OVRCameraRigCustom/TrackingSpace/RightHandAnchor").GetComponent<GestureRecognition>()
        };
        animationRecorder = GameObject.Find("Animation Master").GetComponent<AnimationRecorder>();
        selector = GameObject.FindObjectOfType<Selection>();
    }

    // Update is called once per frame
    void Update()
    {
        /*for (int i = 0; i < 2; i++)
        {
            string[] newGesture = gestureRecognition[i].getGesture();
            if (newGesture[0] != "open hand" && currentGestures[i] != newGesture[0] && currentGestures[i] != newGesture[1])
            {
                opdateCurrentGesture();
                break;
            }
        }*/
        //startPointing();
        string[] tempGesture = {gestureRecognition[0].getGesture()[0], gestureRecognition[1].getGesture()[0]};
        
        for (int i = 0; i < 2; i++)
        {
            if (!trackedHands[i].IsTracked)
            {
                continue;
            }

            // vvv Fix start stop contradiction vvv
            // vvv plus accidental gesture      vvv
            /*if (tempGesture[i] == "thumb up")
            {
                animationRecorder.StartRecording();
            }

            if (tempGesture[i] == "open hand")
            {
                animationRecorder.StopRecording();
            }*/

            if (tempGesture[i] == "1-finger point")
            {
                selector.SelectObject(i);
            }

            if (tempGesture[i] == "ok hand")
            {
                selector.DeselectObject();
            }
        }
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
