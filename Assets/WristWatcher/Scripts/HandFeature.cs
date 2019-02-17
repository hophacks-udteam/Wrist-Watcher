using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandFeature
{
    public float angleL = 110f;
    public float angleR = 110f;
    public float leftLeft = 0;
    public float leftRight = 0;
    public float rightLeft = 0;
    public float rightRight = 0;
    public float leftOrientation = 0;
    public float rightOrientation = 0;
    public bool Locked = false;
    public int badL = 0;
    public int badR = 0;

    public override string ToString()
    {
        return "" + angleL + "," + angleR + "," + leftLeft + "," + leftRight + "," + rightLeft + "," + rightRight + ","+ leftOrientation + "," + rightOrientation+","+(Locked==true?0:1)+"," + badL+","+badR;
    }
    public void FromString(string str)
    {
        if (str != null)
        {
            string[] data = new string[11];
            data = str.Split(',');
            angleL = float.Parse(data[0]);
            angleR = float.Parse(data[1]);
            leftLeft = float.Parse(data[2]);
            leftRight = float.Parse(data[3]);
            rightLeft = float.Parse(data[4]);
            rightRight = float.Parse(data[5]);
            leftOrientation = float.Parse(data[6]);
            rightOrientation = float.Parse(data[7]);
            badL = (int)float.Parse(data[9]);
            badR = (int)float.Parse(data[10]);
        }
    }
}
