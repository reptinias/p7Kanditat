using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

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

    public GameObject[][] mappedObjects = new GameObject[2][];

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
            mappedObjects[i] = objArr;
            
            print("Mapped objects length: " +  mappedObjects[i].Length);
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
        print( mappedObjects[0].Length);
        print( mappedObjects[1].Length);


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
        for (int i = 0; i < mappedObjects.Length; i++)
        {
            for (int j = 0; j < mappedObjects[i].Length; j++)
            {
                if (mappedObjects[i][j])
                {
                    mappedObjects[i][j].GetComponent<BoneToRigMapping>().ResetFinger();
                    mappedObjects[i][j].GetComponent<MeshRenderer>().material.SetColor("_Color", defaultColor);
                    mappedObjects[i][j] = null;
                }
                fingerMaterials[i][j].SetColor("_Color", Color.white);
            }
        }
    }

    public void MapFinger(int handIndex, int fingerIndex, GameObject collidedObj)
    {
        int[] handAndFingerIndex = collidedObj.GetComponent<BoneToRigMapping>().GetPreviousIndexes();
        int prevHandIndex = handAndFingerIndex[0];
        int prevFingerIndex = handAndFingerIndex[1];
        
        collidedObj.GetComponent<BoneToRigMapping>().SetIndexes(handIndex, fingerIndex);

        print("Prev indexes: " + prevHandIndex + " "  + prevFingerIndex);
        print("Indexes: " + handIndex + " "  + fingerIndex);

        if (prevHandIndex != -1 && prevFingerIndex != -1)
        {
            fingerMaterials[prevHandIndex][prevFingerIndex].SetColor("_Color", Color.white);

            //den henter selv den nye
            
            mappedObjects[prevHandIndex][prevFingerIndex].GetComponent<MeshRenderer>().material.SetColor("_Color", defaultColor);
            mappedObjects[prevHandIndex][prevFingerIndex] = null;
        }
        
        mappedObjects[handIndex][fingerIndex] = collidedObj;
        Color alphaCol = fingerColor[fingerIndex];
        alphaCol.a = 0.3f;
        mappedObjects[handIndex][ fingerIndex].GetComponent<MeshRenderer>().material.SetColor("_Color", alphaCol);
        fingerMaterials[handIndex][ fingerIndex].SetColor("_Color", fingerColor[fingerIndex]);

        if (!mappedObjects[handIndex][fingerIndex].transform.parent.gameObject.GetComponent<Animation>())
            mappedObjects[handIndex][fingerIndex].transform.parent.gameObject.AddComponent<Animation>();
        animRecorder.SetTarget(mappedObjects[handIndex][fingerIndex].transform.parent.gameObject);

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
            
            if (mappedObjects[handIndex][ fingerIdx] != null)
            {
                mappedObjects[handIndex][ fingerIdx].GetComponent<MeshRenderer>().material
                    .SetColor("_Color", defaultColor);
                //fingerMaterials[handIndex, fingerIndex]
            }
            
            fingerMaterials[handIndex][ fingerIdx].SetColor("_Color", fingerColor[fingerIdx]);
            mappedObjects[handIndex][ fingerIdx] = otherObject;
            Color alphaCol = fingerColor[fingerIdx];
            alphaCol.a = 0.3f;
            mappedObjects[handIndex][fingerIdx].GetComponent<MeshRenderer>().material.SetColor("_Color", alphaCol);
        }
        else
        {
            fingerMaterials[handIndex][ fingerIdx].SetColor("_Color", Color.white);
        }
    }

    public Vector3 CalcMidPoint(List<Transform> points)
    {
        float xTotal = 0;
        float yTotal = 0;
        float zTotal = 0;
        for (int i = 0; i < points.Count; i++)
        {
            xTotal += points[i].position.x;
            yTotal += points[i].position.y;
            zTotal += points[i].position.z;
        }
        return new Vector3(xTotal / points.Count, yTotal / points.Count, zTotal / points.Count);
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
            for (int i = 0; i < mappedObjects[j].Length; i++) {
                if (mappedObjects[j][i])
                {
                    spherePoints.Add(mappedObjects[j][i].transform.parent);

                    int handIndex = mappedObjects[j][i].GetComponent<BoneToRigMapping>().GetPreviousIndexes()[0];
                    int fingerIndex = mappedObjects[j][i].GetComponent<BoneToRigMapping>().GetPreviousIndexes()[1];
                    print("hand index: " + handIndex);
                    print("finger index: " + fingerIndex);

                    handPoints.Add(m_hands[handIndex].Bones[fingerTipsIndex[fingerIndex]].Transform);
                }
            }


        Vector3 midPointSphere = CalcMidPoint(spherePoints);
        Vector3 midPointHand = CalcMidPoint(handPoints);
        float distHand = Vector3.Distance(midPointHand, handPoints[0].position);
        float distSphere = Vector3.Distance(midPointSphere, spherePoints[0].position);
        float scale = distSphere / distHand;



        for (int i = 0; i < rigComponents.Count; i++)
        {
            Vector3 heading = spherePoints[i].position - midPointSphere;
            var distance = heading.magnitude;
            var direction = heading / distance;
            rigComponents[i].StartMapping(spherePoints, handPoints);
        }
    }

    public void StopMapping()
    {
        print("STOPS MAPPING :)");
        for (int i = 0; i < rigComponents.Count; i++)
        {
            rigComponents[i].StopMapping();
        }
    }

    public void AddRigComponents(BoneToRigMapping boneToRigMapping)
    {
        rigComponents.Add(boneToRigMapping);
    }
}
