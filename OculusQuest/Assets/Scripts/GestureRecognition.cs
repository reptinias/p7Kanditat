using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Unity.XR.CoreUtils;

public class GestureRecognition : MonoBehaviour
{
    private ReadInputDevices InputDevices;
    public bool recording = true;
    public bool anomaliesTesting = false;
    public string templateSaveName;
    public int pointsPerGesture = 30;
    public float samplingRate = 0.01f;
    public bool limitSamples = false;
    public int maxPointsAllowed = 100;
    public float standardRatio = 100f;
    public float devTightness = 1f;
    public float anomaliesFactor = 5f; 
    
    private bool gestureStarted;
    private bool gestureComplete;
    private bool inputReady;

    private string gestureFileName = "gestures.json";
    private Vector3 startPoint;
    private Vector3 currentPoint;
    private DrawnGesture currentGesture;
    private List<Vector3> currentPointList;
    private Vector3[] reducedPoints;
    private GestureTemplates templates;
    private float tempTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        InputDevices = GameObject.Find("ReadInputs").GetComponent<ReadInputDevices>();
        varInitialization();
        LoadTemplates();    
    }
    
    #region variable initialization and reset
    private void varInitialization()
    {
        currentPoint = new Vector3(0, 0, 0);
        startPoint = new Vector3(0, 0, 0);
        currentPointList = new List<Vector3>();
        currentPointList.Add(new Vector3(0, 0, 0));
        reducedPoints = new Vector3[pointsPerGesture];
        for (int i = 0; i < pointsPerGesture; i++)
        {
            reducedPoints[i] = new Vector3(0, 0, 0);
        }
        gestureStarted = false;
        gestureComplete = false;
        inputReady = false;
        currentGesture = new DrawnGesture("currentGesture", pointsPerGesture);
    }


    private void varReset()
    {
        for (int i = 0; i < pointsPerGesture; i++)
        {
            reducedPoints[i].Set(0, 0, 0);
            //reducedPoints[i].SetY(0);
            //reducedPoints[i].SetZ(0);
        }
        currentPointList.Clear();
        currentPointList.Add(new Vector3(0,0, 0));
        gestureStarted = false;
        gestureComplete = false;
    }

    #endregion
    
    // Update is called once per frame
    void Update()
    {
        tempTime += Time.deltaTime;
        // vvv Change this to a static gesture output fra DNN vvv
        for (int i = 0; i < InputDevices.m_hands.Length; i++)
        {
            //if (Input.GetMouseButton(0))
            if(InputDevices.procesGesture[i] == true)
            {
                if (inputReady)
                {
                    if (!gestureStarted)
                    {
                        gestureStarted = true;
                        StartGesture(InputDevices.m_hands[i].Bones[20].Transform.position*10);
                    }

                    if ((!gestureComplete) && (tempTime > samplingRate))
                    {
                        tempTime = 0f;
                        ContinueGesture(InputDevices.m_hands[i].Bones[20].Transform.position*10);
                    }

                    if (gestureComplete)
                    {
                        EndGesture();
                    }
                }
            }
            else
            {
                if (gestureStarted)
                {
                    EndGesture();
                }
                inputReady = true;
            }
        }
    }
    
    private void SaveTemplates()
    {
        string filePath = Application.dataPath + "/StreamingAssets/" + gestureFileName;
        string saveData = JsonUtility.ToJson(templates);
        File.WriteAllText(filePath, saveData);
    }

    private void LoadTemplates()
    {
        templates = new GestureTemplates();
        string filePath = Path.Combine(Application.streamingAssetsPath, gestureFileName);
        if (File.Exists(filePath))
        {
            string data = File.ReadAllText(filePath);
            templates = JsonUtility.FromJson<GestureTemplates>(data);
        }
    }
    
    private void StartGesture(Vector3 followPoint)
    {
        Debug.Log("gesture started");
        Vector3 fP = new Vector3(followPoint.x, followPoint.y, followPoint.z);
        startPoint.Set(fP.x, fP.y, fP.z);
        //startPoint.SetY(fP.y);
        //startPoint.SetZ(fP.z);
        gestureComplete = false;
    }
    
    private void ContinueGesture(Vector3 followPoint)
    {
        //Vector3 fP = new Vector3(followPoint.x, followPoint.y, followPoint.z);
        Debug.Log("points: " );
        Debug.Log(followPoint.x + " " + startPoint.x);
        Debug.Log(followPoint.y + " " + startPoint.y);
        Debug.Log(followPoint.z + " " + startPoint.z);
        
        currentPoint.Set(followPoint.x - startPoint.x, followPoint.y - startPoint.y, followPoint.z - startPoint.z);
        //currentPoint.SetY(followPoint.y - startPoint.GetY());
        //currentPoint.SetZ(followPoint.z - startPoint.GetZ());
        //Debug.Log("x: " + currentPoint.GetX() + " y: " + currentPoint.GetY() + " z: " + currentPoint.GetZ());
        currentPointList.Add(new Vector3(currentPoint.x, currentPoint.y, currentPoint.z));
        if (currentPoint.x > currentGesture.GetMaxX())
        {
            currentGesture.SetMaxX(currentPoint.x);
        }
        if (currentPoint.x < currentGesture.GetMinX())
        {
            currentGesture.SetMinX(currentPoint.x);
        }
        
        if (currentPoint.y > currentGesture.GetMaxY())
        {
            currentGesture.SetMaxY(currentPoint.y);
        }
        if (currentPoint.y < currentGesture.GetMinY())
        {
            currentGesture.SetMinY(currentPoint.y);
        }
        
        if (currentPoint.z < currentGesture.GetMinZ())
        {
            currentGesture.SetMinZ(currentPoint.z);
        }
        if (currentPoint.z < currentGesture.GetMinZ())
        {
            currentGesture.SetMinZ(currentPoint.z);
        }
        
        if (limitSamples && currentPointList.Count >= maxPointsAllowed)
        {
            gestureComplete = true;
            Debug.Log(message: "Gesture Complete!");
        }
    }
    
    private void EndGesture()
    {
        if (inputReady) inputReady = false;
        gestureStarted = false;
        gestureComplete = true;
        Rescale(currentGesture);
        MapPoints(currentGesture);
        if (recording)
        {
            currentGesture.SetName(templateSaveName);
            templates.templates.Add(new DrawnGesture(currentGesture.GetName(), pointsPerGesture, 
                currentGesture.GetMaxX(), currentGesture.GetMaxY(), currentGesture.GetMaxZ(),
                currentGesture.GetMinX(), currentGesture.GetMinY(), currentGesture.GetMinZ(),
                currentGesture.GetPoints()));
            SaveTemplates();
        } else
        {
            DrawnGesture m = FindMatch(currentGesture, templates);
            Debug.Log(m.GetName());
        }
        varReset();
    }
    
    private void Rescale(DrawnGesture gesture)
    {
        float scale = 1f;
        float xrange = gesture.GetMaxX() - gesture.GetMinX();
        float yrange = gesture.GetMaxY() - gesture.GetMinY();
        float zrange = gesture.GetMaxZ() - gesture.GetMinZ();

        if (xrange >= yrange && xrange >= zrange)
        {
            scale = standardRatio / (gesture.GetMaxX() - gesture.GetMinX());
        } 
        if(yrange >= xrange && yrange >= zrange)
        {
            scale = standardRatio / (gesture.GetMaxY() - gesture.GetMinY());
        }
        if (zrange >= xrange && zrange >= yrange)
        {
            scale = standardRatio / (gesture.GetMaxZ() - gesture.GetMinZ());
        }

        if (scale != 1)
        {
            foreach (Vector3 point in currentPointList)
            {
                point.Set(point.x * scale, point.y * scale, point.z * scale);
                //point.SetY(point.GetY() * scale);
                //point.SetZ(point.GetZ() * scale);
            }
        }
    }
    
    private void MapPoints(DrawnGesture gesture)
    {
        reducedPoints[0].Set(currentPointList[0].x, currentPointList[0].y, currentPointList[0].z);
        //reducedPoints[0].SetY(currentPointList[0].GetY());
        int newIndex = 1;
        float totalDistance = TotalDistance();
        float coveredDistance = 0;
        float thisDistance = 0;
        float idealInterval = totalDistance / pointsPerGesture;
        for (int i = 0; i < currentPointList.Count - 1; i++)
        {
            thisDistance = PointDistance(currentPointList[i], currentPointList[i + 1]);
            bool passedIdeal = (coveredDistance + thisDistance) >= idealInterval;
            if (passedIdeal)
            {
                Vector3 reference = currentPointList[i];
                while (passedIdeal && newIndex < reducedPoints.Length)
                {
                    float percentNeeded = (idealInterval - coveredDistance) / thisDistance;
                    if (percentNeeded > 1f) percentNeeded = 1f;
                    if (percentNeeded < 0f) percentNeeded = 0f;
                    float new_x = (((1f - percentNeeded) * reference.x) + (percentNeeded * currentPointList[i + 1].x));
                    float new_y = (((1f - percentNeeded) * reference.y) + (percentNeeded * currentPointList[i + 1].y));
                    float new_z = (((1f - percentNeeded) * reference.z) + (percentNeeded * currentPointList[i + 1].z));
                    reducedPoints[newIndex] = new Vector3(new_x, new_y, new_z);
                    reference = reducedPoints[newIndex];
                    newIndex++;
                    thisDistance = (coveredDistance + thisDistance) - idealInterval;
                    coveredDistance = 0;
                    passedIdeal = (coveredDistance + thisDistance) >= idealInterval;
                }
                coveredDistance = thisDistance;
            } else
            {
                coveredDistance += thisDistance;
            }
            gesture.SetPoints(reducedPoints);
        }
    }
    
    private DrawnGesture FindMatch(DrawnGesture playerGesture, GestureTemplates templates)
    {
        float minAvgDifference = float.MaxValue;
        DrawnGesture match = new DrawnGesture("no match", pointsPerGesture);
        foreach(DrawnGesture template in templates.templates)
        {
            Debug.Log(template.GetName());
            float d = AverageDifference(playerGesture, template);
            Debug.Log(d.ToString());
            if (d < minAvgDifference)
            {
                minAvgDifference = d;
                match = template;               
            }
        }
        return match;
    }
    
    private float AverageDifference(DrawnGesture playerGesture, DrawnGesture template)  
    {
        int numPoints = playerGesture.GetNumPoints();

        if (numPoints != template.GetNumPoints())
        {
            Debug.Log("Number of points differs from templates");
            return -1f;
        }

        float totalDifference = 0;

        for (int i = 0; i < numPoints; i++)
        {
            totalDifference += PointDistance(playerGesture.GetPoints()[i], template.GetPoints()[i]);
        }

        return (totalDifference / numPoints);
    }
    
    private float TotalDistance()
    {
        float totalDistance = 0;
        for(int i = 0; i < currentPointList.Count - 1; i++)
        {
            totalDistance += PointDistance(currentPointList[i], currentPointList[i + 1]);
        }
        Debug.Log("total distance: " + totalDistance);
        return totalDistance;
    }
    private float PointDistance(Vector3 a, Vector3 b)
    {
        float xDif = a.x - b.x;
        float yDif = a.y - b.y;
        float zDif = a.z - b.z;
        Debug.Log("Distance: " + (xDif * xDif) + (yDif * yDif) + (zDif * zDif));
        return Mathf.Sqrt((xDif * xDif) + (yDif * yDif) + (zDif * zDif));
    }
}



