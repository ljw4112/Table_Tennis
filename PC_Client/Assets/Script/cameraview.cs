using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraview : MonoBehaviour
{
    public Transform target;
    public float dist = 1.0f;
    public float height = 0.5f;

    private Transform me;
    void Start()
    {
        me = Camera.main.GetComponent<Transform>();
        if (PhotonNetwork.isMasterClient)
        {
            me.position = new Vector3(1.32f, 18.94f, 40.63f);
            me.rotation = Quaternion.Euler(3f, 180f, 0f);
        }
        else
        {
            me.position = new Vector3(1.32f, 18.94f, -40.63f);
            me.rotation = Quaternion.Euler(3f, 0, 0f);
        }

        /*
        me = GetComponent<Transform>();
        Quaternion rot = Quaternion.Euler(-me.rotation.x, me.rotation.y, me.rotation.z);
        if(PhotonNetwork.isMasterClient)
            me.position = target.position - (rot * Vector3.forward * dist) + ( Vector3.up *height);
        else
            me.position = target.position + (rot * Vector3.forward * dist) + ( Vector3.up *height);
        me.LookAt(target); 
        */
    }

    // Update is called once per frame
    void LateUpdate()
    {
       
    }
}