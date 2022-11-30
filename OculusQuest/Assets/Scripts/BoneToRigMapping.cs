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
    Vector3 initialFingertipPosLocal;
    Quaternion initialFingertipRotation;
    Vector3 initialPos;
    Quaternion initialRotation;

    List<int> finger_index = new List<int>();
    private NewReadInputs InputDevices;

    private Color[] fingerColor = { Color.black, Color.blue, Color.yellow, Color.red, Color.green };

    private Material[,] fingerMaterials = new Material[2, 5];
    private Material[] finger0Materials = new Material[5];
    private Material[] finger1Materials = new Material[5];

    private MeshRenderer meshRend;

    int curHandIndex = -1;

    int curFingerIndex = -1;

    BoneMappingHandler boneMappingHandler;
    ChangeHandMaterial changeHandMaterialScript;
    AnimationRecorder animRecorder;

    List<Transform> spherePoints;
    List<Transform> handPoints;
    float scale;

    private Vector3 initialHandPos;

    public void SetIndexes(int hand, int finger)
    {
        curHandIndex = hand;
        curFingerIndex = finger;
    }

    public void ResetFinger()
    {
        curFingertipBone = null;
        curHandIndex = -1;
        curFingerIndex = -1;
    }

    public int[] GetPreviousIndexes()
    {
        return new int[]{curHandIndex, curFingerIndex};
    }

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        GetComponent<MeshRenderer>().material.color = new Color(0.5f, 0.5f, 0.5f, .3f);
        changeHandMaterialScript = GameObject.FindObjectOfType<ChangeHandMaterial>();
        animRecorder = GameObject.FindObjectOfType<AnimationRecorder>();

        //changeHandMaterialScript.ChangeMaterial(0,curIndex, gameObject);
        //StartMapping();
        m_renderer = GetComponent<Renderer>();

        boneMappingHandler = GameObject.FindObjectOfType<BoneMappingHandler>();

        InputDevices = GameObject.Find("ReadInputs").GetComponent<NewReadInputs>();
        trackedHands = InputDevices.trackedHands;

        m_hands = InputDevices.m_hands;

        m_isIndexStaying = new bool[2] { false, false };

        //we don't want the cube to move over collision, so let's just use a trigger
        GetComponent<Collider>().isTrigger = true;

        for (int i = 0; i < trackedHands.Length; i++)
            for (int j = 0; j < 5; j++)
            {
                fingerMaterials[i, j] = trackedHands[i].GetComponent<SkinnedMeshRenderer>().materials[j];
                if (i == 0)
                    finger0Materials[j] = trackedHands[i].GetComponent<SkinnedMeshRenderer>().materials[j];
                else
                    finger1Materials[j] = trackedHands[i].GetComponent<SkinnedMeshRenderer>().materials[j];

            }

        meshRend = GetComponent<MeshRenderer>();
    }
    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        if (curMapping)
        {
            Vector3 differencePos = m_hands[curHandIndex].Bones[0].Transform.position - initialHandPos;//20 -30 = -10
            //Vector3 differencePosLocal = curFingertipBone.Transform.localPosition - initialFingertipPosLocal;//20 -30 = -10

            //transform.parent.position = initialPos + differencePos;
            //transform.parent.position += differencePosLocal;


            Vector3 midPointSphere = boneMappingHandler.CalcMidPoint(spherePoints);
            Vector3 midPointHand = boneMappingHandler.CalcMidPoint(handPoints);
            
            int[] fingerTipsIndex = { 5, 8, 11, 14, 18 };

            print("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            print(m_hands[curHandIndex].Bones[fingerTipsIndex[curFingerIndex]].Transform.position);
            print(midPointSphere);
            print(midPointHand);
            
            //transform.parent.position = midPointSphere + distHand * scale * direction; //Vector3.Scale(new Vector3(heading.x, heading.y, heading.z), new Vector3(scale, scale, scale));
            Vector3 mappingPos =  ((m_hands[curHandIndex].Bones[fingerTipsIndex[curFingerIndex]].Transform.position - midPointHand) * scale) 
                                  + midPointSphere;
            
            transform.parent.position = mappingPos;
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
        if (!curMapping)
        {
            int[] handAndFingerIndex = GetHandAndFingerIndex(collider);
            //if there is an associated hand, it means that an index of one of two hands is entering the cube
            //change the color of the cube accordingly (blue for left hand, green for right one)
            if (handAndFingerIndex[0] != -1 && handAndFingerIndex[1] != -1)
            

                //fingerMaterials[handIndex,fingerIdx].SetColor("_Color", fingerColor[fingerIdx]);
                changeHandMaterialScript.ChangeMaterial(handAndFingerIndex[0], handAndFingerIndex[1], gameObject);
            
            //m_renderer.material.color = fingerIdx == 0 ? m_renderer.material.color = Color.blue : m_renderer.material.color = Color.green;
               //m_isIndexStaying[0] = true;
                //m_isIndexStaying[1] = true;
            
        }
    }

    /// <summary>
    /// Trigger Exit.
    /// Notice that this gameobject must have a trigger collider
    /// </summary>
    /// <param name="collider">Collider of interest</param>
    private void OnTriggerExit(Collider collider)
    {/*
        if (!curMapping)
        {
            //get hand associated with trigger
            int fingerIdx = GetFingerIndex(collider);

            //if there is an associated hand, it means that an index of one of two hands is levaing the cube,
            //so set the color of the cube back to white, or to the one of the other hand, if it is in
            if (fingerIdx != -1)
            {
                //m_isIndexStaying[handIdx] = false;
                //m_renderer.material.color = m_isIndexStaying[0] ? m_renderer.material.color = Color.blue :
                //(m_isIndexStaying[1] ? m_renderer.material.color = Color.green : Color.white);
            }
        }*/
    }

    /// <summary>
    /// Gets the hand id associated with the index finger of the collider passed as parameter, if any
    /// </summary>
    /// <param name="collider">Collider of interest</param>
    /// <returns>0 if the collider represents the finger tip of left hand, 1 if it is the one of right hand, -1 if it is not an index fingertip</returns>
    private int[] GetHandAndFingerIndex(Collider collider)
    {
        int handIndex = -1;
        //Checking Oculus code, it is possible to see that physics capsules gameobjects always end with _CapsuleCollider
        if (collider.gameObject.name.Contains("_CapsuleCollider"))
        {
            //get the name of the bone from the name of the gameobject, and convert it to an enum value
            string boneName = collider.gameObject.name.Substring(0, collider.gameObject.name.Length - 16);
            OVRPlugin.BoneId boneId = (OVRPlugin.BoneId)Enum.Parse(typeof(OVRPlugin.BoneId), boneName);

            OVRPlugin.BoneId[] fingerTips = { OVRPlugin.BoneId.Hand_Thumb3, OVRPlugin.BoneId.Hand_Index3, OVRPlugin.BoneId.Hand_Middle3, OVRPlugin.BoneId.Hand_Ring3, OVRPlugin.BoneId.Hand_Pinky3 };
            int[] fingerTipsIndex = {5, 8, 11, 14, 18};
            
            if (collider.transform.IsChildOf(trackedHands[0].transform))
            {
                handIndex = 0;
            }
            else if (collider.transform.IsChildOf(trackedHands[1].transform))
            {
                handIndex = 1;
            }

            //if it is the tip of the finger
            for (int i = 0; i < fingerTips.Length; i++) //OVRPlugin.BoneId fingertip in fingerTips)
            {
                if (boneId == fingerTips[i])
                {
                    if (handIndex >= 0)
                    {
                        if (curFingertip != fingerTips[i])
                        {
                            //fingerMaterials[handIndex, curIndex].SetColor("_Color", Color.white);
                            OVRBone fingerTipBone = m_hands[handIndex].Bones[fingerTipsIndex[i]];
                            if (collider.bounds.Contains(fingerTipBone.Transform.position))
                            {
                                curFingertip = fingerTips[i];
                                //curIndex = i;
                                curFingertipBone = fingerTipBone;
                                //meshRend.material.SetColor("_Color", fingerColor[i]);
                                return new int[] { handIndex, i };
                            }
                        }
                    }
                }
            }
        }
        return new int[] { handIndex, -1 };

    }

    
    public void StartMapping(List<Transform> spherePoints, List<Transform> handPoints)
    {
        if (curFingertipBone != null)
        {
            curMapping = true;
            initialFingertipPosLocal = curFingertipBone.Transform.localPosition;
            initialFingertipRotation = curFingertipBone.Transform.rotation;
            initialHandPos = m_hands[curHandIndex].Bones[0].Transform.position;
            initialRotation = transform.parent.rotation;
            initialPos = transform.parent.position;

            initialFingertipPos = m_hands[curHandIndex].Bones[0].Transform.position;

            this.spherePoints = spherePoints;
            this.handPoints = handPoints;
            
            Vector3 midPointSphere = boneMappingHandler.CalcMidPoint(this.spherePoints);
            Vector3 midPointHand = boneMappingHandler.CalcMidPoint(this.handPoints);

            float distHand = Vector3.Distance(midPointHand, handPoints[0].position);
            float distSphere = Vector3.Distance(midPointSphere, spherePoints[0].position);
            scale = distSphere / distHand;
            if (scale < 1)
                scale = 1;
        }
    }

    public void StopMapping()
    {
        print("STOPS MAPPING IN BALL :)");
        curMapping = false;
    }

}