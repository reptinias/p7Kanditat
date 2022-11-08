using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the Cube with the Oculus Hands base functionalities
/// When the Left Index touches the cube, it becomes blue;
/// When the Right Index touches the cube, it become green;
/// When the Left hand pinches the middle finger, it becomes red;
/// </summary>
[RequireComponent(typeof(Collider))]
public class BoneToRigMapping : MonoBehaviour
{
    /// <summary>
    /// Renderer of this cube
    /// </summary>
    private Renderer m_renderer;

    private OVRSkeleton[] m_hands;
    private OVRHand[] trackedHands;

    /// <summary>
    /// True if an index tip is inside the cube, false otherwise.
    /// First item is left hand, second item is right hand
    /// </summary>
    private bool[] m_isIndexStaying;

    private OVRBone curFingertipBone;
    OVRPlugin.BoneId curFingertip;
    public TMP_Text testText;

    bool curMapping = false;

    Vector3 initialFingertipPos;
    Quaternion initialFingertipRotation;
    Vector3 initialPos;
    Quaternion initialRotation;

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        //StartMapping();
        m_renderer = GetComponent<Renderer>();
        trackedHands = new OVRHand[]
        {
            GameObject.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor/LeftOVRHandPrefab").GetComponent<OVRHand>(),
            GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor/RightOVRHandPrefab").GetComponent<OVRHand>()
        };

        m_hands = new OVRSkeleton[]
{
            GameObject.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor/LeftOVRHandPrefab").GetComponent<OVRSkeleton>(),
            GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor/RightOVRHandPrefab").GetComponent<OVRSkeleton>()
};

        m_isIndexStaying = new bool[2] { false, false };

        //we don't want the cube to move over collision, so let's just use a trigger
        GetComponent<Collider>().isTrigger = true;
    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        if (curMapping)
        {
            Vector3 differencePos = curFingertipBone.Transform.position - initialFingertipPos;//20 -30 = -10
            Quaternion differenceRot = Quaternion.Inverse(initialFingertipRotation) * curFingertipBone.Transform.rotation;//20 -30 = -10

            transform.position = initialPos + differencePos;
            transform.rotation = initialRotation * differenceRot;
        }
        /*
        //check for middle finger pinch on the left hand, and make che cube red in this case
        if (m_hands[0].GetFingerIsPinching(OVRHand.HandFinger.Middle))
            m_renderer.material.color = Color.red;
        //if no pinch, and the cube was red, make it white again
        else if (m_renderer.material.color == Color.red)
            m_renderer.material.color = Color.white;*/
    }

    /// <summary>
    /// Trigger enter.
    /// Notice that this gameobject must have a trigger collider
    /// </summary>
    /// <param name="collider">Collider of interest</param>
    private void OnTriggerEnter(Collider collider)
    {
        print("trigger");
        if (!curMapping)
        {
            print("cur Not mapping");
            //get hand associated with trigger
            int handIdx = GetIndexFingerHandId(collider);
            print(handIdx);
            //if there is an associated hand, it means that an index of one of two hands is entering the cube
            //change the color of the cube accordingly (blue for left hand, green for right one)
            if (handIdx != -1)
            {
                m_renderer.material.color = handIdx == 0 ? m_renderer.material.color = Color.blue : m_renderer.material.color = Color.green;
                m_isIndexStaying[handIdx] = true;
            }
        }
    }

    /// <summary>
    /// Trigger Exit.
    /// Notice that this gameobject must have a trigger collider
    /// </summary>
    /// <param name="collider">Collider of interest</param>
    private void OnTriggerExit(Collider collider)
    {
        if (!curMapping)
        {
            //get hand associated with trigger
            int handIdx = GetIndexFingerHandId(collider);

            //if there is an associated hand, it means that an index of one of two hands is levaing the cube,
            //so set the color of the cube back to white, or to the one of the other hand, if it is in
            if (handIdx != -1)
            {
                m_isIndexStaying[handIdx] = false;
                m_renderer.material.color = m_isIndexStaying[0] ? m_renderer.material.color = Color.blue :
                                            (m_isIndexStaying[1] ? m_renderer.material.color = Color.green : Color.white);
            }
        }
    }

    /// <summary>
    /// Gets the hand id associated with the index finger of the collider passed as parameter, if any
    /// </summary>
    /// <param name="collider">Collider of interest</param>
    /// <returns>0 if the collider represents the finger tip of left hand, 1 if it is the one of right hand, -1 if it is not an index fingertip</returns>
    private int GetIndexFingerHandId(Collider collider)
    {
        print("GetIndexFingerHandId");
        //Checking Oculus code, it is possible to see that physics capsules gameobjects always end with _CapsuleCollider
        if (collider.gameObject.name.Contains("_CapsuleCollider"))
        {
            //get the name of the bone from the name of the gameobject, and convert it to an enum value
            string boneName = collider.gameObject.name.Substring(0, collider.gameObject.name.Length - 16);
            OVRPlugin.BoneId boneId = (OVRPlugin.BoneId)Enum.Parse(typeof(OVRPlugin.BoneId), boneName);

            OVRPlugin.BoneId[] fingerTips = { OVRPlugin.BoneId.Hand_Thumb3, OVRPlugin.BoneId.Hand_Index3, OVRPlugin.BoneId.Hand_Middle3, OVRPlugin.BoneId.Hand_Ring3, OVRPlugin.BoneId.Hand_Pinky3 };
            int[] fingerTipsIndex = { 5, 8, 11, 14, 18};

            int handIndex = -1;
            if (collider.transform.IsChildOf(trackedHands[0].transform))
            {
                handIndex = 0;
            }
            else if (collider.transform.IsChildOf(trackedHands[1].transform))
            {
                handIndex = 1;
            }

            string testTextString = "";
            //if it is the tip of the Index
            for (int i = 0; i < fingerTips.Length; i++) //OVRPlugin.BoneId fingertip in fingerTips)
            {
                print("index: " + i);
                if (boneId == fingerTips[i])
                {

                    print("DIng DING DING");
                    if (handIndex >= 0)
                    {
                        if (curFingertip != fingerTips[i])
                        {
                            OVRBone fingerTipBone = m_hands[handIndex].Bones[fingerTipsIndex[i]];
                            if (collider.bounds.Contains(fingerTipBone.Transform.position))
                            {
                                print("Not the same as last time");
                                curFingertip = fingerTips[i];
                                testTextString += i.ToString() + " ";
                                curFingertipBone = fingerTipBone;
                            }
                        }
                    }
                }
            }
            if (testTextString != "")
                testText.text = testTextString;

            return handIndex;
        }
        return -1;

    }

    public void StartMapping()
    {
        curMapping = true;
        initialFingertipPos = curFingertipBone.Transform.position;
        initialFingertipRotation = curFingertipBone.Transform.rotation;
        initialPos = transform.position;
        initialRotation = transform.rotation;
    }

    public void StopMapping()
    {
        curMapping = false;
    }

}