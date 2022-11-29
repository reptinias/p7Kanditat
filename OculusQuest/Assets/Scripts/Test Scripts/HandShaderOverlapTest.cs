using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandShaderOverlapTest : MonoBehaviour
{
    private Color[] fingerColor = { Color.magenta, Color.blue, Color.yellow, Color.red, Color.green, Color.white };
    SkinnedMeshRenderer smr;
    private void Start()
    {
        smr = GetComponentInChildren<SkinnedMeshRenderer>();

        for (int i = 0; i < smr.materials.Length; i++)
        {
            smr.materials[i].SetColor("_Color", fingerColor[i]);
        }

    }


}
