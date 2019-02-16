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
    private List<string[]> rowData = new List<string[]>();
    public AudioSource badStuff;
    public CapsuleHand leftHand;
    public CapsuleHand rightHand;
    [Range(0f,20f)]
    public float tolerance = 15f;
    [Range(85f,130f)]
    public float target = 115f;
    [Range(1f,10f)]
    public float timeout = 2f;

    
    private float angleL = 110f;
    private float angleR = 110f;
    private float leftLeft = 0;
    private float leftRight = 0;
    private float rightLeft = 0;
    private float rightRight = 0;
    private float wrong = 0;
    private float lastErr = 0f;
    private bool Locked = false;
    private int bad = 0;
    string[] rowDataTemp = new string[8];
    private void Start()
    {
        lastErr = Time.time;
    }
   
    // Update is called once per frame
    void Update()
    {
        // left right should be no more than 30 deg
        // Check if both hands are tracked
        Locked = leftHand.IsTracked && rightHand.IsTracked;

        // Up/down should be no more than 15 deg
        // Check up down angle
        if (leftHand.Handedness==Chirality.Left && leftHand.IsTracked && leftHand.GetLeapHand() != null)
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
        }
        if (Locked)
        {
            //Debug.Log(leftLeft + " " + leftRight + "::" + rightLeft+" " + rightRight);
        }
        
        if (Locked && (Mathf.Abs(angleR-target) > tolerance || Mathf.Abs(angleL - target) > tolerance) && (Time.time - lastErr) >= timeout)
        {
            bad = 1;
            //Debug.Log("Bad");
            lastErr = Time.time;
            if (badStuff != null)
                badStuff.Play();
        }
        else
        {
            bad = 0;
        }
        rowDataTemp[0] = "" + angleL;
        rowDataTemp[1] = "" + angleR;
        rowDataTemp[2] = "" + leftLeft;
        rowDataTemp[3] = "" + leftRight;
        rowDataTemp[4] = "" + rightLeft;
        rowDataTemp[5] = "" + rightRight;
        rowDataTemp[6] = "" + bad;
        rowDataTemp[7] = "" + Locked;
        rowData.Add(rowDataTemp);
        string delimiter = ",";
        StringBuilder sb = new StringBuilder();
        for (int index = 0; index < rowDataTemp.Length; index++)
            sb.AppendLine(string.Join(delimiter, rowDataTemp[index]));
        string filePath = "C:\\Users\\Vinay\\Documents\\WristWatcher\\Assets\\WristWatcher\\test.csv";
        // Create a file to write to.
        System.IO.File.AppendAllText(filePath,sb.ToString()+"\n");

        //Debug.Log(filePath);

    //Debug.Log("Left Angle:" + angleL + " Right Angle:" + angleR);

}
}
