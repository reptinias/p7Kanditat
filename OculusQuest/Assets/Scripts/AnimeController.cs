using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimeController : MonoBehaviour
{
    public GameObject cube;
    public GameObject drage;
    
    public Animator animator;
    public Animator drageAnime;
    [SerializeField]
    private int pressNumber = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            pressNumber += 1;
            if (pressNumber == 7)
            {
                cube.SetActive(false);
                drage.SetActive(true);
            }

            animator.SetInteger("PressNumber", pressNumber);
            drageAnime.SetInteger("PressNumber", pressNumber);
        }
        
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            pressNumber -= 1;
            if (pressNumber == 7)
            {
                cube.SetActive(true);
                drage.SetActive(false);
            }

            animator.SetInteger("PressNumber", pressNumber);
            drageAnime.SetInteger("PressNumber", pressNumber);
        }
    }
}
