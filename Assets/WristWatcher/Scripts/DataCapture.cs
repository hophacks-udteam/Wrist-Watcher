using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Leap.Unity;

public class DataCapture : MonoBehaviour
{
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
    private float lastErr = 0f;
    private bool Locked = false;
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
            Vector3 leftPalmNorm = -leftHand.GetLeapHand().PalmNormal.ToVector3();
            Vector3 leftArm = leftHand.GetLeapHand().Arm.Direction.ToVector3();
            angleL = Vector3.Angle(leftArm, leftPalmNorm);
        }
        if (rightHand.Handedness == Chirality.Right && rightHand.IsTracked && rightHand.GetLeapHand() != null)
        {
            Vector3 rightPalmNorm = -rightHand.GetLeapHand().PalmNormal.ToVector3();
            Vector3 rightArm = rightHand.GetLeapHand().Arm.Direction.ToVector3();
            angleR = Vector3.Angle(rightArm, rightPalmNorm);
        }
       
        if (Locked && (Mathf.Abs(angleR-target) > tolerance || Mathf.Abs(angleL - target) > tolerance) && (Time.time - lastErr) >= timeout)
        {
            Debug.Log("Bad");
            lastErr = Time.time;
            if (badStuff != null)
                badStuff.Play();
        }

        //Debug.Log("Left Angle:" + angleL + " Right Angle:" + angleR);
        
    }
}