[Serializable]
public class GestureTemplates
{
    public List<DrawnGesture> templates;
    public GestureTemplates()
    {
        templates = new List<DrawnGesture>();
    }

}

public class DrawnGesture
{
    private Vector3[] points;
    private string name;
    private float maxX;
    private float minX;
    private float maxY;
    private float minY;
    private float maxZ;
    private float minZ;
    private int numPoints;

    public DrawnGesture(string newName, int pointsPerGesture)
    {
        numPoints = pointsPerGesture;
        points = new Vector3[numPoints];
        name = newName;
        maxX = 0;
        maxY = 0;
        maxZ = 0;
    }
    public DrawnGesture(string newName, int pointsPerGesture, float max_x, float max_y, float max_z, 
        float min_x, float min_y, float min_z, Vector3[] newPoints)
    {
        numPoints = pointsPerGesture;
        points = new Vector3[numPoints];
        SetPoints(newPoints);
        name = newName;
        maxX = max_x;
        minX = min_x;
        maxY = max_y;
        minY = min_y;
        maxZ = max_z;
        minZ = min_z;
    }
    public void Reset()
    {
        maxX = 0;
        minX = 0;
        maxY = 0;
        minY = 0;
        maxZ = 0;
        minZ = 0;
        name = "";
        Array.Clear(points, 0, numPoints);
    }

