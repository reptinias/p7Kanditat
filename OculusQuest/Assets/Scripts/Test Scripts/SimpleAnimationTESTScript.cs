using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAnimationTESTScript : MonoBehaviour
{
    float t = 0f;
    bool right;

    public AnimationRecorder animRecorder;
    bool recording = false;
    public bool shouldRecord;

    bool secondtime = false;

    GameObject child;

    // Start is called before the first frame update
    void Start()
    {
        child = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !recording && shouldRecord)
        {
            if (secondtime)
            {
                animRecorder.RemoveTargets();
                animRecorder.SetTarget(child);
            }

            animRecorder.StartRecording();
            print("Start Recording");
            recording = true;
        }
        else if (Input.GetKeyDown(KeyCode.R) && recording && shouldRecord)
        {
            animRecorder.StopRecording();
            print("Stop Recording");
            recording = false;
            secondtime = true;
        }

        if (Input.GetKeyDown(KeyCode.P) && shouldRecord)
        {
            print("Start Clip");
            animRecorder.PlayRecording();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!secondtime)
        {
            if (recording || !shouldRecord)
            {
                if (!right)
                    t += Time.deltaTime;

                if (right)
                    t -= Time.deltaTime;

                if (!right && t > 1)
                    right = true;

                if (right && t < 0)
                    right = false;

                if (shouldRecord)
                    transform.position = Vector3.Lerp(new Vector3(-7, 0, 0), new Vector3(7, 0, 0), t);
                else if (!shouldRecord)
                    transform.position = Vector3.Lerp(new Vector3(-7, 2, 0), new Vector3(7, 2, 0), t);
            }
        }
        else if (recording)
        {
            if (!right)
                t += Time.deltaTime;

            if (right)
                t -= Time.deltaTime;

            if (!right && t > 1)
                right = true;

            if (right && t < 0)
                right = false;

            if (shouldRecord)
                child.transform.position = Vector3.Lerp(new Vector3(-7, 0, 0), new Vector3(7, 0, 0), t);
        }
    }
}
