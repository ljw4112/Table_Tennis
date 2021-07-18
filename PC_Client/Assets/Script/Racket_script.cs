using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Threading;
using Photon;
public class Racket_script : Photon.PunBehaviour
{
    public Quaternion rot;

    private float x, y, z;
    public bool connect;

    Rigidbody rb;

    float pre_y;
    float now_y;
    float pre_z;
    float now_z;

    public float ac_x;
    public float ac_y;
    public float ac_z;
    int cnt_y;
    int cnt_z;

    private Rotate_Revision rr;
    private ball_script bs;

    private float[] revision_angular;
    private Connect_Device cd;

    private bool follow = false;
    private GameObject ball;

    private PhotonView pv;

    private Vector3 originPosition;

    public bool keyboardEnable;
    public void Start()
    {
        pv = GetComponent<PhotonView>();
        //pv.ObservedComponents[0] = this;

        rb = gameObject.GetComponent<Rigidbody>();
        cnt_y = 40;
        cnt_z = 40;

        rr = GameObject.Find("Network").GetComponent<Rotate_Revision>();
        bs = GetComponent<ball_script>();

        cd = GameObject.Find("Network").GetComponent<Connect_Device>();

        revision_angular = rr.revision_data;
        ball = GameObject.Find("TT_Ball(Clone)");

        originPosition = rb.position;
    }

    public void Update()
    {
        
        
    }

    public void FixedUpdate()
    {
        if(photonView.isMine){
            if(keyboardEnable){
                float keyhorizontal = Input.GetAxis("Horizontal");
                float keyvertical = Input.GetAxis("Vertical");
                transform.Translate(Vector3.right * 5 * Time.smoothDeltaTime * keyhorizontal , Space.World);
              transform.Translate(Vector3.up * 5 * Time.smoothDeltaTime * keyvertical , Space.World);
            }
            rot = cd.getData();

            x = rot.eulerAngles.x;
            y = rot.eulerAngles.y;
            z = rot.eulerAngles.z;

            transform.rotation = Quaternion.Euler(x - revision_angular[0], y - revision_angular[1], z - revision_angular[2]);
        }
        else{
           
        }
        
       


        Vector3 ps;
        var ball = GameObject.Find("TT_Ball(Clone)");
        if(ball){
            ps = ball.GetComponent<ball_script>().getposition();

            if (photonView.isMine && ball.GetComponent<PhotonView>().isMine && ball.GetComponent<ball_script>().colCnt > 0)
            {
                if (photonView.ownerId == 1)
                {
                    rb.position = new Vector3(ps.x + 0.7f, ps.y, rb.position.z);

                }
                else
                {
                    rb.position = new Vector3(ps.x - 0.7f, ps.y, rb.position.z);
                }
            }
        }
        
        
        float temp;

        // y축 각속도
        pre_y = now_y;
        now_y = rb.transform.rotation.eulerAngles.y;
        //Debug.Log(now_x);
        //float temp = System.Math.Min(now_x - pre_x, 360-now_x+pre_x) / Time.deltaTime;
        temp = (now_y - pre_y) / Time.deltaTime;
        if (temp != 0 || cnt_y == 0)
        {
            ac_y = temp;
            //Debug.Log(test_num);
            cnt_y = 40;
        }
        cnt_y--;
        if (cnt_y < 0)
        {
            cnt_y = 0;
        }

        //z축 각속도
        pre_z = now_z;
        now_z = rb.transform.rotation.eulerAngles.z;
        //Debug.Log(now_x);
        //float temp = System.Math.Min(now_x - pre_x, 360-now_x+pre_x) / Time.deltaTime;
        temp = (now_z - pre_z) / Time.deltaTime;
        if (temp != 0 || cnt_z == 0)
        {
            ac_z = temp;
            //Debug.Log(test_num);
            cnt_z = 40;
        }
        cnt_z--;
        if (cnt_z < 0)
        {
            cnt_z = 0;
        }
    }

    [PunRPC]
    public void setRacketPosition()
    {
        rb.position = originPosition;
    }

    [PunRPC]
    public void setFollow(bool temp){
        follow = temp;
    }
    public bool getFollow(){
        return follow;
    }

    //void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){}
}