    public Vector3[] GetPoints()
    {
        return points;
    }
    public void SetPoints(Vector3[] new_points)
    {
        for(int i = 0; i < numPoints; i++)
        {
            points[i] = new Vector3(new_points[i].x, new_points[i].y, new_points[i].z);
        }
    }
    public string GetName()
    {
        return name;
    }
    public void SetName(string n)
    {
        name = n;
    }
    public float GetMaxX()
    {
        return maxX;
    }
    public void SetMaxX(float x)
    {
        maxX = x;
    }
    public float GetMaxY()
    {
        return maxY;
    }
    public void SetMaxY(float y)
    {
        maxY = y;
    }
    public float GetMaxZ()
    {
        return maxZ;
    }

    public void SetMaxZ(float z)
    {
        maxZ = z;
    }
    public float GetMinX()
    {
        return minX;
    }
    public void SetMinX(float x)
    {
        minX = x;
    }
    public int GetNumPoints()
    {
        return numPoints;
    }
    public float GetMinY()
    {
        return minY;
    }
    public void SetMinY(float y)
    {
        minY = y;
    }
    public float GetMinZ()
    {
        return minZ;
    }
    public void SetMinZ(float z)
    {
        minZ = z;
    }
    public void SetNumPoints(int n)
    {
        numPoints = n;
    }
}

public class ThreeDPoint
{
    private float x;
    private float y;
    private float z;

    public ThreeDPoint(float startx, float starty, float startz)
    {
        x = startx;
        y = starty;
        z = startz;
    }

    public float GetX()
    {
        return x;
    }
    public void SetX(float new_x)
    {
        x = new_x;
    }
    public float GetY()
    {
        return y;
    }
    public void SetY(float new_y)
    {
        y = new_y;
    }
    public float GetZ()
    {
        return z;
    }

    public void SetZ(float new_z)
    {
        z = new_z;
    }
} 
