using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class NewReadInputs : MonoBehaviour
{
    public OVRSkeleton[] m_hands;
    public OVRHand[] trackedHands;
    // Start is called before the first frame update
    void Start()
    {
        m_hands = new OVRSkeleton[]
        {
            GameObject.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor/LeftOVRHandPrefab").GetComponent<OVRSkeleton>(),
            GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor/RightOVRHandPrefab").GetComponent<OVRSkeleton>()
        };
        trackedHands = new OVRHand[]
        {
            GameObject.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor/LeftOVRHandPrefab").GetComponent<OVRHand>(),
            GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor/RightOVRHandPrefab").GetComponent<OVRHand>()
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
