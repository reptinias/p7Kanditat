using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneMappingHandler : MonoBehaviour
{
    public BoneToRigMapping[] rigComponents;
    bool mapTransforms;
    private OVRHand[] trackedHands;
    private NewReadInputs InputDevices;
    private Color[] fingerColor = { Color.black, Color.blue, Color.yellow, Color.red, Color.green };

    public Material[,] fingerMaterials = new Material[2, 5];
    public Material[] fingerMaterials0TEST = new Material[5];
    public Material[] fingerMaterials1TEST = new Material[5];

    private GameObject[,] mappedObjects = new GameObject[2, 5];

    // Start is called before the first frame update
    void Start()
    {
        InputDevices = GameObject.Find("ReadInputs").GetComponent<NewReadInputs>();
        trackedHands = InputDevices.trackedHands;
        for (int i = 0; i < trackedHands.Length; i++)
            for (int j = 0; j < 5; j++)
            {

                fingerMaterials[i, j] = trackedHands[i].GetComponent<SkinnedMeshRenderer>().materials[j];
            }
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

    public void ChangeFinger(int handIndex, int fingerIdx, GameObject otherObject)
    {
        if (otherObject)
        {
            if (mappedObjects[handIndex, fingerIdx] != null)
            {
                mappedObjects[handIndex, fingerIdx].GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
                //fingerMaterials[handIndex, fingerIndex]
            }
            fingerMaterials[handIndex, fingerIdx].SetColor("_Color", fingerColor[fingerIdx]);
            mappedObjects[handIndex, fingerIdx] = otherObject;
            mappedObjects[handIndex, fingerIdx].GetComponent<MeshRenderer>().material.SetColor("_Color", fingerColor[fingerIdx]);
        }
        else
        {
            fingerMaterials[handIndex, fingerIdx].SetColor("_Color", Color.white);
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
}
