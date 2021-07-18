using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Net;
using System.Net.Sockets;
using System.Text;

public class tMobile : MonoBehaviour
{
    private TcpClient client;
    private int PORT_NUMBER = 13027;
    private byte[] bytes;
    private byte[] msg;
    private string message;
    private NetworkStream stream;
    private bool start = false;

    public Button b;
    public InputField input;

    private Gyroscope gyro;

    // Start is called before the first frame update
    void Start()
    {
        gyro = Input.gyro;
        gyro.enabled = true;

        Screen.orientation = ScreenOrientation.Portrait;
        b.onClick.AddListener(connect);
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            b.gameObject.SetActive(false);
            input.gameObject.SetActive(false);

            getData();
            bytes = Encoding.UTF8.GetBytes(message);
            stream.Write(bytes, 0, bytes.Length);

            msg = new byte[7];
            int nbytes = stream.Read(msg, 0, 7);
            string quitMessage = Encoding.UTF8.GetString(msg);

            if (quitMessage.Equals("QuitMsg"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    private void connect()
    {
        if(input.text != "")    
        {
            start = !start;
            client = new TcpClient(input.text, PORT_NUMBER);
            stream = client.GetStream();
        }
    }

    private void getData()
    {
        if (gyro.enabled)
        {
            message = "";

            double tmp_w = Convert.ToDouble(gyro.attitude.w);
            message += check(tmp_w);

            message += '|';

            double tmp_x = Convert.ToDouble(gyro.attitude.x);
            message += check(tmp_x);

            message += '|';

            double tmp_y = Convert.ToDouble(gyro.attitude.y);
            message += check(tmp_y);

            message += '|';

            double tmp_z = Convert.ToDouble(gyro.attitude.z);
            message += check(tmp_z);
        }

        Debug.Log(message);
    }

    private string check(double d)              //자릿수 통일(하나당 길이 7만큼만 유지)
    {
        string result = "";
        d = Math.Truncate(d * 100000) / 100000;       //소숫점 넷째자리 이하로 자름 0.0000
        string tmp = d.ToString();
        if(d > 0)
        {
            result += "+";
            tmp = tmp.PadRight(7, '0');
            result += tmp;

        }
        else
        {
            tmp = tmp.PadRight(8, '0');
            result = tmp;
        }
       

        return result;
    }
}
