using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public class AnimationPlayer : MonoBehaviour
{
    private Animator anim;
    public GameObject target;
    public Animator animator;
    private Selection selector;
    
    
    // Start is called before the first frame update
    void Start()
    {
        selector = GameObject.FindObjectOfType<Selection>();
        updateController();
    }

    // Update is called once per frame
    void Update()
    {
        if (selector.newSelection)
        {
            updateController();
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("play animation");
            animator.Play("Animation");
        }
    }

    void updateController()
    {
        target = selector.getSelectedObject();
        animator = target.GetComponent<Animator>();
    }
}
