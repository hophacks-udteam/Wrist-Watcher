using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AsyncIO;
using NetMQ;
using NetMQ.Sockets;


/// <summary>
///     Example of requester who only sends Hello. Very nice guy.
///     You can copy this class and modify Run() to suits your needs.
///     To use this class, you just instantiate, call Start() when you want to start and Stop() when you want to stop.
/// </summary>
public class pythonConnector : RunAbleThread
{
    private HandFeature data = new HandFeature();
    private bool sending = false;
    public void sendData(float angleL, float angleR, float leftLeft, float leftRight, float rightLeft, float rightRight,float leftOrientation,float rightOrientation, bool Locked, int bad)
    {
        if (!sending)
        {
            data.angleL = angleL;
            data.angleR = angleR;
            data.leftLeft = leftLeft;
            data.leftRight = leftRight;
            data.rightLeft = rightLeft;
            data.rightRight = rightRight;
            data.leftOrientation = leftOrientation;
            data.rightOrientation = rightOrientation;
            data.Locked = Locked;
            data.bad = bad;
            sending = true;
            Run();
        }
    }
    public HandFeature getData()
    {
        if (!sending)
            return data;
        return null;
    }

    protected override void Run()
    {
        ForceDotNet.Force(); // this line is needed to prevent unity freeze after one use, not sure why yet
        using (RequestSocket client = new RequestSocket())
        {
            client.Connect("tcp://localhost:5555");


            Debug.Log("Sending Data over");
            client.SendFrame(data.ToString());
            // ReceiveFrameString() blocks the thread until you receive the string, but TryReceiveFrameString()
            // do not block the thread, you can try commenting one and see what the other does, try to reason why
            // unity freezes when you use ReceiveFrameString() and play and stop the scene without running the server
            //                string message = client.ReceiveFrameString();
            //                Debug.Log("Received: " + message);
            string message = null;
            bool gotMessage = false;
            
            while (Running)
            {
                gotMessage = client.TryReceiveFrameString(out message); // this returns true if it's successful
                if (gotMessage) break;
            }

            if (gotMessage) Debug.Log("Received " + message);
            data.FromString(message);
            sending = false;
        }

        NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use, not sure why yet
    }
}
