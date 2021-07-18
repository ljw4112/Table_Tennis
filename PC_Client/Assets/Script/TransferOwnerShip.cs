using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
public class TransferOwnerShip : Photon.MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other){
        if(other.tag == "ball"){
            Debug.Log("ownership transfer");
            other.GetComponent<PhotonView>().RequestOwnership();
        }
    }
}
