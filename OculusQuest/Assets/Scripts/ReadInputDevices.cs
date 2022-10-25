using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.Barracuda;
using System;
using System.Linq;


public class ReadInputDevices : MonoBehaviour
{
    public OVRSkeleton[] m_hands;
    private int handIndex;

    private float[] xCoordinates = new float[24];
    private float[] yCoordinates = new float[24];
    private float[] zCoordinates = new float[24];
    private float[] normCoordinates = new float[72];

    public NNModel nnmodel;
    private Model _runTimeModel;
    private IWorker _engine;
    public Prediction prediction;
    public string[] predString = new string[2];

    public struct Prediction
    {
        public int predictedValue;
        public float[] predicted;
        public string predictionString;
        
        public string convertPrediction(int predictionvalue)
        {
            switch (predictionvalue)
            {
                case 0:
                    predictionString = "open hand";
                    break;
                case 1:
                    predictionString = "closed hand";
                    break;
                case 2:
                    predictionString = "1-finger point";
                    break;
                case 3:
                    predictionString = "thumb up";
                    break;
                case 4:
                    predictionString = "ok hand";
                    break;
                case 5:
                    predictionString = "pinch";
                    break;
                case 6:
                    predictionString = "half pinch";
                    break;
            }

            return predictionString;
        }
        
        public string SetPrediction(Tensor tensor, int handIndex)
        {
            predicted = tensor.AsFloats();
            predictedValue = Array.IndexOf(predicted, predicted.Max());
            Debug.Log($" hand {handIndex} predicted as {convertPrediction(predictedValue)}");
            return convertPrediction(predictedValue);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_hands = new OVRSkeleton[]
        {
            GameObject.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor/LeftOVRHandPrefab").GetComponent<OVRSkeleton>(),
            GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor/RightOVRHandPrefab").GetComponent<OVRSkeleton>()
        };

        _runTimeModel = ModelLoader.Load(nnmodel);
        _engine = WorkerFactory.CreateWorker(_runTimeModel, WorkerFactory.Device.GPU);
        prediction = new Prediction();

    }

    // Update is called once per frame
    void Update()
    {

        handIndex = 0;
        foreach (OVRSkeleton hand in m_hands)
        {
            int index = 0;
            System.Collections.Generic.IList<OVRBone> handBones = hand.Bones;
            //foreach (OVRBone bone in handBones)
            for (int i = 0; i < handBones.Count; i++)
            {
                //sphereList[i].transform.position = handBones[i].Transform.position;
                xCoordinates[i] = handBones[i].Transform.position.x;
                yCoordinates[i] = handBones[i].Transform.position.y;
                zCoordinates[i] = handBones[i].Transform.position.z;
            }
            //Debug.Log($"Max {coordinates.Max()}. Min {coordinates.Min()}");
            for (int i = 0; i < xCoordinates.Length; i++)
            {
                normCoordinates[index]   = (xCoordinates[i] - xCoordinates.Min()) / (xCoordinates.Max() - xCoordinates.Min());
                normCoordinates[index+1] = (yCoordinates[i] - yCoordinates.Min()) / (yCoordinates.Max() - yCoordinates.Min());
                normCoordinates[index+2] = (zCoordinates[i] - zCoordinates.Min()) / (zCoordinates.Max() - zCoordinates.Min());
                index += 3;
            }
            
            //Debug.Log($"hand {handIndex}. coordinates max: {normCoordinates.Max()}. coordinates min: {normCoordinates.Min()}");
            var input = new Tensor(1, 72, normCoordinates);
            Tensor output = _engine.Execute(input).PeekOutput();
            input.Dispose();
            predString[handIndex] = prediction.SetPrediction(output, handIndex);
            handIndex += 1;
        }
    }

    private void OnDestroy()
    {
        _engine?.Dispose();
    }
}
