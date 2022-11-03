using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneMappingHandler : MonoBehaviour
{
    public BoneToRigMapping[] rigComponents;
    bool mapTransforms;

    // Start is called before the first frame update
    void Start()
    {
        
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
