using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ActionTriggerScript : MonoBehaviour
{
    private NewReadInputs readInputs;
    private GestureRecognition[] gestureRecognition;
    private AnimationRecorder animationRecorder;
    private AnimationPlayer animationPlayer;
    private TeleportationScript teleport;
    private Selection selector;
    public string[] currentGestures = {" ", " "};
    public string[] prevGestures = {" ", " "};
    private OVRSkeleton[] m_hands;
    private OVRHand[] trackedHands;
    private bool transRot = false;
    private bool contradiction = false;
    public  GameObject[] recordingLight;
    
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
        animationPlayer = GameObject.Find("Animation Master").GetComponent<AnimationPlayer>();
        teleport = GameObject.FindObjectOfType<TeleportationScript>();
        selector = GameObject.FindObjectOfType<Selection>();
        recordingLight[0].SetActive(false);
        recordingLight[1].SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        bool shouldTransRot = false;
        int handIndexTransRot = -1;

        string[] tempGesture = {gestureRecognition[0].getGesture()[0], gestureRecognition[1].getGesture()[0]};
        
        if (tempGesture[0] == "thumb up" && tempGesture[1] == "open hand"     ||
            tempGesture[1] == "thumb up" && tempGesture[0] == "open hand"     ||
            tempGesture[0] == "1-finger point" && tempGesture[1] == "ok hand" ||
            tempGesture[1] == "1-finger point" && tempGesture[0] == "ok hand"   )
        {
            contradiction = true;
        }
        else
        {
            contradiction = false;
        }
        
        if (tempGesture[0] != tempGesture[1])
        {
            for (int i = 0; i < 2; i++)
            {
                if (!trackedHands[i].IsTracked)
                {
                    continue;
                }

                if (!contradiction)
                {
                    // vvv Fix start stop contradiction vvv
                    // vvv plus accidental gesture      vvv
                    if (tempGesture[i] == "thumb up" && selector.allowRecording)
                    {
                        animationRecorder.StartRecording();
                        recordingLight[i].SetActive(true);
                    }
                    if (tempGesture[i] == "open hand")
                    {
                        animationRecorder.StopRecording();
                        recordingLight[i].SetActive(false);
                    }
                    if (tempGesture[i] == "1-finger point")
                    {
                        selector.SelectObject(i);
                    }
                    if (tempGesture[i] == "ok hand")
                    {
                        selector.DeselectObject();
                    }
                }

                if (tempGesture[i] == "closed hand")
                {
                    shouldTransRot = true;
                    handIndexTransRot = i;
                    //selector.MoveAndRotate(i, transRot);
                }
                
                /*if (Input.GetMouseButtonDown(0))
                {
                    animationPlayer.playRecording();
                }*/

                if (tempGesture[i] == "pistol")
                {
                    teleport.Indicator(i);
                }

                if (prevGestures[i] == "pistol" && tempGesture[i] == "open hand")
                {
                    teleport.Teleportation(i);
                }
            }

            if (transRot != shouldTransRot)
            {
                transRot = shouldTransRot;
                if (transRot)
                    selector.MoveAndRotate(handIndexTransRot);
                else
                    selector.StopMoveAndRotate();
            }
        }
        prevGestures = tempGesture;
    }

    public void ResetTransRotation()
    {
        transRot = false;
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
}
