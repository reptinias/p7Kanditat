using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigSelection : MonoBehaviour
{
    public GameObject spherePrefab;
    private GameObject sphere;
    InteractableObject interactableObject;

    public Material notSelectedMat;
    public Material selectedMat;

    bool sphereSelected = false;
    BoneMappingHandler boneMappingHandler;

    private void Start()
    {
        boneMappingHandler = GameObject.FindObjectOfType<BoneMappingHandler>();
    }

    public void SpawnSelectionBall(InteractableObject io)
    {
        sphere = Instantiate(spherePrefab);
        sphere.transform.parent = transform;
        sphere.transform.localPosition = new Vector3(0,0,0);
        interactableObject = io;

        boneMappingHandler = GameObject.FindObjectOfType<BoneMappingHandler>();
        var s = sphere.GetComponent<BoneToRigMapping>();
        print(s);
        boneMappingHandler.AddRigComponents(s);
    }

    /*public void EnableInteractableObject()
    {
        interactableObject.EnableOutline(true);
    }*/

    public void EnableSphere(bool enabled)
    {
        sphere.SetActive(enabled);
    }

   /* public void SelectSphere(bool selected)
    {
        sphereSelected = selected;

        if (selected)
            sphere.GetComponent<MeshRenderer>().material = selectedMat;
        else
            sphere.GetComponent<MeshRenderer>().material = notSelectedMat;

    }*/

    private void Update()
    {
    }
}
