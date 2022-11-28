using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TeleportationScript : MonoBehaviour
{
    private NewReadInputs InputDevices;
    private OVRSkeleton[] hands;
    
    public LineRenderer line;
    public float maxLineLength = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        InputDevices = GameObject.Find("ReadInputs").GetComponent<NewReadInputs>();
        hands = InputDevices.m_hands;
    }

    public void Teleportation(int index)
    {
        RaycastHit hit;
        Ray ray = new Ray(hands[index].Bones.ElementAt(20).Transform.position, hands[index].Bones.ElementAt(20).Transform.position 
                                                                               - hands[index].Bones.ElementAt(8).Transform.position);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.CompareTag("Ground"))
            {
                transform.position = new Vector3(hit.point.x, 1f, hit.point.z);
            }
        }

        line.enabled = false;
    }

    public void Indicator(int index)
    {
        RaycastHit hit;
        Ray ray = new Ray(hands[index].Bones.ElementAt(20).Transform.position, hands[index].Bones.ElementAt(20).Transform.position 
                                                                               - hands[index].Bones.ElementAt(8).Transform.position);
        Vector3 endPosition = hands[index].Bones.ElementAt(20).Transform.position + (maxLineLength * hands[index].Bones.ElementAt(20).Transform.position 
            - hands[index].Bones.ElementAt(8).Transform.position);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.CompareTag("Ground"))
            {
                line.SetPositions(new Vector3[2]{hands[index].Bones.ElementAt(20).Transform.position, hit.point});
            }
        }
    }
}
