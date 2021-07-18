using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;


public class PhotonInit : Photon.PunBehaviour
{
    void Awake(){
        PhotonNetwork.ConnectUsingSettings("1.0");
    }

    void Update(){
        
        
    }
    public override void OnJoinedLobby(){
        Debug.Log("Joined Lobby");
        PhotonNetwork.JoinRandomRoom();
    }
    
    // Update is called once per frame
    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg){
        Debug.Log("No Room");
        PhotonNetwork.CreateRoom("MyRoom");
    }

    public override void OnCreatedRoom(){
        Debug.Log("Finish make a room");
    }

    public override void OnJoinedRoom(){
        Debug.Log("Joined Room");
        StartCoroutine(this.CreatePlayer());
        //GameObject.Find("Canvas").SetActive(false);
    }

    void OnGUI(){
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }
    IEnumerator CreatePlayer()
    {

        Vector3 respawn_pos;
        Vector3 respawn_rot;
        string respawn_name;
        if (PhotonNetwork.isMasterClient)
        {
            respawn_name = "TT_Racket1";
            respawn_pos = new Vector3(0.7f, 12f, 21.96f);
            respawn_rot = new Vector3(67.146f, 184.14f, -1.916f);
            PhotonNetwork.Instantiate("TT_Ball", new Vector3(0f, 20f, 21.96f), Quaternion.Euler(0, 0, 0), 0);
        }
        else
        {
            respawn_name = "TT_Racket2";
            respawn_pos = new Vector3(-0.7f, 12f, -21.96f);
            respawn_rot = new Vector3(81.752f, 181.29f, 181.18f);
        }
        PhotonNetwork.Instantiate(respawn_name, respawn_pos, Quaternion.Euler(respawn_rot), 0);

        yield return null;
    }
}