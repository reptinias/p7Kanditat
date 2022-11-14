using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Selection : MonoBehaviour
{
    public Transform objectsInScene;
    private List<GameObject> objects;
    public Outline outlineScript;
    public RigSelection rigSelectionScript;

    float distance = 50f;

    GameObject selectedObject;

    InteractableObject curInteractableObject;
    InteractableObject selectedInteractableObject;

    public AnimationRecorder animationRecorder;

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
            GameObject root = objectsInScene.GetChild(i).gameObject;
            print("Foreach loop: " + root);
            MakeObjectSelectable(root, true);
        }
    }

    void MakeChildrenSelectable(GameObject parent)
    {
        int children = parent.transform.childCount;

        for (int i = 0; i < children; ++i)
        {
            if (parent.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>())
            {
                MakeObjectSelectable(parent.transform.GetChild(i).gameObject, true);
            }

            if (parent.transform.GetChild(i).GetComponent<TwoBoneIKConstraint>())
            {
                Transform[] cArr = parent.transform.GetChild(i).GetComponentsInChildren<Transform>();
                foreach (Transform child in cArr)
                    if (child.name.Contains("target"))
                        MakeObjectSelectable(child.gameObject, false);
            }

            if (parent.transform.GetChild(i).childCount > 0)
                MakeChildrenSelectable(parent.transform.GetChild(i).gameObject);
        }
    }

    void MakeObjectSelectable(GameObject obj, bool hasOutline)
    {
        MeshRenderer mr = obj.GetComponent<MeshRenderer>();
        SkinnedMeshRenderer smr = obj.GetComponent<SkinnedMeshRenderer>();
        if (!mr && !smr && hasOutline)
        {
            MakeChildrenSelectable(obj);
        }
        else
        {
            if (hasOutline)
            {
                Outline outline = obj.AddComponent<Outline>();
                outline.OutlineMode = outlineScript.OutlineMode;
                outline.OutlineWidth = outlineScript.OutlineWidth;
                outline.OutlineColor = outlineScript.OutlineColor;
                outline.enabled = false;

                GameObject parentObject;

                if (obj.GetComponent<Animator>())
                    parentObject = obj;
                else
                    parentObject = obj.transform.parent.gameObject;

                curInteractableObject = parentObject.AddComponent<InteractableObject>();
                curInteractableObject.SetOutLine(outline);
                objects.Add(parentObject);
            }
            else
            {
                RigSelection rs = obj.AddComponent<RigSelection>();
                rs.spherePrefab = rigSelectionScript.spherePrefab;
                rs.selectedMat = rigSelectionScript.selectedMat;
                rs.notSelectedMat = rigSelectionScript.notSelectedMat;
                objects.Add(obj);

                curInteractableObject.AddBone(rs);
                rs.SpawnSelectionBall(curInteractableObject);
            }
        }
    }

    void RemoveSelection(GameObject obj)
    {
        InteractableObject io = obj.GetComponent<InteractableObject>();
        RigSelection rs = obj.GetComponent<RigSelection>();

        if (io)
            obj.GetComponent<InteractableObject>().EnableOutline(false);
        if (rs)
            obj.GetComponent<RigSelection>().SelectSphere(false);
    }

    public void SelectObject()
    {
        foreach (GameObject child in objects)
        {
            if (selectedInteractableObject)
                if (selectedInteractableObject.gameObject == child)
                    continue;
            RemoveSelection(child);
        }

        //create a ray cast and set it to the mouses cursor position in game
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distance))
        {
            selectedObject = hit.transform.gameObject;

            Animator targetAnim = selectedObject.GetComponent<Animator>();
            if (!targetAnim)
            {
                selectedObject = selectedObject.transform.parent.gameObject;
            }

            InteractableObject io = selectedObject.GetComponent<InteractableObject>();
            if (io)
            {
                if (selectedInteractableObject)
                {
                    GameObject previousSelectedObject = selectedInteractableObject.gameObject;
                    if (selectedObject != previousSelectedObject)
                    {
                        RemoveSelection(previousSelectedObject);
                    }
                }

                selectedInteractableObject = selectedObject.GetComponent<InteractableObject>();
                selectedInteractableObject.EnableOutline(true);
                animationRecorder.SetTarget(selectedInteractableObject.gameObject);
            }

            RigSelection rs = selectedObject.GetComponent<RigSelection>();
            if (rs)
            {
                rs.SelectSphere(true);
            }

            print(hit.transform);
            //draw invisible ray cast/vector
            Debug.DrawLine(ray.origin, hit.point);
            //log hit area to the console
            //Debug.Log(hit.point);
        }
        else
        {
            selectedObject = null;
            if (selectedInteractableObject)
            {
                RemoveSelection(selectedInteractableObject.gameObject);
                selectedInteractableObject = null;
            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)){
            SelectObject();
        }
    }
}