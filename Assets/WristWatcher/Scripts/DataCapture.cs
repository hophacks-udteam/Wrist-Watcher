using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using UnityEngine;
using UnityEditor;
using Leap.Unity;

public class DataCapture : MonoBehaviour
{
    private pythonConnector _connector;
    private List<string[]> rowData = new List<string[]>();
    public AudioSource badStuff;
    public CapsuleHand leftHand;
    public CapsuleHand rightHand;
    [Range(0f, 20f)]
    public float tolerance = 15f;
    [Range(85f, 130f)]
    public float target = 115f;
    [Range(1f, 10f)]
    public float timeout = 2f;

    public bool useServer = false;

    private float angleL = 110f;
    private float angleR = 110f;
    private float leftLeft = 0;
    private float leftRight = 0;
    private float rightLeft = 0;
    private float rightRight = 0;
    private float leftOrientation = 0;
    private float rightOrientation = 0;
    private float lastErr = 0f;
    private bool Locked = false;
    private int badL = 0;
    private int badR = 0;
    private HandFeature data = new HandFeature();
    string[] rowDataLeft = new string[11];
    string[] rowDataRight = new string[11];
    public Material leftHandMat;
    public Material rightHandMat;
    public Gradient colorScale;
    private float leftScore = 0;
    private float rightScore = 0;
    private void Start()
    {
        lastErr = Time.time;
        if (useServer)
        {
            _connector = new pythonConnector();
            _connector.Start();
        }
        leftHand.updateMatColor(Color.white);
        rightHand.updateMatColor(Color.white);
    }


