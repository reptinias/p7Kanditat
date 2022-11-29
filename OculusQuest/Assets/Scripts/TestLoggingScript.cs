using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLoggingScript : MonoBehaviour
{
    private ActionTriggerScript action;
    private LoggingManager loggingManager;
    private Selection selector;
    private GameObject user;
    private BoneMappingHandler mappingHandler;
    private AnimationRecorder animeRecorder;
    private int betweenTasks = 1;
    private ChangeModel changeModel;
    public GameObject[] AnimationObjects;

    private int TrailIndex = 0;
    private string modelName = "";
    
    int[,] translatedMapArray = new int[2,5];
    
    // Start is called before the first frame update
    void Start()
    {
        action = GameObject.FindObjectOfType<ActionTriggerScript>();
        loggingManager = GameObject.Find("Logging").GetComponent<LoggingManager>();
        selector = GameObject.FindObjectOfType<Selection>();
        user = GameObject.Find("OVRCameraRigCustom");
        mappingHandler = GameObject.FindObjectOfType<BoneMappingHandler>();
        animeRecorder = GameObject.FindObjectOfType<AnimationRecorder>();
        changeModel = GameObject.FindObjectOfType<ChangeModel>();
        AnimationObjects = changeModel.models;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (betweenTasks == 0)
            {
                betweenTasks = 1;
            }
            else
            {
                betweenTasks = 0;
                TrailIndex++;
            }

            TrailIndex = changeModel.GetTestId();
            modelName = changeModel.GetModelName();
        }
        
        loggingManager.Log("MyLabel", "Trail ID", TrailIndex);
        loggingManager.Log("MyLabel", "Model Name", modelName);
        
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (mappingHandler.mappedObjects[i][j] == null)
                {
                    translatedMapArray[i,j] = 0;
                }
                else
                {
                    translatedMapArray[i,j] = 1;
                }
            }
        }
        
        Dictionary<string, object> userData = new Dictionary<string, object>() {
            {"User X", user.transform.position.x},
            {"User Y", user.transform.position.y},
            {"User Z", user.transform.position.z}
        };
        loggingManager.Log("MyLabel", userData);
        
        Dictionary<string, object> animetableData = new Dictionary<string, object>() {
            {"AObject X", AnimationObjects[TrailIndex].transform.position.x},
            {"AObject Y", AnimationObjects[TrailIndex].transform.position.y},
            {"AObject Z", AnimationObjects[TrailIndex].transform.position.z}
        };
        loggingManager.Log("MyLabel", animetableData);
        
        Dictionary<string, object> handMapping = new Dictionary<string, object>() {
            {"Left Hand Thumb", translatedMapArray[0,0]},
            {"Left Hand Index", translatedMapArray[0,1]},
            {"Left Hand Long", translatedMapArray[0,2]},
            {"Left Hand Ring", translatedMapArray[0,3]},
            {"Left Hand Picky", translatedMapArray[0,4]},
            {"Right Hand Thumb", translatedMapArray[1,0]},
            {"Right Hand Index", translatedMapArray[1,1]},
            {"Right Hand Long", translatedMapArray[1,2]},
            {"Right Hand Ring", translatedMapArray[1,3]},
            {"Right Hand Picky", translatedMapArray[1,4]},
        };
        loggingManager.Log("MyLabel", handMapping);
        
        loggingManager.Log("MyLabel", "Gesture Shift", action.gestureShift);

        Dictionary<string, object> currentGestures = new Dictionary<string, object>() {
            {"Left Hand", action.currentGestures[0]},
            {"Right Hand", action.currentGestures[1]}
        };
        loggingManager.Log("MyLabel", currentGestures);

        if (selector.getSelectedObject() == null)
        {
            loggingManager.Log("MyLabel", "Selected Object", 0);
        }
        else
        {
            loggingManager.Log("MyLabel", "Selected Object", 1);
        }
        
        if (animeRecorder.recording)
        {
            loggingManager.Log("MyLabel", "Record Animation", 1);
        }
        else
        {
            loggingManager.Log("MyLabel", "Record Animation", 0);
        }
        
        loggingManager.Log("MyLabel", "Play Animation", action.animationPlay);
        
        loggingManager.Log("MyLabel", "Between tasks", betweenTasks);
        
        if (Input.GetKeyDown("return"))
        {
            // Tell the logging manager to save the data (to disk and SQL by default).
            loggingManager.SaveLog("MyLabel");

            // After saving the data, you can tell the logging manager to clear its logs.
            // Now its ready to save more data. Saving data will append to the existing log.
            loggingManager.ClearLog("MyLabel");

            // If you want to start a new file, you can ask loggingManager to generate
            // a new file timestamp. Saving data hereafter will go to the new file.
            loggingManager.NewFilestamp();
        }
    }
}
