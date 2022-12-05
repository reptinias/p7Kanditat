using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using System;

[Serializable]
public class GameObjectArray
{
    public GameObject[] y;
}

public class BoneMappingHandler : MonoBehaviour
{
    public List<BoneToRigMapping> rigComponents =  new List<BoneToRigMapping>();
    public bool mapTransforms;
    private OVRHand[] trackedHands;
    private OVRSkeleton[] m_hands;
    private NewReadInputs InputDevices;
    private Color[] fingerColor = { Color.magenta, Color.blue, Color.yellow, Color.red, Color.green };
    private Color defaultColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);

    public Material[][] fingerMaterials = new Material[2][];
    public Material[] fingerMaterials0TEST = new Material[5];
    public Material[] fingerMaterials1TEST = new Material[5];

    public GameObjectArray[] mappedObjects = new GameObjectArray[2];

    private GameObject[] objArr;
    AnimationRecorder animRecorder;
    // Start is called before the first frame update
    void Start()
    {
        animRecorder = GameObject.FindObjectOfType<AnimationRecorder>();
        InputDevices = GameObject.Find("ReadInputs").GetComponent<NewReadInputs>();
        trackedHands = InputDevices.trackedHands;
        m_hands = InputDevices.m_hands;
        print("MAPPED OBJECTS");
        for (int i = 0; i < trackedHands.Length; i++)
        {
            Material[] fingerMaterialArr= new Material[5];
            objArr = new GameObject[5];
            mappedObjects[i].y = objArr;
            
            print("Mapped objects length: " +  mappedObjects[i].y.Length);
            for (int j = 0; j < 5; j++)
            {
                fingerMaterialArr[j] = trackedHands[i].GetComponent<SkinnedMeshRenderer>().materials[j];
                if (i == 0)
                    fingerMaterials0TEST[j] = trackedHands[i].GetComponent<SkinnedMeshRenderer>().materials[j];
                else
                    fingerMaterials1TEST[j] = trackedHands[i].GetComponent<SkinnedMeshRenderer>().materials[j];

                fingerMaterialArr[j].SetColor("_Color", Color.white);
            }
            
            fingerMaterials[i] = fingerMaterialArr;
        }
        print( mappedObjects.Length);
        print( mappedObjects[0].y.Length);
        print( mappedObjects[1].y.Length);


    }

    // Update is called once per frame
    void Update()
    {/*
        if (mapTransforms)
            StartMapping();
        else
            StopMapping();*/
    }

    public void RemoveAllMapping()
    {
        print("Remove All Mapping");
        for (int i = 0; i < mappedObjects.Length; i++)
        {
            for (int j = 0; j < mappedObjects[i].y.Length; j++)
            {
                if (mappedObjects[i].y[j])
                {
                    mappedObjects[i].y[j].GetComponent<BoneToRigMapping>().StopMapping();
                    mappedObjects[i].y[j].GetComponent<BoneToRigMapping>().ResetFinger();
                    mappedObjects[i].y[j].GetComponent<MeshRenderer>().material.SetColor("_Color", defaultColor);
                    mappedObjects[i].y[j] = null;
                }
                fingerMaterials[i][j].SetColor("_Color", Color.white);
            }
        }
        //spherePoints = new List<Transform>();
        //handPoints = new List<Transform>();
    }

    public void MapFinger(int handIndex, int fingerIndex, GameObject collidedObj)
    {
        print("Maps finger ");
        int[] handAndFingerIndex = collidedObj.GetComponent<BoneToRigMapping>().GetPreviousIndexes();
        int prevHandIndex = handAndFingerIndex[0];
        int prevFingerIndex = handAndFingerIndex[1];
       
        //check a, if prev 01 -> 01 def 
        if (prevHandIndex != -1 && prevFingerIndex != -1)
        {
            fingerMaterials[prevHandIndex][prevFingerIndex].SetColor("_Color", Color.white);
            mappedObjects[prevHandIndex].y[prevFingerIndex] = null;

        }

        //check index, if index changed -> index def
        if (mappedObjects[handIndex].y[fingerIndex])
        {
            mappedObjects[handIndex].y[fingerIndex].GetComponent<MeshRenderer>().material.SetColor("_Color", defaultColor);
            mappedObjects[handIndex].y[fingerIndex].GetComponent<BoneToRigMapping>().ResetFinger();
            mappedObjects[handIndex].y[fingerIndex] = null;
        }

        collidedObj.GetComponent<BoneToRigMapping>().SetIndexes(handIndex, fingerIndex);

        mappedObjects[handIndex].y[fingerIndex] = collidedObj;
        Color alphaCol = fingerColor[fingerIndex];
        alphaCol.a = 0.3f;
        mappedObjects[handIndex].y[ fingerIndex].GetComponent<MeshRenderer>().material.SetColor("_Color", alphaCol);
        fingerMaterials[handIndex][ fingerIndex].SetColor("_Color", fingerColor[fingerIndex]);

        if (!mappedObjects[handIndex].y[fingerIndex].transform.parent.gameObject.GetComponent<Animation>())
            mappedObjects[handIndex].y[fingerIndex].transform.parent.gameObject.AddComponent<Animation>();
        animRecorder.SetTarget(mappedObjects[handIndex].y[fingerIndex].transform.parent.gameObject);

    }

    public void ChangeFinger(int handIndex, int fingerIdx, GameObject otherObject)
    {
        if (otherObject)
        {
            print("handIndex: " + handIndex);
            print("fingerIndex: " + fingerIdx);
            print("Finger Index Color: " + fingerColor[fingerIdx]);
            print("(index)Material: " + fingerMaterials[handIndex][ fingerIdx]);
            print("(index) is material: "+(bool)fingerMaterials[handIndex][ fingerIdx]);
            print("(index)Material name: " + fingerMaterials[handIndex][ fingerIdx].name);
            
            if (mappedObjects[handIndex].y[ fingerIdx] != null)
            {
                mappedObjects[handIndex].y[ fingerIdx].GetComponent<MeshRenderer>().material
                    .SetColor("_Color", defaultColor);
                //fingerMaterials[handIndex, fingerIndex]
            }
            
            fingerMaterials[handIndex][ fingerIdx].SetColor("_Color", fingerColor[fingerIdx]);
            mappedObjects[handIndex].y[ fingerIdx] = otherObject;
            Color alphaCol = fingerColor[fingerIdx];
            alphaCol.a = 0.3f;
            mappedObjects[handIndex].y[fingerIdx].GetComponent<MeshRenderer>().material.SetColor("_Color", alphaCol);
        }
        else
        {
            fingerMaterials[handIndex][ fingerIdx].SetColor("_Color", Color.white);
        }
    }

    public Vector3 CalcMidPoint(List<Transform> points)
    {
        Vector3 addPoints = new Vector3(0,0,0);
        for (int i = 0; i < points.Count; i++)
        {
            addPoints += points[i].position;
        }
        return addPoints / points.Count;
        //return new Vector3(xTotal / points.Count, yTotal / points.Count, zTotal / points.Count);
    }

    List<Transform> spherePoints = new List<Transform>();
    List<Transform> handPoints = new List<Transform>();

    public void StartMapping()
    {
        spherePoints = new List<Transform>();
        handPoints = new List<Transform>();

        OVRPlugin.BoneId[] fingerTips = { OVRPlugin.BoneId.Hand_Thumb3, OVRPlugin.BoneId.Hand_Index3, OVRPlugin.BoneId.Hand_Middle3, OVRPlugin.BoneId.Hand_Ring3, OVRPlugin.BoneId.Hand_Pinky3 };
        int[] fingerTipsIndex = { 5, 8, 11, 14, 18 };

        print("START MAPPING EPIC");
        for (int j = 0; j < mappedObjects.Length; j++)
            for (int i = 0; i < mappedObjects[j].y.Length; i++) {
                if (mappedObjects[j].y[i])
                {
                    spherePoints.Add(mappedObjects[j].y[i].transform.parent);

                    int handIndex = mappedObjects[j].y[i].GetComponent<BoneToRigMapping>().GetPreviousIndexes()[0];
                    int fingerIndex = mappedObjects[j].y[i].GetComponent<BoneToRigMapping>().GetPreviousIndexes()[1];

                    handPoints.Add(m_hands[handIndex].Bones[fingerTipsIndex[fingerIndex]].Transform);
                }
            }



        for (int i = 0; i < spherePoints.Count; i++)
        {
            spherePoints[i].GetComponentInChildren<BoneToRigMapping>().StartMapping(spherePoints, handPoints);
        }
    }

    private Transform spherePointError;
    public void StopMapping()
    {
        print("STOPS MAPPING :)");
        for (int i = 0; i < spherePoints.Count; i++)
        {
            spherePointError = spherePoints[i];

            spherePoints[i].GetComponentInChildren<BoneToRigMapping>().StopMapping();
        }
    }

    public void AddRigComponents(BoneToRigMapping boneToRigMapping)
    {
        rigComponents.Add(boneToRigMapping);
    }
}
