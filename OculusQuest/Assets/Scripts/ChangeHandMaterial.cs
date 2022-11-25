using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeHandMaterial : MonoBehaviour
{
    private BoneMappingHandler boneMappingHandler;

    //public GameObject[] colorChangeObjects;

    //public GameObject[] otherObjs;

    // Start is called before the first frame update
    void Start()
    {
        boneMappingHandler = GameObject.FindObjectOfType<BoneMappingHandler>();
        /*

                for (int i = 0; i < colorChangeObjects.Length; i++)
                {
                    int j = (int)(i / 5);
                    int index = i % 5;
                    print(j);
                    print(index);
                    boneMappingHandler.fingerMaterials[j, index] = colorChangeObjects[i].GetComponent<MeshRenderer>().material;

                }

                ChangeMaterial(0, 0, otherObjs[0]);
                ChangeMaterial(1, 0, otherObjs[1]);
                ChangeMaterial(0, 1, otherObjs[2]);
                ChangeMaterial(1, 1, otherObjs[0]);
                ChangeMaterial(0, 2, otherObjs[1]);
                ChangeMaterial(1, 2, otherObjs[2]);
                ChangeMaterial(0, 3, otherObjs[0]);
                ChangeMaterial(1, 3, otherObjs[1]);
                ChangeMaterial(0, 4, otherObjs[2]);
                ChangeMaterial(1, 4, otherObjs[0]);
                ChangeMaterial(1, 0, otherObjs[1]);
                ChangeMaterial(0, 1, otherObjs[2]);
                */
    }

    public void ChangeMaterial(int handIdx, int fingerIdx, GameObject otherobj)
    {
        /*BoneToRigMapping boneToRig = otherobj.GetComponent<BoneToRigMapping>();
        if (boneToRig) // They should have?
        {
            if (boneToRig.GetPreviousIndexes()[0] != handIdx || boneToRig.GetPreviousIndexes()[1] != fingerIdx)
                boneMappingHandler.ChangeFinger(boneToRig.GetPreviousIndexes()[0], boneToRig.GetPreviousIndexes()[1], null);

            //otherobj.GetComponent<MeshRenderer>().material.color = Color.white;
            boneToRig.SetIndexes(handIdx, fingerIdx);
        }*/
        boneMappingHandler.MapFinger(handIdx, fingerIdx, otherobj);

    }

    // Update is called once per frame
    void Update()
    {

    }
}