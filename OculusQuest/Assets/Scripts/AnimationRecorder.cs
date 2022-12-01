using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class AnimationRecorder : MonoBehaviour
{
    private GameObjectRecorder[] m_Recorders;
    public List<GameObject> targets = new List<GameObject>();
    //private AnimatorController controller;
    private List<Animation> anims = new List<Animation>();
    public bool recording = false;
    private List<AnimationClip> clips = new List<AnimationClip>();
    private List<bool> isTarget = new List<bool>();

    private void Start()
    {
        /* foreach (GameObject target in targets)
        {
            anims.Add(target.GetComponent<Animation>());
            clips.Add(null);
            isTarget.Add(true);

        }*/
    }

    public void RemoveTargets()
    {
        for (int i = 0; i < isTarget.Count; i++)
        {
            isTarget[i] = false;
        }
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
            for (int i = 0; i < targets.Count; i++)
            {
                if (!isTarget[i])
                    continue;

                if (clips[i] == null)
                    continue;
                m_Recorders[i].TakeSnapshot(Time.deltaTime);
            }
        }

    }

    public void PlayRecording()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (anims[i].GetClipCount() > 0)
                if (!anims[i].isPlaying)
                    anims[i].PlayQueued("Test");
        }
    }

    public void StopPlayingClip()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (anims[i].GetClipCount() > 0)
                if (anims[i].isPlaying)
                    anims[i].Stop();
        }
    }

    //Called to start recording. Takes a target object and that target's AnimatorController as arguments(will fix later)
    //Changes to be made: Find target object's AnimatorController automatically
    public void StartRecording()
    {
        if (!recording)
        {
            Debug.Log("i've started recording");

            m_Recorders = new GameObjectRecorder[targets.Count];
            for (int i = 0; i < targets.Count; i++)
            {
                if (!isTarget[i])
                {
                    Debug.LogError("Start Recording not target?");

                    continue;
                }


                m_Recorders[i] = new GameObjectRecorder(targets[i]);

                // Bind all the Transforms on the GameObject and all its children.
                m_Recorders[i].BindComponent(targets[i].transform);
                //m_Recorders[i].BindComponentsOfType<Transform>(targets[i], true);
                //Clears clip data
                if (clips[i] == null)
                    clips[i] = new AnimationClip { name = "Test" };
                else
                    Debug.LogError("Start Recording not null");

                //Safeguard. Dunno if it does anything anymore
                //target.GetComponent<Animator>().StopPlayback();
                recording = true;
            }
        }
    }

    //Called to stop recording
    public void StopRecording()
    {
        if (recording)
        {
            Debug.Log("i've stopped recording");
            for (int i = 0; i < targets.Count; i++)
            {
                if (!isTarget[i])
                    continue;

                if (clips[i] == null)
                {
                    Debug.LogError("AAAAAAAAAAaaa: " + i);
                    continue;
                }

                if (m_Recorders[i].isRecording)
                {
                    //Creates a new animation clip in the designated path
                    //AssetDatabase.CreateAsset(clip, "Assets/" + target.name + "Anim" + System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Second.ToString() + ".anim");
                    //Saves clip to AnimatorController
                    clips[i].legacy = true;
                    //anim.clip = clip;
                    anims[i].AddClip(clips[i], "Test");

                    //Animator animator = target.GetComponent<Animator>();
                    //animator.GetCurrentAnimatorClipInfo(0)[0].clip.;
                    //AnimatorState state = controller.AddMotion(clip);
                    //clip.wrapMode = WrapMode.Loop;
                    // Save the recorded session to the clip.
                    m_Recorders[i].SaveToClip(clips[i]);


                    //Safeguard. Dunno if it does anything anymore
                    //target.GetComponent<Animator>().StopPlayback();

                }
                recording = false;
            }
        }
    }

    void Save()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (clips[i] != null)
                //Creates a new animation clip in the designated path
                AssetDatabase.CreateAsset(clips[i], "Assets/" + targets[i].name + "Anim" + System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Second.ToString() + ".anim");
        }
    }

    public void OnApplicationQuit()
    {
        Save();
    }

    public void SetTarget(GameObject newTarget)
    {
        bool hasAlreadyBeenUsed = false;
        for (int i = 0; i < targets.Count; i++)
        {
            if (newTarget == targets[i])
            {
                isTarget[i] = true;
                hasAlreadyBeenUsed = true;

            }
        }
        if (!hasAlreadyBeenUsed)
        {
            targets.Add(newTarget);
            anims.Add(newTarget.GetComponent<Animation>());
            clips.Add(null);
            isTarget.Add(true);
        }
    }
}
