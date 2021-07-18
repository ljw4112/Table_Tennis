using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Threading;

public class Connect_Device : MonoBehaviour
{
    public int port = 5051;
    TcpListener listener;
    TcpClient tc;

    Thread receiveThread;
    NetworkStream stream;

    private bool connect;
    private float[] data;
    private float[] revision_data;
    private Quaternion rot;
    public void Start()
    {
        listener = new TcpListener(IPAddress.Any, 13027);                   //모바일 기기 읽기 시작
        receiveThread = new Thread(new ThreadStart(Listen));                //쓰레드 시작
        receiveThread.IsBackground = true;
    }

    public void startThread()                                               //Connect Panel이 열렸을 때 동작할 수 있도록 함.
    {
        listener.Start();
        receiveThread.Start();
    }

    public void Update()
    {

    }

    public void OnDisable()
    {
        if (receiveThread != null)
        {
            receiveThread.Abort();
        }

    }
    public void Listen()
    {
        Debug.Log("Waiting for broadcast");
        string message = "";
        tc = listener.AcceptTcpClient();
        stream = tc.GetStream();
        while (true)
        {
            try
            {
                int nbytes;
                byte[] bytes = new byte[35];
                nbytes = stream.Read(bytes, 0, bytes.Length);

                message = Encoding.UTF8.GetString(bytes);
                string[] gyroData = message.Split('|');
                connect = true;

                data = new float[4];
                data[0] = float.Parse(gyroData[0]);                     //gyro.attitude.w
                data[1] = float.Parse(gyroData[1]);                     //gyro.attitude.x
                data[2] = float.Parse(gyroData[2]);                     //gyro.attitude.y
                data[3] = float.Parse(gyroData[3]);                     //gyro.attitude.z

                rot = new Quaternion(data[1], -data[3], data[2], data[0]);

                string status = "Success";
                byte[] msg = Encoding.UTF8.GetBytes(status);
                stream.Write(msg, 0, msg.Length);
            }
            catch (SocketException e)
            {
                Debug.Log(e);
            }
        }
    }

    public bool isConnect()
    {
        return connect;
    }

    public Quaternion getData()
    {
        return rot;
    }

    public void OnApplicationQuit()
    {
        if (connect)
        {
            string status = "QuitMsg";
            byte[] msg = Encoding.UTF8.GetBytes(status);

            stream.Write(msg, 0, msg.Length);

            tc.Close();
            stream.Close();
        }
    }
}