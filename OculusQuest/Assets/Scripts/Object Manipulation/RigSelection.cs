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

    public void SpawnSelectionBall(InteractableObject io)
    {
        sphere = Instantiate(spherePrefab);
        sphere.transform.parent = transform;
        sphere.transform.localPosition = new Vector3(0,0,0);
        interactableObject = io;
    }

    /*public void EnableInteractableObject()
    {
        interactableObject.EnableOutline(true);
    }*/

    public void EnableSphere(bool enabled)
    {
        sphere.SetActive(enabled);
    }

    public void SelectSphere(bool selected)
    {
        sphereSelected = selected;

        if (selected)
            sphere.GetComponent<MeshRenderer>().material = selectedMat;
        else
            sphere.GetComponent<MeshRenderer>().material = notSelectedMat;

    }

    private void Update()
    {
        if (sphereSelected)
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += Vector3.up * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.position += Vector3.down  * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.position += Vector3.left * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.position += Vector3.right * Time.deltaTime;
            }
        }
    }
}
