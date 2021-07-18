using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rotate_Revision : MonoBehaviour
{
    private Racket_script rs;

    private Vector3 rotate_plain = new Vector3(0, 180, 0);                   //휴대폰을 땅에 대고 있을 때의 각도
    private Quaternion rotation_data;
    private ScreenUI su;
    public float[] revision_data;

    // Start is called before the first frame update
    void Start()
    {
        su = GameObject.Find("Canvas").GetComponent<ScreenUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setData()
    {
        revision_data = su.getData();
    }
}
