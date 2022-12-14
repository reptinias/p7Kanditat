using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Linq;

public class Selection : MonoBehaviour
{
    public Transform objectsInScene;
    private List<GameObject> objects;
    public Outline outlineScript;
    public RigSelection rigSelectionScript;
    public bool allowRecording;

    float distance = 50f;

    private GameObject selectedObject = null;

    InteractableObject curInteractableObject;
    InteractableObject selectedInteractableObject;

    public AnimationRecorder animationRecorder;

    private NewReadInputs InputDevices;
    private OVRSkeleton[] m_hands;
    private OVRHand[] trackedHands;
    public float length = 1.0f;

    public LineRenderer line;

    bool moveAndRotate = false;

    Vector3 initialRootBonePos;
    Quaternion initialRootBoneRotation;
    Vector3 initialPos;
    Quaternion initialRotation;

    private OVRBone rootBone;

    private ActionTriggerScript actionTrigger;
    public bool newSelection;

    BoneMappingHandler boneMappingHandler;
    [SerializeField]
    public RecordingLightScript[] recordingLights;

    // Start is called before the first frame update
    void Start()
    {
        boneMappingHandler = GameObject.FindObjectOfType<BoneMappingHandler>();
           InputDevices = GameObject.Find("ReadInputs").GetComponent<NewReadInputs>();
        actionTrigger = GameObject.FindObjectOfType<ActionTriggerScript>().GetComponent<ActionTriggerScript>();

        m_hands = InputDevices.m_hands;

        allowRecording = false;
        UpdateObjectsInScene();
        DeselectObject();

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

            if (parent.transform.GetChild(i).GetComponent<TwoBoneIKConstraint>() || parent.transform.GetChild(i).GetComponent<ChainIKConstraint>())
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

                if (obj.GetComponent<Animation>())
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
                print("Added bone to: " + curInteractableObject);
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
        //if (rs)
            //obj.GetComponent<RigSelection>().SelectSphere(false);
    }

    public void DeselectObject() //
    {
        RemoveSelectedObjects();
        
        boneMappingHandler.RemoveAllMapping();
        
        selectedObject = null;
        if (selectedInteractableObject)
        {
            RemoveSelection(selectedInteractableObject.gameObject);
            selectedInteractableObject = null;
        }
        actionTrigger.ResetTransRotation();
        allowRecording = false;
        newSelection = false;
        foreach (RecordingLightScript light in recordingLights)
        {
            light.collisionAmount = 0;
        }
        animationRecorder.RemoveTargets();
    }

    void RemoveSelectedObjects()
    {
        foreach (GameObject child in objects)
        {
            if (selectedInteractableObject)
                if (selectedInteractableObject.gameObject == child)
                    continue;
            RemoveSelection(child);
        }
    }

    public void SelectObject(int handIndex)
    {
        RemoveSelectedObjects();

        Vector3 transformPosition = m_hands[handIndex].Bones.ElementAt(20).Transform.position;
        Vector3 transformForward = m_hands[handIndex].Bones.ElementAt(20).Transform.position - m_hands[handIndex].Bones.ElementAt(8).Transform.position;

        //create a ray cast and set it to the mouses cursor position in game
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        Ray ray = new Ray(transformPosition, transformForward);
        Vector3 endPosition = transformPosition + length * transformForward;

        if (Physics.Raycast(ray, out hit, distance))
        {
            GameObject tempObj = hit.transform.gameObject;

            Animation targetAnim = tempObj.GetComponent<Animation>();
            if (!targetAnim && tempObj.transform.parent)
            {
                tempObj = tempObj.transform.parent.gameObject;
            }

            if (tempObj.tag == "AnimateAble")
            {
                selectedObject = tempObj;
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

                /*RigSelection rs = selectedObject.GetComponent<RigSelection>();
                if (rs)
                {
                    rs.SelectSphere(true);
                }*/

                print(hit.transform);
                //draw invisible ray cast/vector

                line.SetPositions(new Vector3[2] { transformPosition, endPosition });
                InitialiseMoveAndRotate();
                allowRecording = true;
            }
            else
            {
                actionTrigger.ResetTransRotation();
            }
            //log hit area to the console
            //Debug.Log(hit.point);
        }
        else
        {
            //DeselectObject();
        }

        newSelection = true;
    }

    public void InitialiseMoveAndRotate()
    {
        if (selectedObject && moveAndRotate)
        {
            initialRootBonePos = rootBone.Transform.position;
            initialRootBoneRotation = rootBone.Transform.rotation;
            /*Collider selectedObjectCollider = selectedObject.GetComponent<Collider>();
            if (!selectedObjectCollider)
            {
                selectedObjectCollider = selectedObject.GetComponentInChildren<Collider>();
            }
            initialPos = selectedObjectCollider.bounds.center;*/
            initialPos = selectedObject.transform.position;
            initialRotation = selectedObject.transform.rotation;
            Debug.Log("Initial info " + initialPos + " " + initialRotation);
        }
    }

    public void MoveAndRotate(int handIndex)
    {
        moveAndRotate = true;
        rootBone = m_hands[handIndex].Bones[0];
        InitialiseMoveAndRotate();
    }

    public void StopMoveAndRotate()
    {
        moveAndRotate = false;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (moveAndRotate && selectedObject)
        {
            Vector3 differencePos = rootBone.Transform.position - initialRootBonePos;//20 -30 = -10
            Quaternion differenceRot = initialRootBoneRotation * Quaternion.Inverse((rootBone.Transform.rotation));//20 -30 = -10

            selectedObject.transform.position = initialPos + differencePos;
            selectedObject.transform.rotation = initialRotation * differenceRot;
        }
        /*if (Input.GetMouseButtonDown(0)){
            SelectObject();
        }*/
    }

    public GameObject getSelectedObject()
    {
        return selectedObject;
    }
}