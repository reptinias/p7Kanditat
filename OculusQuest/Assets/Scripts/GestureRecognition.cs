using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using Unity.Mathematics;
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
    public bool extendedGesture = false;
    public float threshold = 1.0f;
    
    private bool gestureStarted;
    private bool gestureComplete;
    private bool inputReady;

    private string gestureFileName = "gestures.json";
    private ThreeDPoint startPoint;
    private ThreeDPoint currentPoint;
    private DrawnGesture currentGesture;
    private List<ThreeDPoint> currentPointList;
    private ThreeDPoint[] reducedPoints;
    private GestureTemplates templates;
    private float tempTime = 0f;

    private int handIndex;
    private OVRSkeleton hand;
    // Start is called before the first frame update
    void Start()
    {
        varInitialization();
        LoadTemplates();
        
        InputDevices = GameObject.Find("ReadInputs").GetComponent<ReadInputDevices>();
        name = gameObject.name;
        if (name == "LeftHandAnchor")
        {
            hand = InputDevices.m_hands[0];
            handIndex = 0;
        }
        else
        {
            hand = InputDevices.m_hands[1];
            handIndex = 1;
        }
    }
    
    #region variable initialization and reset
    private void varInitialization()
    {
        currentPoint = new ThreeDPoint(0, 0, 0);
        startPoint = new ThreeDPoint(0, 0, 0);
        currentPointList = new List<ThreeDPoint>();
        currentPointList.Add(new ThreeDPoint(0, 0, 0));
        reducedPoints = new ThreeDPoint[pointsPerGesture];
        for (int i = 0; i < pointsPerGesture; i++)
        {
            reducedPoints[i] = new ThreeDPoint(0, 0, 0);
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
            reducedPoints[i].SetX(0);
            reducedPoints[i].SetY(0);
            reducedPoints[i].SetZ(0);
        }
        currentPointList.Clear();
        currentPointList.Add(new ThreeDPoint(0,0, 0));
        gestureStarted = false;
        gestureComplete = false;
    }

    #endregion
    
    // Update is called once per frame
    void Update()
    {
        tempTime += Time.deltaTime;
        if(InputDevices.procesGesture[handIndex] == true)
        {
            if (inputReady)
            {
                if (!gestureStarted)
                {
                    gestureStarted = true;
                    StartGesture(hand.Bones.ElementAt(20).Transform.position*10);
                }

                if ((!gestureComplete) && (tempTime > samplingRate))
                {
                    tempTime = 0f;
                    ContinueGesture(hand.Bones.ElementAt(20).Transform.position*10);
                }
                
                if (gestureComplete && extendedGesture == false)
                {
                    EndGesture();
                }
            }
        }
        else
        {
            if (gestureStarted && extendedGesture == false)
            {
                EndGesture();
            }

            inputReady = true;
        }
    }
    
    private void SaveTemplates()
    {
        string filePath = Application.dataPath + "/StreamingAssets/" + gestureFileName;
        string saveData = JsonUtility.ToJson(templates, true);
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
        startPoint.SetX(fP.x);
        startPoint.SetY(fP.y);
        startPoint.SetZ(fP.z);
        gestureComplete = false;
        extendedGesture = false;
    }
    
    private void ContinueGesture(Vector3 followPoint)
    {
        currentPoint.SetX(followPoint.x - startPoint.GetX());
        currentPoint.SetY(followPoint.y - startPoint.GetY());
        currentPoint.SetZ(followPoint.z - startPoint.GetZ());

        currentPointList.Add(new ThreeDPoint(currentPoint.GetX(), currentPoint.GetY(), currentPoint.GetZ()));
        if (currentPoint.GetX() > currentGesture.GetMaxX())
        {
            currentGesture.SetMaxX(currentPoint.GetX());
        }
        if (currentPoint.GetX() < currentGesture.GetMinX())
        {
            currentGesture.SetMinX(currentPoint.GetX());
        }
        
        if (currentPoint.GetY() > currentGesture.GetMaxY())
        {
            currentGesture.SetMaxY(currentPoint.GetY());
        }
        if (currentPoint.GetY() < currentGesture.GetMinY())
        {
            currentGesture.SetMinY(currentPoint.GetY());
        }
        
        if (currentPoint.GetZ() > currentGesture.GetMaxZ())
        {
            currentGesture.SetMaxZ(currentPoint.GetZ());
        }
        if (currentPoint.GetZ() < currentGesture.GetMinZ())
        {
            currentGesture.SetMinZ(currentPoint.GetZ());
        }
        
        if (limitSamples && currentPointList.Count >= maxPointsAllowed)
        {
            extendedGesture = true;
            Rescale(currentGesture);
            MapPoints(currentGesture);
            DrawnGesture m = FindMatch(currentGesture, templates);
            if (m.GetName() == "no match")
            {
                currentPointList.RemoveRange(0, Math.Min(10, currentPointList.Count));
            }
            else
            {
                gestureComplete = true;
                Debug.Log("Extended Gesture: " + m.GetName());
            }
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
        }
        else
        {
            DrawnGesture m = FindMatch(currentGesture, templates);
            Debug.Log("short Gesture: " + m.GetName());
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
            foreach (ThreeDPoint point in currentPointList)
            {
                point.SetX(point.GetX() * scale);
                point.SetY(point.GetY() * scale);
                point.SetZ(point.GetZ() * scale);
            }
        }
    }
    
    private void MapPoints(DrawnGesture gesture)
    {
        reducedPoints[0].SetX(currentPointList[0].GetX());
        reducedPoints[0].SetY(currentPointList[0].GetY());
        reducedPoints[0].SetZ(currentPointList[0].GetZ());
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
                ThreeDPoint reference = currentPointList[i];
                while (passedIdeal && newIndex < reducedPoints.Length)
                {
                    float percentNeeded = (idealInterval - coveredDistance) / thisDistance;
                    if (percentNeeded > 1f) percentNeeded = 1f;
                    if (percentNeeded < 0f) percentNeeded = 0f;
                    float new_x = (((1f - percentNeeded) * reference.GetX()) + (percentNeeded * currentPointList[i + 1].GetX()));
                    float new_y = (((1f - percentNeeded) * reference.GetY()) + (percentNeeded * currentPointList[i + 1].GetY()));
                    float new_z = (((1f - percentNeeded) * reference.GetZ()) + (percentNeeded * currentPointList[i + 1].GetZ()));
                    reducedPoints[newIndex] = new ThreeDPoint(new_x, new_y, new_z);
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
            //Debug.Log(template.GetName());
            float d = AverageDifference(playerGesture, template);
            //Debug.Log(d.ToString());
            if (d < minAvgDifference && d < threshold)
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
    private float PointDistance(ThreeDPoint a, ThreeDPoint b)
    {
        float xDif = a.GetX() - b.GetX();
        float yDif = a.GetY() - b.GetY();
        float zDif = a.GetZ() - b.GetZ();
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

[Serializable]
public class DrawnGesture
{
    public ThreeDPoint[] points;
    public string name;
    public float maxX;
    public float minX;
    public float maxY;
    public float minY;
    public float maxZ;
    public float minZ;
    public int numPoints;

    public DrawnGesture(string newName, int pointsPerGesture)
    {
        numPoints = pointsPerGesture;
        points = new ThreeDPoint[numPoints];
        name = newName;
        maxX = 0;
        maxY = 0;
        maxZ = 0;
    }
    public DrawnGesture(string newName, int pointsPerGesture, float max_x, float max_y, float max_z, 
        float min_x, float min_y, float min_z, ThreeDPoint[] newPoints)
    {
        numPoints = pointsPerGesture;
        points = new ThreeDPoint[numPoints];
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

    public ThreeDPoint[] GetPoints()
    {
        return points;
    }
    public void SetPoints(ThreeDPoint[] new_points)
    {
        for(int i = 0; i < numPoints; i++)
        {
            points[i] = new ThreeDPoint(new_points[i].GetX(), new_points[i].GetY(), new_points[i].GetZ());
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

[Serializable]
public class ThreeDPoint
{
    public float x;
    public float y;
    public float z;

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
