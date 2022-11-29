using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChangeModel : MonoBehaviour
{
    public GameObject[] models;
    private GameObject tempModel;
    private int testId = 0;
    private bool changeModel = true;
    private Selection selector;

    // Start is called before the first frame update
    void Start()
    {
        selector = GameObject.FindObjectOfType<Selection>();
        Shuffle();
        DeactivateModels();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (changeModel)
            {
                selector.DeselectObject();
                DeactivateModels();
                models[testId].SetActive(true);
                testId++;
                if (testId > 2)
                {
                    testId = 2;
                }
                changeModel = false;
            }
            else
            {
                changeModel = true;
            }
        }
    }

    public string GetModelName()
    {
        return models[testId].name;
    }

    public int GetTestId()
    {
        return testId;
    }

    public void Shuffle() {
        for (int i = 0; i < models.Length; i++) {
            int rnd = Random.Range(0, models.Length);
            tempModel = models[rnd];
            models[rnd] = models[i];
            models[i] = tempModel;
        }
    }

    public void DeactivateModels()
    {
        foreach (GameObject model in models)
        {
            model.SetActive(false);
        }
    }
}
