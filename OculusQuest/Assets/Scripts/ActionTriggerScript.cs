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
    public bool transRot = false;
    private bool contradiction = false;
    public  GameObject[] recordingLight;
    public  RecordingLightScript[] recordingLightScripts;
    public int gestureShift = 0;
    public int animationPlay = 0;
    
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
        recordingLightScripts = new RecordingLightScript[]
        {
            GameObject.Find("OVRCameraRigCustom/TrackingSpace/LeftHandAnchor/LeftOVRHandPrefab/RecordingLight").GetComponent<RecordingLightScript>(),
            GameObject.Find("OVRCameraRigCustom/TrackingSpace/RightHandAnchor/RightOVRHandPrefab/RecordingLight").GetComponent<RecordingLightScript>()
        };
        recordingLight[0].SetActive(false);
        recordingLight[1].SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        bool shouldTransRot = false;
        int handIndexTransRot = -1;

        string[] tempGesture = {gestureRecognition[0].getGesture()[0], gestureRecognition[1].getGesture()[0]};
        currentGestures = new string[]{gestureRecognition[0].getGesture()[0], gestureRecognition[1].getGesture()[0]};
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
                    if (recordingLightScripts[i].pressed && selector.allowRecording)
                    {
                        animationRecorder.StartRecording();
                    }
                    if (recordingLightScripts[i].pressed && !selector.allowRecording)
                    {
                        animationRecorder.StopRecording();
                    }
                    if (tempGesture[i] == "pointing hand")
                    {
                        selector.SelectObject(i);
                        if (selector.getSelectedObject() != null)
                        {
                            recordingLight[i].SetActive(true); 
                        }
                    }
                    if (tempGesture[i] == "ok hand")
                    {
                        selector.DeselectObject();
                        recordingLight[i].SetActive(false); 
                    }
                }

                if (tempGesture[i] == "closed hand")
                {
                    shouldTransRot = true;
                    handIndexTransRot = i;
                    //selector.MoveAndRotate(i, transRot);
                }
                
                if (tempGesture[i] == "thumb down" && tempGesture[i] != prevGestures[i])
                {
                    animationPlay = 1;
                    animationRecorder.PlayRecording();
                    //animationPlayer.playRecording();
                }
                else
                {
                    animationPlay = 0;
                }

                if (tempGesture[i] == "pointing hand")
                {
                    teleport.line.enabled = false;
                    teleport.Indicator(i);
                }
                else
                {
                    teleport.line.enabled = false;
                }

                if (prevGestures[i] == "pistol hand" && tempGesture[i] == "pointing hand")
                {
                    teleport.Teleportation(i);
                }
            }

            /*if (transRot != shouldTransRot)
            {
                transRot = shouldTransRot;
                if (transRot)
                    selector.MoveAndRotate(handIndexTransRot);
                else
                    selector.StopMoveAndRotate();
            }*/
        }

        if (prevGestures[0] != currentGestures[0] && prevGestures[1] != currentGestures[1])
        {
            gestureShift = 1;
        }
        else
        {
            gestureShift = 0;
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
