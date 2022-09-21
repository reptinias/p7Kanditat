using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selection : MonoBehaviour
{
    public Transform objectsInScene;
    private List<GameObject> objects;
    public Outline outlineScript;

    float distance = 50f;

    GameObject selectedObject;

    // Start is called before the first frame update
    void Start()
    {
        UpdateObjectsInScene();
    }

    void UpdateObjectsInScene()
    {
        objects = new List<GameObject>();
        int children = objectsInScene.childCount;
        print("child count: " + children);
        for (int i = 0; i < children; ++i)
        {
            print("Foreach loop: " + objectsInScene.GetChild(i).gameObject);
            objects.Add(objectsInScene.GetChild(i).gameObject);
            GiveObjectOutline(objectsInScene.GetChild(i).gameObject);
        }
    }

    void GiveObjectOutline(GameObject obj)
    {
        Outline outline = obj.AddComponent<Outline>();
        outline.OutlineMode = outlineScript.OutlineMode;
        outline.OutlineWidth = outlineScript.OutlineWidth;
        outline.OutlineColor = outlineScript.OutlineColor;
        outline.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)){
            foreach (GameObject child in objects)
                child.GetComponent<Outline>().enabled = false;

            //create a ray cast and set it to the mouses cursor position in game
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, distance))
            {
                selectedObject = hit.transform.gameObject;
                Outline targetOutline = selectedObject.GetComponent<Outline>();
                if (targetOutline)
                {
                    targetOutline.enabled = true;
                }
                //draw invisible ray cast/vector
                Debug.DrawLine(ray.origin, hit.point);
                //log hit area to the console
                //Debug.Log(hit.point);
            }
            else
                selectedObject = null;
        }
    }
}