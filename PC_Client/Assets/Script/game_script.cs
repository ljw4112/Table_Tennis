using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon;

public class game_script : Photon.MonoBehaviour
{
    private int masterScore;
    private int masterSetScore;
    private int guestScore;
    private int guestSetScore;
    private int result;

    public Text masterScore_txt;
    public Text guestScore_txt;
    public Text masterSetScore_txt;
    public Text guestSetScore_txt;
    public Text Set_Round;
    public Text WinText;
    public Text LoseText;
    public Text ServeText;

    public bool textFlag = false;

    // 1 : master win, 2 : guest win
    private int winner;
    private int serve;
    private int[] change_serve = { 0, 2, 1 };
    const string masterRacket = "master_racket";
    const string guestRacket = "guest_racket";
    const string masterPlace = "master_place";
    const string guestPlace = "guest_place";
    const string floor = "floor";
    // Start is called before the first frame update
    void Start()
    {
        masterScore = 0;
        masterSetScore = 0;
        guestScore = 0;
        guestSetScore = 0;
        serve = 1;
        winner = 0;
        result = 0;
        //PhotonNetwork.logLevel = PhotonLogLevel.Full;
    }

    int tmp = 0;

    // Update is called once per frame
    void Update()
    {
        if(GameObject.Find("TT_Racket1(Clone)") != null)
        {
            if (GameObject.Find("TT_Racket1(Clone)").GetComponent<PhotonView>().isMine && tmp == 0)
            {
                ServeText.fontSize = 30;
                Debug.Log(ServeText.fontSize);
                tmp++;
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            ServeText.fontSize = 1;
        }
    }

    [PunRPC]
    public void setStatus(int mS, int mSS, int gS, int gSS, int sv, int wn, int res){
        
        masterScore = mS;
        masterSetScore = mSS;
        guestScore = gS;
        guestSetScore = gSS;
        serve = sv;
        winner = wn;
        result = res;
        guestScore_txt.text = guestScore.ToString();
        masterScore_txt.text = masterScore.ToString();
        masterSetScore_txt.text = masterSetScore.ToString();
        guestSetScore_txt.text = guestSetScore.ToString();
        Set_Round.text = (masterSetScore + guestSetScore).ToString();
        Debug.Log("setresult called::: masterscore: " + masterScore + " mastersetscore: " + masterSetScore + " guestscore: " + guestScore + " guestsetscore: " + guestSetScore + " serve: " + serve + " winner: " + winner + " result: " + result);
    }

    [PunRPC]
    public void setResult(int mS, int mSS, int gS, int gSS, int sv, int wn, int res){
        
        masterScore = mS;
        masterSetScore = mSS;
        guestScore = gS;
        guestSetScore = gSS;
        serve = sv;
        winner = wn;
        result = res;
        guestScore_txt.text = guestScore.ToString();
        masterScore_txt.text = masterScore.ToString();
        masterSetScore_txt.text = masterSetScore.ToString();
        guestSetScore_txt.text = guestSetScore.ToString();
        Set_Round.text = (masterSetScore + guestSetScore + 1).ToString();
        Debug.Log("setresult called::: masterscore: " + masterScore + " mastersetscore: " + masterSetScore + " guestscore: " + guestScore + " guestsetscore: " + guestSetScore + " serve: " + serve + " winner: " + winner + " result: " + result);
    }
    public List<int> getResult(){
        List<int> ret = new List<int>();
        ret.Add(masterScore);
        ret.Add(masterSetScore);
        ret.Add(guestScore);
        ret.Add(guestSetScore);
        ret.Add(serve);
        ret.Add(winner);
        ret.Add(result);
        return ret;
    }
    
    public void gameFinishCheck(string pre, string now, int cnt)
    {
        Debug.Log("gameFinishCheck called::: pre: " + pre + " now: " + now + " cnt: " + cnt);
        // return 0 : 게임 속행, 1 : master win, 2 : guest win, 3 : master set win, 4 : guest set win, 5 : master play win, 6 : guest play win
        int ret = 0;
        if (cnt == 1)
        {
            if (serve == 1)
            {
                if (now == masterRacket)
                {
                    ret = 0;
                }
                else
                {
                    ret = 2;
                }
            }
            else
            {
                if (now == guestRacket)
                {
                    ret = 0;
                }
                else
                {
                    ret = 1;
                }
            }
        }
        else if (cnt == 2)
        {
            if (pre == masterRacket)
            {
                if (now == masterPlace)
                {
                    ret = 0;
                }
                else
                {
                    ret = 2;
                }
            }
            else
            {
                if (now == guestPlace)
                {
                    ret = 0;
                }
                else
                {
                    ret = 1;
                }
            }
        }
        else
        {
            if (now == floor)
            {
                if (pre == masterPlace || pre == masterRacket)
                {
                    ret = 2;
                }
                else
                {
                    ret = 1;
                }
            }
            else if (now == masterPlace)
            {
                if (pre == masterPlace || pre == masterRacket)
                {
                    ret = 2;
                }
            }
            else if (now == masterRacket)
            {
                if (pre == masterRacket)
                {
                    ret = 2;
                }
            }
            else if (now == guestPlace)
            {
                if (pre == guestPlace || pre == guestRacket)
                {
                    ret = 1;
                }
            }
            else if (now == guestRacket)
            {
                if (pre == guestRacket)
                {
                    ret = 1;
                }
            }
        }
        if (ret != 0)
        {
            if (ret == 1)
            {
                masterScore++;
            }
            else
            {
                guestScore++;
            }
        }
        // change serve
        if (ret!=0&& (masterScore + guestScore) % 2 == 0)
        {
            Debug.Log("serve is : " + serve);
            Debug.Log(masterScore + guestScore);
            serve = change_serve[serve];
            Debug.Log("serve is : " + serve);
        }
        // master win
        if (masterScore >= 3 && masterScore - guestScore >= 2)
        {            
            masterSetScore++;
            guestScore = masterScore = 0;
            ret = 3;
            if (masterSetScore == 2)
            {
                masterScore = 0;
                winner = 1;
                ServeText.fontSize = 1;
                GameObject.Find("Canvas").GetComponent<PhotonView>().RPC("FinishGame", PhotonTargets.All, masterSetScore, guestSetScore, winner);

                ret = 5;
            }
        }
        // guest win
        else if (guestScore >= 3 && guestScore - masterScore >= 2)
        {
            guestSetScore++;
            guestScore = masterScore = 0;
            ret = 4;
            if (guestSetScore == 2)
            {
                winner = 2;
                ServeText.fontSize = 1;
                GameObject.Find("Canvas").GetComponent<PhotonView>().RPC("FinishGame", PhotonTargets.All, masterSetScore, guestSetScore, winner);

                ret = 6;
            }
        }

        result = ret;
        //return ret;
    }

    public int whoServe()
    {
        return serve;
    }
    public int getMasterScore()
    {
        return masterScore;
    }
    public int getGuestScore()
    {
        return guestScore;
    }
    public int getMasterSetScore()
    {
        return masterSetScore;
    }
    public int getGuestSetScore()
    {
        return guestSetScore;
    }
    public int getWinner()
    {
        return winner;
    }
    public int getServe()
    {
        return serve;
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
        
    }

    [PunRPC]
    public void setServeText()
    {
        if (!textFlag)
        {
            Debug.Log("Racket1 is Mine? : " + GameObject.Find("TT_Racket1(Clone)").GetComponent<PhotonView>().isMine);
            if (GameObject.Find("TT_Racket1(Clone)").GetComponent<PhotonView>().isMine)
            {
                if (serve == 1)
                {
                    ServeText.fontSize = 30;
                }
            }
            else
            {
                Debug.Log("Serve : " + serve);
                if (serve == 2)
                {
                    ServeText.fontSize = 30;
                }
            }
        } 
    }

    [PunRPC]
    public void FinishGame(int ms, int gs, int winner)
    {
        textFlag = true;
        if(winner == 1)
        {
            if (GameObject.Find("TT_Racket1(Clone)").GetComponent<PhotonView>().isMine)
            {
                WinText.text = ms + " VS "  + gs + "\n" + "You win!";
                WinText.fontSize = 30;
            }
            else
            {
                LoseText.text = gs + " VS " + ms + "\n" + "You lose!";
                LoseText.fontSize = 30;
            }
        }
        else
        {
            if (GameObject.Find("TT_Racket1(Clone)").GetComponent<PhotonView>().isMine)
            {
                LoseText.text = ms + " VS " + gs + "\n" + "You lose!";
                LoseText.fontSize = 30;
            }
            else
            {
                WinText.text = gs + " VS " + ms + "\n" + "You win!";
                WinText.fontSize = 30;
            }
        }
        Time.timeScale = 0.1f;
        Invoke("loadMainLevel", 0.5f);
    }

    void loadMainLevel()
    {
        SceneManager.LoadScene("MainScene");
    }
}