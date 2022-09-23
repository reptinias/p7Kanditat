using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour
{
    public float speed = 4f;
    public GameObject self;
    
    

    void Start()
    {
        
    }


    //Very basic movement script. Only used for debugging of animation
    void Update()
    {
        if (Input.GetKey("w"))
            Move(Vector3.forward);
        if (Input.GetKey("s"))
            Move(Vector3.back);
        if (Input.GetKey("d"))
            Move(Vector3.right);
        if (Input.GetKey("a"))
            Move(Vector3.left);
    }

    void Move(Vector3 direction)
    {
        self.transform.position += Time.deltaTime*direction * speed;
    }
}
