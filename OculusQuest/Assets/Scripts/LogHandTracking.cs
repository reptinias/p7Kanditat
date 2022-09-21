using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogHandTracking : MonoBehaviour
{
    [SerializeField]
    private OVRSkeleton[] m_hands;
    
    private LoggingManager loggingManager;
    private string CsvFileName = "HandData";
    
    [SerializeField] 
    private int classID = 0;
    
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

        // Tell the logging manager to save the data (to disk and SQL by default).
        loggingManager.SaveLog("MyLabel");

        // After saving the data, you can tell the logging manager to clear its logs.
        // Now its ready to save more data. Saving data will append to the existing log.
        loggingManager.ClearLog("MyLabel");

        // If you want to start a new file, you can ask loggingManager to generate
        // a new file timestamp. Saving data hereafter will go to the new file.
        loggingManager.NewFilestamp();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (OVRSkeleton hand in m_hands)
        {
           
            IList<OVRBone> handBones = hand.Bones;
            foreach (OVRBone bone in handBones)
            {
                loggingManager.Log(CsvFileName, classID + ", x", bone.Transform.position.x);
                loggingManager.Log(CsvFileName, classID + ", y", bone.Transform.position.y);
                loggingManager.Log(CsvFileName, classID + ", z", bone.Transform.position.z);
                Debug.Log(bone.Transform.position);
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
