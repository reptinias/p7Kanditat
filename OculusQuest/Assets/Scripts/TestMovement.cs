using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour
{
    public float speed = 4f;
    public GameObject self;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("w"))
            Move(Vector3.forward);
        else if (Input.GetKey("s"))
            Move(Vector3.back);
        else if (Input.GetKey("d"))
            Move(Vector3.right);
        else if (Input.GetKey("a"))
            Move(Vector3.left);
    }

    void Move(Vector3 direction)
    {
        self.transform.position += direction * speed * Time.deltaTime;
    }
}
