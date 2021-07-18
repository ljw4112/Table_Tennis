using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using System;

using UnityEngine.UI;
//using static Receive_test1;

public class ball_script : Photon.PunBehaviour
{
    Renderer capsuleColor;
    Rigidbody rb;

    bool flag;
    bool floorFlag;

    private RaycastHit hit;
    private Vector3 position;
    private GameObject myRacket;
    private GameObject yourRacket;

    public float x, y, z;
    private float a, b, c;
    public float magnesConst = 0.05f;
    public float addFConst = 1000f;
    public float netV = 50f;
    float cnt = 0;
    float floorCnt = 0;
    private string preCol;
    private string nowCol;
    public int colCnt;

    private bool netFlag;

    private float pre_z;

    private Vector3 currPos;
    private Quaternion currRot;
    private PhotonView pv;

    public AudioSource audioSource;
    public AudioClip[] audioClip;

    public Text serveText;

    private int ownerflag = 0;
    //private Vector3 currVelo;
    // Start is called before the first frame update
    void Start()
    {
        capsuleColor = gameObject.GetComponent<Renderer>();
        rb = gameObject.GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();
        // pv.ObservedComponents[0] = this;


        a = rb.position.x;
        b = rb.position.y;
        c = rb.position.z;
        if (PhotonNetwork.isMasterClient)
        {
            //Debug.Log("ball is mine");
            myRacket = GameObject.Find("TT_Racket1(Clone)");
            yourRacket = GameObject.Find("TT_Racket2(Clone)");
        }
        else
        {
            //Debug.Log("ball is not mine");
            myRacket = GameObject.Find("TT_Racket2(Clone)");
            yourRacket = GameObject.Find("TT_Racket1(Clone)");
        }
        //rb.useGravity = false;
        pv.RPC("setgravity", PhotonTargets.All, false);
        flag = true;
        floorFlag = true;
        preCol = "";
        nowCol = "";
        colCnt = 0;
        netFlag = true;

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pv.isMine && !rb.useGravity)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            if (FindObjectOfType<game_script>().getServe() == 1)
            {
                rb.position = new Vector3(0f, 20f, 21.96f);
            }
            else
            {
                rb.position = new Vector3(0f, 20f, -21.96f);
            }
        }


        if (PhotonNetwork.isMasterClient)
        {
            if (yourRacket == null)
                yourRacket = GameObject.Find("TT_Racket2(Clone)");
        }
        else
        {
            if (yourRacket == null)
                yourRacket = GameObject.Find("TT_Racket1(Clone)");
        }

        // 초기 상태로 되돌리기
        if (Input.GetKeyDown(KeyCode.F) || rb.position.y < 0)
        {
            //Debug.Log("input w");
            rb.position = new Vector3(a, b, c);
            if (this.photonView.ownerId == 1)
                rb.velocity = new Vector3(0, -1, 1) * 15f;
            else
                rb.velocity = new Vector3(0, -1, -1) * 15f;
            // rb.velocity = new Vector3(0,1,0);
            rb.angularVelocity = new Vector3(0, 0, 0);
            ownerflag = 0;
        }
        if (Input.GetKeyDown(KeyCode.Z) && pv.isMine)
        {
            //rb.useGravity = true;
            pv.RPC("setgravity", PhotonTargets.All, true);
        }
        // rb.velocity = currVelo;
    }
    private void FixedUpdate()
    {
        if (rb.useGravity && pv.isMine)
        {
            if (pre_z >= 0 && rb.position.z <= 0)
            { // 마스터->게스트 영역
                pv.TransferOwnership(2);
                //myRacket.GetComponent<PhotonView>().RPC("setFollow", PhotonTargets.All, true);
            }
            else if (pre_z <= 0 && rb.position.z >= 0)
            {
                pv.TransferOwnership(1);
                //myRacket.GetComponent<PhotonView>().RPC("setFollow", PhotonTargets.All, true);
            }
        }


        // 마그누스 효과
        //float velocityMag = rb.velocity.magnitude;
        //Vector3 velocityDir = rb.velocity.normalized;
        //float airDragMag = 0.5f * Mathf.Pow(velocityMag, 2f);
        //rb.AddRelativeForce(airDragMag * -velocityDir);

        // 탁구채와의 충돌을 위한 60프레임 지나기
        if (cnt > 0)
        {
            cnt--;
        }
        if (cnt == 0)
        {
            flag = true;
        }
        if (floorCnt > 0)
        {
            floorCnt--;
        }
        if (floorCnt == 0)
        {
            floorFlag = true;
        }
        pre_z = rb.position.z;

    }

    public void ownershipto1()
    {
        this.GetComponent<PhotonView>().TransferOwnership(1);
        // pv.RPC("setVelo", PhotonTargets.Others, rb.velocity, rb.angularVelocity);


    }

    public void ownershipto2()
    {
        this.GetComponent<PhotonView>().TransferOwnership(2);
        // pv.RPC("setVelo", PhotonTargets.Others, rb.velocity, rb.angularVelocity);

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("net"))
        {
            if (netFlag)
            {
                if (rb.position.y <= 9.3)
                {
                    // Debug.Log("낮아서 네트에 걸림");
                    rb.velocity = new Vector3(0, 0, 0);
                    rb.AddForce(new Vector3(0, -100, -100));
                    netFlag = false;
                }
                else
                {
                    if (Math.Abs(rb.velocity.x) < netV * ((0.19 + 9.3 - rb.position.y) / 0.19))
                    {
                        Debug.Log("느려서 네트에 걸림");
                        rb.velocity *= -1;
                        rb.velocity /= 3;
                        netFlag = false;
                    }
                }
            }
        }
        // 탁구채 충돌
        else if (flag)
        {
            audioSource.clip = audioClip[0];
            audioSource.Play();
            if (pv.isMine)
            {
                flag = false;

                //GameObject touchRacket = other.gameObject;
                //rb.velocity = Vector3.Reflect(rb.velocity, touchRacket.transform.up);
                // rb.AddForce(touchRacket.transform.up * addFConst);
                // flag = false;
                // cnt = 60;

                myRacket.GetComponent<PhotonView>().RPC("setFollow", PhotonTargets.All, false);
                yourRacket.GetComponent<PhotonView>().RPC("setFollow", PhotonTargets.All, false);

                if (other.GetComponent<PhotonView>().ownerId == 1 && PhotonNetwork.isMasterClient)
                { // 내가 마스터이고 마스터 라켓에 공이 닿았을 때
                    preCol = nowCol;
                    nowCol = "master_racket";
                    colCnt++;

                    //로컬에서 계산
                    GameObject.Find("Canvas").GetComponent<game_script>().gameFinishCheck(preCol, nowCol, colCnt);
                    //변수 동기화
                    List<int> t = GameObject.Find("Canvas").GetComponent<game_script>().getResult();
                    int colResult = t[6];
                    GameObject.Find("Canvas").GetComponent<PhotonView>().RPC("setStatus", PhotonTargets.All, t[0], t[1], t[2], t[3], t[4], t[5], t[6]);
                    pv.RPC("updatecol", PhotonTargets.All, preCol, nowCol, colCnt, flag, floorFlag, netFlag);


                    // Debug.Log("colresult" + colResult);
                    // play end
                    if (colResult > 4)
                    {
                        // 대전 종료 코드
                    }
                    // set end
                    else if (colResult > 2)
                    {
                        // 게임 다시 시작하는 코드
                        reGame(FindObjectOfType<game_script>().getServe());
                        //rb.useGravity = false;
                        pv.RPC("setgravity", PhotonTargets.All, false);
                    }
                    // game end
                    else if (colResult > 0)
                    {
                        // 게임 다시 시작하는 코드
                        reGame(FindObjectOfType<game_script>().getServe());
                        //rb.useGravity = false;
                        pv.RPC("setgravity", PhotonTargets.All, false);
                    }

                    GameObject touchRacket = other.gameObject;
                    rb.velocity = Vector3.Reflect(rb.velocity, touchRacket.transform.up) * 0.9f;
                    rb.AddForce(touchRacket.transform.up * addFConst);
                    //flag = false;
                    cnt = 60;

                    //Invoke("ownershipto2", 0.1f);
                    //this.GetComponent<PhotonView>().TransferOwnership(2);
                    // Debug.Log("ownership transfer master to guest");


                }
                else if (other.GetComponent<PhotonView>().ownerId == 2 && !PhotonNetwork.isMasterClient)
                {
                    preCol = nowCol;
                    nowCol = "guest_racket";
                    colCnt++;

                    //로컬에서 계산
                    GameObject.Find("Canvas").GetComponent<game_script>().gameFinishCheck(preCol, nowCol, colCnt);
                    //변수 동기화
                    List<int> t = GameObject.Find("Canvas").GetComponent<game_script>().getResult();
                    int colResult = t[6];
                    GameObject.Find("Canvas").GetComponent<PhotonView>().RPC("setStatus", PhotonTargets.All, t[0], t[1], t[2], t[3], t[4], t[5], t[6]);
                    pv.RPC("updatecol", PhotonTargets.All, preCol, nowCol, colCnt, flag, floorFlag, netFlag);

                    // play end
                    if (colResult > 4)
                    {
                        // 대전 종료 코드
                    }
                    // set end
                    else if (colResult > 2)
                    {
                        // 게임 다시 시작하는 코드
                        reGame(FindObjectOfType<game_script>().getServe());
                        //rb.useGravity = false;
                        pv.RPC("setgravity", PhotonTargets.All, false);
                    }
                    // game end
                    else if (colResult > 0)
                    {
                        // 게임 다시 시작하는 코드
                        reGame(FindObjectOfType<game_script>().getServe());
                        //rb.useGravity = false;
                        pv.RPC("setgravity", PhotonTargets.All, false);
                    }

                    GameObject touchRacket = other.gameObject;
                    rb.velocity = Vector3.Reflect(rb.velocity, touchRacket.transform.up) * 0.9f;
                    rb.AddForce(touchRacket.transform.up * addFConst);
                    //flag = false;
                    cnt = 60;

                    //Invoke("ownershipto1", 0.1f);
                    //this.GetComponent<PhotonView>().TransferOwnership(1);
                    Debug.Log("ownership transfer guest to master");

                }
            }
        }
    }

    private void OnCollisionExit(Collision col)
    {
        if (floorFlag)
        {
            audioSource.clip = audioClip[1];
            audioSource.Play();

            floorFlag = false;
            floorCnt = 20;
            if (pv.isMine)
            {
                // 반대 벽 충돌
                if (col.gameObject.CompareTag("Finish"))
                {
                    // Debug.Log("collision with wall");
                    rb.AddForce(new Vector3(0, 0, 1) * -1 * addFConst);
                    // Debug.Log(rb.velocity.magnitude);
                }
                else if (col.gameObject.CompareTag("floor"))
                {
                    preCol = nowCol;
                    nowCol = "floor";
                    colCnt++;


                    //로컬에서 계산
                    GameObject.Find("Canvas").GetComponent<game_script>().gameFinishCheck(preCol, nowCol, colCnt);
                    //변수 동기화
                    List<int> t = GameObject.Find("Canvas").GetComponent<game_script>().getResult();
                    int colResult = t[6];
                    GameObject.Find("Canvas").GetComponent<PhotonView>().RPC("setStatus", PhotonTargets.All, t[0], t[1], t[2], t[3], t[4], t[5], t[6]);
                    pv.RPC("updatecol", PhotonTargets.All, preCol, nowCol, colCnt, flag, floorFlag, netFlag);

                    Debug.Log("바닥닿을때 colResult:" + colResult);
                    if (colResult > 4)
                    {
                        // 대전 종료 코드
                    }
                    // set end
                    else if (colResult > 2)
                    {
                        // 게임 다시 시작하는 코드
                        reGame(FindObjectOfType<game_script>().getServe());
                        //rb.useGravity = false;
                        pv.RPC("setgravity", PhotonTargets.All, false);
                    }
                    // game end
                    else if (colResult > 0)
                    {
                        // 게임 다시 시작하는 코드
                        reGame(FindObjectOfType<game_script>().getServe());
                        //rb.useGravity = false;
                        pv.RPC("setgravity", PhotonTargets.All, false);
                    }
                }
                else if (col.gameObject.CompareTag("masterPlace"))
                {
                    //audioSource.clip = audioClip[1];
                    //audioSource.Play();
                    preCol = nowCol;
                    nowCol = "master_place";
                    colCnt++;


                    //로컬에서 계산
                    GameObject.Find("Canvas").GetComponent<game_script>().gameFinishCheck(preCol, nowCol, colCnt);
                    //변수 동기화
                    List<int> t = GameObject.Find("Canvas").GetComponent<game_script>().getResult();
                    int colResult = t[6];
                    GameObject.Find("Canvas").GetComponent<PhotonView>().RPC("setStatus", PhotonTargets.All, t[0], t[1], t[2], t[3], t[4], t[5], t[6]);
                    pv.RPC("updatecol", PhotonTargets.All, preCol, nowCol, colCnt, flag, floorFlag, netFlag);

                    //Debug.Log("In masterplace colResult:" + colResult);
                    // play end
                    if (colResult > 4)
                    {
                        // 대전 종료 코드
                    }
                    // set end
                    else if (colResult > 2)
                    {
                        // 게임 다시 시작하는 코드
                        reGame(FindObjectOfType<game_script>().getServe());
                        //rb.useGravity = false;
                        pv.RPC("setgravity", PhotonTargets.All, false);
                    }
                    // game end
                    else if (colResult > 0)
                    {
                        // 게임 다시 시작하는 코드
                        reGame(FindObjectOfType<game_script>().getServe());
                        //rb.useGravity = false;
                        pv.RPC("setgravity", PhotonTargets.All, false);
                    }
                }
                else if (col.gameObject.CompareTag("guestPlace"))
                {
                    //audioSource.clip = audioClip[1];
                    //audioSource.Play();
                    preCol = nowCol;
                    nowCol = "guest_place";
                    colCnt++;


                    //로컬에서 계산
                    GameObject.Find("Canvas").GetComponent<game_script>().gameFinishCheck(preCol, nowCol, colCnt);
                    //변수 동기화
                    List<int> t = GameObject.Find("Canvas").GetComponent<game_script>().getResult();
                    int colResult = t[6];
                    GameObject.Find("Canvas").GetComponent<PhotonView>().RPC("setStatus", PhotonTargets.All, t[0], t[1], t[2], t[3], t[4], t[5], t[6]);
                    pv.RPC("updatecol", PhotonTargets.All, preCol, nowCol, colCnt, flag, floorFlag, netFlag);

                    //Debug.Log("In guestplace colResult:" + colResult);
                    // play end
                    if (colResult > 4)
                    {
                        // 대전 종료 코드
                    }
                    // set end
                    else if (colResult > 2)
                    {
                        // 게임 다시 시작하는 코드
                        reGame(FindObjectOfType<game_script>().getServe());

                        pv.RPC("setgravity", PhotonTargets.All, false);
                        //rb.useGravity = false;
                    }
                    // game end
                    else if (colResult > 0)
                    {
                        // 게임 다시 시작하는 코드
                        reGame(FindObjectOfType<game_script>().getServe());
                        //rb.useGravity = false;
                        pv.RPC("setgravity", PhotonTargets.All, false);
                    }
                }
            }
        }
    }
    public Vector3 getposition()
    {
        return rb.position;
    }
    public Quaternion getrotation()
    {
        return rb.rotation;
    }

    [PunRPC]
    void setVelo(Vector3 velo, Vector3 angularvelo)
    {
        rb.velocity = velo;
        rb.angularVelocity = angularvelo;
    }

    [PunRPC]

    void setgravity(bool usegravity)
    {
        rb.useGravity = usegravity;
        //Debug.Log(usegravity + "로 전환");
    }
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
    [PunRPC]
    void updatecol(string pCol, string nCol, int cCnt, bool f, bool ff, bool fff)
    {
        preCol = pCol;
        nowCol = nCol;
        colCnt = cCnt;
        flag = f;
        floorFlag = ff;
        netFlag = fff;
    }

    public void reGame(int serve)
    {
        //myRacket.GetComponent<Racket_script>().setFollow(false);
        //yourRacket.GetComponent<Racket_script>().setFollow(false);
        myRacket.GetComponent<PhotonView>().RPC("setFollow", PhotonTargets.All, false);
        yourRacket.GetComponent<PhotonView>().RPC("setFollow", PhotonTargets.All, false);

        myRacket.GetComponent<PhotonView>().RPC("setRacketPosition", PhotonTargets.All);
        yourRacket.GetComponent<PhotonView>().RPC("setRacketPosition", PhotonTargets.All);
        //myRacket.GetComponent<Racket_script>().setRacketPosition();
        //yourRacket.GetComponent<Racket_script>().setRacketPosition();
        //Debug.Log("re game");
        pv.RPC("updatecol", PhotonTargets.All, "", "", 0, true, true, true);

        Debug.Log("Serve : " + serve);
        if (serve == 1)
        {
            /*
            var ball = GameObject.Find("TT_Ball(Clone)");
            if (ball)
            {
                PhotonNetwork.Destroy(GameObject.Find("TT_Ball(Clone)").GetComponent<PhotonView>());
                PhotonNetwork.Instantiate("TT_Ball", new Vector3(0.11f, 20.23f, 17.36f), Quaternion.Euler(0, 0, 0), 0);
            }
            */
            var ball = GameObject.Find("TT_Ball(Clone)");
            ball.transform.position = new Vector3(0f, 20f, 21.96f);
            ball.transform.rotation = Quaternion.Euler(0, 0, 0);
            ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
            ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            //ball.GetComponent<PhotonView>().TransferOwnership(1);
            Invoke("ownershipto1", 0.3f);



            GameObject.Find("Canvas").GetComponent<PhotonView>().RPC("setServeText", PhotonTargets.All);
            /*
            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("난 마스터, 서브 넣는다.");
                GameObject.Find("Canvas").GetComponent<game_script>().setServeText();
            }*/

        }
        else
        {
            /*
            var ball = GameObject.Find("TT_Ball(Clone)");
            if (ball)
            {
                PhotonNetwork.Destroy(GameObject.Find("TT_Ball(Clone)").GetComponent<PhotonView>());
                PhotonNetwork.Instantiate("TT_Ball", new Vector3(0f, 14.5f, -16.6f), Quaternion.Euler(0, 0, 0), 0);
            }
            */
            Debug.Log("serve==2 실행됨1");
            var ball = GameObject.Find("TT_Ball(Clone)");
            ball.transform.position = new Vector3(0f, 20f, -21.96f);
            ball.transform.rotation = Quaternion.Euler(0, 0, 0);
            ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
            ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            //ball.GetComponent<PhotonView>().TransferOwnership(2);
            Invoke("ownershipto2", 0.3f);

            Debug.Log("serve==2 실행됨2");
            GameObject.Find("Canvas").GetComponent<PhotonView>().RPC("setServeText", PhotonTargets.All);
            //if (!PhotonNetwork.isMasterClient)
            //{
            //    Debug.Log("난 게스트, 서브 넣는다.");
            //    GameObject.Find("Canvas").GetComponent<game_script>().setServeText();
            //}
        }
    }
}