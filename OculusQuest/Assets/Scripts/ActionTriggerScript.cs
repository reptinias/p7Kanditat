using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ActionTriggerScript : MonoBehaviour
{
    private int selectingHand = -1;
    private BoneMappingHandler boneMapper;
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
    private bool recording = false;
    private bool isMapping = false;
    
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
        boneMapper = GameObject.FindObjectOfType<BoneMappingHandler>().GetComponent<BoneMappingHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        bool shouldTransRot = false;
        int handIndexTransRot = -1;

        currentGestures = new string[]{gestureRecognition[0].getGesture()[0], gestureRecognition[1].getGesture()[0]};
        if (currentGestures[0] == "thumb up" && currentGestures[1] == "thumb down"    ||
            currentGestures[1] == "thumb up" && currentGestures[0] == "thumb down"    ||
            currentGestures[0] == "pointing hand" && currentGestures[1] == "ok hand" ||
            currentGestures[1] == "pointing hand" && currentGestures[0] == "ok hand" )
        {
            contradiction = true;
        }
        else
        {
            contradiction = false;
        }
        
        if (currentGestures[0] != currentGestures[1])
        {
            for (int i = 0; i < 2; i++)
            {
                // vvv Fix start stop contradiction vvv
                // vvv plus accidental gesture      vvv
                if (recordingLight[i].activeSelf)
                {
                    if (!animationRecorder.recording && recordingLightScripts[i].pressed && !recording)
                    {
                        animationRecorder.StartRecording();
                        recording = true;
                        boneMapper.StartMapping();
                    }
                    else if (animationRecorder.recording && !recordingLightScripts[i].pressed && recording)
                    {
                        print("STOP RECORDING?!?!??!? :)");
                        animationRecorder.StopRecording();
                        recording = false;
                        boneMapper.StopMapping();
                    }
                }

                // g??re s?? man kun kan selecte med en h??nd (BOOLS MIKAEL BOOLS)
                if (!animationRecorder.recording)
                {
                    if (!contradiction)
                    {
                        if (currentGestures[i] == "pointing hand" && selector.getSelectedObject() == null)
                        {
                            selectingHand = i;
                            selector.SelectObject(i);
                            if (selector.getSelectedObject() != null)
                            {
                                recordingLight[i].SetActive(true);
                            }
                        }

                        if (currentGestures[i] == "ok hand" && i != selectingHand)
                        {
                            selectingHand = -1;
                            selector.DeselectObject();
                            recordingLight[i].SetActive(false);
                        }

                        if (currentGestures[i] == "thumb up" && !animationRecorder.playingAnimation)
                        {
                            animationRecorder.PlayRecording();
                        }

                        if (currentGestures[i] == "thumb down" && animationRecorder.playingAnimation)
                        {
                            animationRecorder.StopPlayingClip();
                        }
                    }

                    if (currentGestures[i] == "closed hand" && selectingHand != i && selectingHand != -1 && !isMapping)
                    {
                        isMapping = true;
                        boneMapper.StartMapping();
                    }
                    if (currentGestures[i] != "closed hand" && isMapping && selectingHand != i)
                    {
                        isMapping = false;
                        boneMapper.StopMapping();
                    }

                    
                    if (animationRecorder.playingAnimation)
                    {
                        animationPlay = 1;
                    }
                    else if(!animationRecorder.playingAnimation)
                    {
                        animationPlay = 0;
                    }
                    
                    
                    /*
                    if (currentGestures[i] == "pointing hand" || currentGestures[i] == "pistol hand")
                    {
                        teleport.line.enabled = true;
                        teleport.Indicator(i);
                    }
                    else
                    {
                        teleport.line.enabled = false;
                    }

                    if (prevGestures[i] == "pistol hand" && currentGestures[i] == "pointing hand")
                    {
                        teleport.Teleportation(i);
                    }
                    */
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

        if (prevGestures[0] != currentGestures[0] || prevGestures[1] != currentGestures[1])
        {
            gestureShift = 1;
        }
        else
        {
            gestureShift = 0;
        }
        prevGestures = currentGestures;
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
