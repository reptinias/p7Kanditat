using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class AnimationRecorder : MonoBehaviour
{
    // Start is called before the first frame update
    public AnimationClip clip;

    private GameObjectRecorder m_Recorder;
    
    void Start()
    {
        // Create recorder and record the script GameObject.
        m_Recorder = new GameObjectRecorder(gameObject);

        // Bind all the Transforms on the GameObject and all its children.
        m_Recorder.BindComponentsOfType<Transform>(gameObject, true);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetKey("p"))
            this.enabled = false;

        if (clip == null)
            return;
        m_Recorder.TakeSnapshot(Time.deltaTime);
    }
    void OnDisable()
    {
        if (clip == null)
            return;

        if (m_Recorder.isRecording)
        {
            // Save the recorded session to the clip.
            m_Recorder.SaveToClip(clip);
        }
    }
}
