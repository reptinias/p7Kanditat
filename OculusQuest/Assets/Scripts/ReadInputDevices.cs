using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


public class ReadInputDevices : MonoBehaviour
{
    [SerializeField]
    private OVRSkeleton[] m_hands;
    
    // Start is called before the first frame update
    void Start()
    {
        m_hands = new OVRSkeleton[]
        {
            GameObject.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor/LeftOVRHandPrefab").GetComponent<OVRSkeleton>(),
            GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor/RightOVRHandPrefab").GetComponent<OVRSkeleton>()
        };
    }

    // Update is called once per frame
    void Update()
    {
        foreach (OVRSkeleton hand in m_hands)
        {
            IList<OVRBone> handBones = hand.Bones;
            foreach (OVRBone bone in handBones)
            {
                // write position til en csv-file
                Debug.Log(bone.Transform.position);
            }
        }
    }
}
