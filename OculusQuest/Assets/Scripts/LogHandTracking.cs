using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using System.Linq;

public class LogHandTracking : MonoBehaviour
{
    [SerializeField]
    private OVRSkeleton[] m_hands;
    
    private LoggingManager loggingManager;
    private string CsvFileName = "HandData";
    
    [SerializeField] 
    private int classID = 0;
    
    private float[] xCoordinates = new float[24];
    private float[] yCoordinates = new float[24];
    private float[] zCoordinates = new float[24];
    
    private float[] normCoordinates = new float[72];
    
    
    // Start is called before the first frame update
    void Start()
    {
        m_hands = new OVRSkeleton[]
        {
            GameObject.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor/LeftOVRHandPrefab").GetComponent<OVRSkeleton>(),
            GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor/RightOVRHandPrefab").GetComponent<OVRSkeleton>()
        };
        
        // Find the logging Manager in the scene.
        loggingManager = GameObject.Find("Logging").GetComponent<LoggingManager>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (OVRSkeleton hand in m_hands)
        {
            int index = 0;
            loggingManager.Log(CsvFileName, "Class", classID);
            IList<OVRBone> handBones = hand.Bones;
            foreach (OVRBone bone in handBones)
            {
                xCoordinates[index] = bone.Transform.position.x;
                yCoordinates[index] = bone.Transform.position.y;
                zCoordinates[index] = bone.Transform.position.z;
                index++;
            }

            index = 0;
            for (int i = 0; i < xCoordinates.Length; i++)
            {
                normCoordinates[index]   = (xCoordinates[i] - xCoordinates.Min()) / (xCoordinates.Max() - xCoordinates.Min());
                normCoordinates[index+1] = (yCoordinates[i] - yCoordinates.Min()) / (yCoordinates.Max() - yCoordinates.Min());
                normCoordinates[index+2] = (zCoordinates[i] - zCoordinates.Min()) / (zCoordinates.Max() - zCoordinates.Min());
                index += 3;
            }

            for (int i = 0; i < normCoordinates.Length; i++)
            {
                loggingManager.Log(CsvFileName, "$Coord " + i.ToString(), normCoordinates[i]);
            }
        }

        if (Input.GetKey("space"))
        {
            loggingManager.SaveLog(CsvFileName);
            loggingManager.ClearLog(CsvFileName);
            loggingManager.NewFilestamp();
            print("CSV was saved");
        }
    }
}
