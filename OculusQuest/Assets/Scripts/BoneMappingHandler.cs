using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class BoneMappingHandler : MonoBehaviour
{
    public List<BoneToRigMapping> rigComponents =  new List<BoneToRigMapping>();
    bool mapTransforms;
    private OVRHand[] trackedHands;
    private NewReadInputs InputDevices;
    private Color[] fingerColor = { Color.black, Color.blue, Color.yellow, Color.red, Color.green };

    public Material[][] fingerMaterials = new Material[2][];
    public Material[] fingerMaterials0TEST = new Material[5];
    public Material[] fingerMaterials1TEST = new Material[5];

    public GameObject[][] mappedObjects = new GameObject[2][];

    private GameObject[] objArr;
    // Start is called before the first frame update
    void Start()
    {
        InputDevices = GameObject.Find("ReadInputs").GetComponent<NewReadInputs>();
        trackedHands = InputDevices.trackedHands;
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
            }
            
            fingerMaterials[i] = fingerMaterialArr;
        }
        print( mappedObjects.Length);

        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            mapTransforms = !mapTransforms;
            if (mapTransforms)
                StartMapping();
            else
                StopMapping();
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
            
            mappedObjects[prevHandIndex][prevFingerIndex].GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
            mappedObjects[prevHandIndex][prevFingerIndex] = null;
        }
        
        mappedObjects[handIndex][fingerIndex] = collidedObj;
        mappedObjects[handIndex][ fingerIndex].GetComponent<MeshRenderer>().material.SetColor("_Color", fingerColor[fingerIndex]);
        fingerMaterials[handIndex][ fingerIndex].SetColor("_Color", fingerColor[fingerIndex]);

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
                    .SetColor("_Color", Color.white);
                //fingerMaterials[handIndex, fingerIndex]
            }
            
            fingerMaterials[handIndex][ fingerIdx].SetColor("_Color", fingerColor[fingerIdx]);
            mappedObjects[handIndex][ fingerIdx] = otherObject;
            mappedObjects[handIndex][ fingerIdx].GetComponent<MeshRenderer>().material.SetColor("_Color", fingerColor[fingerIdx]);
        }
        else
        {
            fingerMaterials[handIndex][ fingerIdx].SetColor("_Color", Color.white);
        }
    }

    void StartMapping()
    {
        foreach (BoneToRigMapping comp in rigComponents)
            comp.StartMapping();
    }

    void StopMapping()
    {
        foreach (BoneToRigMapping comp in rigComponents)
            comp.StopMapping();
    }

    public void AddRigComponents(BoneToRigMapping boneToRigMapping)
    {
        rigComponents.Add(boneToRigMapping);
    }
}
