using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public class AnimationPlayer : MonoBehaviour
{
    private Animator anim;
    public GameObject target;
    private AnimatorController controller;
    private Selection selector;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
        controller = target.GetComponent<Animator>().runtimeAnimatorController as AnimatorController;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
