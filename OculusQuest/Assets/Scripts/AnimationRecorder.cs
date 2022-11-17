using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class AnimationRecorder : MonoBehaviour
{
    private GameObjectRecorder m_Recorder;
    public GameObject target;
    private AnimatorController controller;
    private bool recording = false;
    private AnimationClip clip;

    private void Start()
    {
        controller = target.GetComponent<Animator>().runtimeAnimatorController as AnimatorController;
    }

    void LateUpdate()
    {
       /* //If the program is recording and the p-key is pressed, stop recording
    
        if (recording && Input.GetKey("p"))
        {
            StopRecording();
        }

        //If the program is NOT recording and the k-key is pressed, start recording
        if (recording == false && Input.GetKey("k"))
        {
            StartRecording(target, controller);
        }*/
        //Records changes to the object and saves them to the recorder
        if (recording)
        {
            if (clip == null)
                return;
            m_Recorder.TakeSnapshot(Time.deltaTime);
        }
        
    }

    //Called to start recording. Takes a target object and that target's AnimatorController as arguments(will fix later)
    //Changes to be made: Find target object's AnimatorController automatically
    public void StartRecording()
    {
        if (!recording)
        {
            Debug.Log("i've started recording");
            m_Recorder = new GameObjectRecorder(target);

            // Bind all the Transforms on the GameObject and all its children.
            m_Recorder.BindComponentsOfType<Transform>(target, true);
            //Clears clip data
            clip = null;
            clip = new AnimationClip();

            //Safeguard. Dunno if it does anything anymore
            target.GetComponent<Animator>().StopPlayback();
            recording = true;
        }
    }

    //Called to stop recording
    public void StopRecording()
    {
        if (recording)
        {
            Debug.Log("i've stopped recording");
            if (clip == null)
                return;

            if (m_Recorder.isRecording)
            {
                //Creates a new animation clip in the designated path
                AssetDatabase.CreateAsset(clip,
                    "Assets/" + target.name + "Anim" + System.DateTime.Now.Minute.ToString() +
                    System.DateTime.Now.Second.ToString() + ".anim");
                //Saves clip to AnimatorController
                controller.AddMotion(clip);
                // Save the recorded session to the clip.
                m_Recorder.SaveToClip(clip);

                //Safeguard. Dunno if it does anything anymore
                target.GetComponent<Animator>().StopPlayback();

            }
            recording = false;
        }
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
        controller = target.GetComponent<Animator>().runtimeAnimatorController as AnimatorController;
    }
}
