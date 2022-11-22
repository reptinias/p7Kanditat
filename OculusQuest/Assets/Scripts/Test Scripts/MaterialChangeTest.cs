using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChangeTest : MonoBehaviour
{
    public MeshRenderer meshRend;

    // Start is called before the first frame update
    void Start()
    {
        meshRend.materials[0].SetColor("_Color", Color.yellow);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