    private void OnDestroy()
    {
        if (useServer)
        {
            _connector.Stop();
        }
    }
    // Update is called once per frame
    void Update()
    {
        leftScore = Mathf.Max(0, leftScore - .016f);
        rightScore = Mathf.Max(0, rightScore - .032f);

        // left right should be no more than 30 deg
        // Check if both hands are tracked
        Locked = leftHand.IsTracked && rightHand.IsTracked;

        // Up/down should be no more than 15 deg
        // Check up down angle
        if (leftHand.Handedness == Chirality.Left && leftHand.IsTracked && leftHand.GetLeapHand() != null)
        {
            //leftHand;
            Leap.Hand hand = leftHand.GetLeapHand();
            var arm = hand.Arm;
            Vector3 right = arm.Basis.xBasis.ToVector3() * arm.Width * 0.7f * 0.5f;
            Vector3 wrist = arm.WristPosition.ToVector3();
            Vector3 elbow = arm.ElbowPosition.ToVector3();
            float armLength = Vector3.Distance(wrist, elbow);
            wrist -= arm.Direction.ToVector3() * armLength * 0.05f;
            Vector3 thumbBase = leftHand.getThumbBase();
            Vector3 mockThumbBase = leftHand.getMockThumbBase();
            Vector3 leftArmFrontRight = wrist + right;
            Vector3 leftArmFrontLeft = wrist - right;
            leftLeft = Vector3.Distance(leftArmFrontRight, mockThumbBase);
            leftRight = Vector3.Distance(leftArmFrontLeft, thumbBase);


            leftOrientation = Mathf.Rad2Deg * (hand.Direction.AngleTo(arm.Direction));



            // Get up/down 
            Vector3 leftPalmNorm = -hand.PalmNormal.ToVector3();
            Vector3 leftArm = hand.Arm.Direction.ToVector3();
            angleL = Vector3.Angle(leftArm, leftPalmNorm);
        }
        if (rightHand.Handedness == Chirality.Right && rightHand.IsTracked && rightHand.GetLeapHand() != null)
        {
            //leftHand;
            Leap.Hand hand = rightHand.GetLeapHand();
            var arm = hand.Arm;
            Vector3 right = arm.Basis.xBasis.ToVector3() * arm.Width * 0.7f * 0.5f;
            Vector3 wrist = arm.WristPosition.ToVector3();
            Vector3 elbow = arm.ElbowPosition.ToVector3();
            float armLength = Vector3.Distance(wrist, elbow);
            wrist -= arm.Direction.ToVector3() * armLength * 0.05f;
            Vector3 thumbBase = rightHand.getThumbBase();
            Vector3 mockThumbBase = rightHand.getMockThumbBase();
            Vector3 rightArmFrontRight = wrist + right;
            Vector3 rightArmFrontLeft = wrist - right;
            rightLeft = Vector3.Distance(rightArmFrontRight, mockThumbBase);
            rightRight = Vector3.Distance(rightArmFrontLeft, thumbBase);
            // Get up/down 
            Vector3 leftPalmNorm = -hand.PalmNormal.ToVector3();
            Vector3 leftArm = hand.Arm.Direction.ToVector3();
            angleR = Vector3.Angle(leftArm, leftPalmNorm);

            rightOrientation = Mathf.Rad2Deg * (hand.Direction.AngleTo(arm.Direction));

        }
        if (Locked)
        {
            //Debug.Log(leftLeft + " " + leftRight + "::" + rightLeft+" " + rightRight);
            badL = 0;
            badR = 0;
            if (useServer)
            {
                // Connect to python server
                _connector.sendData(angleL, angleR, leftLeft, leftRight, rightLeft, rightRight, leftOrientation, rightOrientation, Locked, badL, badR);

                badL = _connector.getData().badL;
                badR = _connector.getData().badR;
            }
            else
            {
                if ((Mathf.Abs(angleL - target) > tolerance || (leftOrientation >= 35)))
                {
                    //Debug.Log(leftOrientation + " " + rightOrientation + " bad");
                    badL = 1;
                    //Debug.Log("Bad");
                }
                if ((Mathf.Abs(angleR - target) > tolerance || (rightOrientation >= 35)))
                {
                    //Debug.Log(leftOrientation + " " + rightOrientation + " bad");
                    badR = 1;
                    //Debug.Log("Bad");
                }
            }
            leftScore = Mathf.Min(100, leftScore + ((float)badL)/20);
            rightScore = Mathf.Min(100, rightScore + ((float)badR) / 60);
            try
            {
                leftHand.updateMatColor(colorScale.Evaluate(leftScore));
                rightHand.updateMatColor(colorScale.Evaluate(rightScore));
            }
            catch (MissingComponentException)
            {

            }
           
        }
        else
        {
            badL = 0;
            badR = 0;
        }
        if (badL == 1 || badR == 1)
        {
            // Some visual
            if (Time.time - lastErr >= timeout)
            {
                lastErr = Time.time;
                if (badStuff != null)
                    badStuff.Play();
            }
        }
        //Debug.Log(badL + " " + badR);
        rowDataLeft[0] = "" + angleL;
        rowDataLeft[1] = "" + leftLeft;
        rowDataLeft[2] = "" + leftRight;
        rowDataLeft[3] = "" + leftOrientation;
        rowDataLeft[4] = "" + badL;
        rowDataLeft[5] = "" + Locked;

        rowDataRight[0] = "" + angleR;
        rowDataRight[1] = "" + rightLeft;
        rowDataRight[2] = "" + rightRight;
        rowDataRight[3] = "" + rightOrientation;
        rowDataRight[4] = "" + badR;
        rowDataRight[5] = "" + Locked;

        string delimiter = ",";
        StringBuilder sbL = new StringBuilder();
        StringBuilder sbR = new StringBuilder();
        for (int index = 0; index < rowDataLeft.Length; index++)
        {
            sbL.AppendLine(string.Join(delimiter, rowDataLeft[index]));
            sbR.AppendLine(string.Join(delimiter, rowDataRight[index]));
        }
        string filePathL = "C:\\Users\\Vinay\\Documents\\WristWatcher\\Assets\\WristWatcher\\Left.csv";
        string filePathR = "C:\\Users\\Vinay\\Documents\\WristWatcher\\Assets\\WristWatcher\\Right.csv";

        // Create a file to write to.
        System.IO.File.AppendAllText(filePathL, sbL.ToString() + "\n");
        System.IO.File.AppendAllText(filePathR, sbR.ToString() + "\n");
        

        //Debug.Log(filePath);

        //Debug.Log("Left Angle:" + angleL + " Right Angle:" + angleR);

    }
}
