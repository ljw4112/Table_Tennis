using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Gravitons.UI.Modal;
using UnityEngine.SceneManagement;

public class ScreenUI : MonoBehaviour
{
    public Slider Volume_Slider;
    public AudioSource audioSource;
    public AudioClip btn_click_sound, Back_button_sound;
    public Button fullScreen, fullScreenWindow, Window;
    public Button gameStart, Practice, randomRacket;

    public RectTransform main, game_start, connect, option, score;
    public bool isStart, isConnect, isOption, isScore;                  //PANEL을 나누는 변수

    private GameObject popup_Panel;
    private Button gameStart_btn;
    private Button connect_btn;
    private Button option_btn;
    private Button score_btn;
    private Button back_btn;
    private Text connect_time_text;
    private Text connect_Label;

    private Connect_Device cd;
    private ScoreReading sc;

    public bool isConnected = false;                                //디바이스와 연관되어 있는지 확인
    private bool firstStart = true, firstOption = true;                    //게임을 키고 처음 실행했는지 확인 (스레드 중복처리를 위해)

    private float x, y, z, dx, dy, dz;
    public float[] returnData;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(GameObject.Find("Network"));
        popup_Panel = GameObject.Find("ModalManager");
        gameStart_btn = GameObject.Find("GameStart_btn").GetComponent<Button>();
        connect_btn = GameObject.Find("Connect_btn").GetComponent<Button>();
        option_btn = GameObject.Find("Option_btn").GetComponent<Button>();
        score_btn = GameObject.Find("Score_btn").GetComponent<Button>();
        back_btn = GameObject.Find("Back_Button").GetComponent<Button>();
        Volume_Slider = GameObject.Find("Volume_Slider").GetComponent<Slider>();
        audioSource = GetComponent<AudioSource>();
        connect_time_text = GameObject.Find("Connect_Time_Label").GetComponent<Text>();
        connect_Label = GameObject.Find("Connect_Time_label").GetComponent<Text>();

        gameStart_btn.onClick.AddListener(onClick_gameStart);
        connect_btn.onClick.AddListener(onClick_connect);
        option_btn.onClick.AddListener(onClick_option);
        score_btn.onClick.AddListener(onClick_score);
        back_btn.onClick.AddListener(onClick_Back);
        fullScreen.onClick.AddListener(delegate { setWindow(fullScreen); });
        fullScreenWindow.onClick.AddListener(delegate { setWindow(fullScreenWindow); });
        Window.onClick.AddListener(delegate { setWindow(Window); });
        gameStart.onClick.AddListener(scene_gameStart);
        Practice.onClick.AddListener(scene_Practice);
        randomRacket.onClick.AddListener(scene_randomRacket);

        back_btn.gameObject.SetActive(false);
        isStart = isConnect = isOption = isScore = false;

        audioSource.clip = btn_click_sound;

        popup_Panel.transform.position = new Vector2(Screen.width / 2, Screen.height / 2);
        sc = score.GetComponent<ScoreReading>();

        //초기 위치지정
        game_start.offsetMax = new Vector2(-Screen.width, 0);
        connect.anchoredPosition = new Vector2(0, -Screen.height);
        score.offsetMax = new Vector2(Screen.width, 0);
        option.anchoredPosition = new Vector2(0, Screen.height);
        back_btn.transform.position = new Vector2(-Screen.width / 12 + 200, Screen.height - 100);

        returnData = new float[3];

        sc = new ScoreReading();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void onClick_gameStart()
    {
        audioSource.Play();
      //  if (!isConnected)
       // {
          //  ModalManager.Show("Not connected", "Game is not connected to mobile device!", new[] { new ModalButton() { Text = "OK" } });

      //  }
       // else
        {
            main.DOAnchorPos(new Vector2(-Screen.width, 0), 0.25f);
            game_start.DOAnchorPos(Vector2.zero, 0.25f);
            isStart = true;
            back_btn.gameObject.SetActive(true);
        }
    }

    void scene_gameStart()
    {
        SceneManager.LoadScene("Play");
    }

    void scene_Practice()
    {
        SceneManager.LoadScene("Practice");
    }

    void scene_randomRacket()
    {

    }

    void onClick_connect()
    {
        connect_time_text.gameObject.SetActive(false);
        connect_Label.gameObject.SetActive(false);
        audioSource.Play();
        if (!isConnected)
        {
            main.DOAnchorPos(new Vector2(0, -Screen.height), 0.25f);
            connect.DOAnchorPos(Vector2.zero, 0.25f);

            if (firstStart)
            {
                cd = GameObject.Find("Network").GetComponent<Connect_Device>();
                cd.startThread();
            }
            ModalManager.Show("Connecting", "연결을 시작합니다. OK버튼을 누른 후 모바일 앱을 켜주세요.", new[] { new ModalButton() { Text = "OK", Callback = startCoroutine } });
        }
        else
        {
            gameStart_btn.interactable = false;
            connect_btn.interactable = false;
            option_btn.interactable = false;
            score_btn.interactable = false;
            ModalManager.Show("Connected", "Game is connected to mobile device already!", new[] { new ModalButton() { Text = "OK", Callback = reInteractive } });
        }

        if (firstStart)
        {
            firstStart = !firstStart;
        }
    }

    void startCoroutine()
    {
        StartCoroutine("timerCoroutine");
    }

    IEnumerator timerCoroutine()
    {
        float time = 0f;
        connect_time_text.gameObject.SetActive(true);
        connect_Label.gameObject.SetActive(true);
        while (true)
        {
            isConnected = cd.isConnect();
            time += Time.deltaTime;
            connect_time_text.text = "Time Left : " + ((int)time).ToString();
            if (time >= 30f)
            {
                ModalManager.Show("Connecting Failed", "연결실패. 메인화면으로 돌아갑니다.", new[] { new ModalButton() { Text = "OK", Callback = onClick_Back } });
                connect_time_text.gameObject.SetActive(false);
                connect_Label.gameObject.SetActive(false);
                break;
            }
            if (isConnected)
            {
                ModalManager.Show("Connecting successed", "연결성공. 보정 후 메인화면으로 이동됩니다..", new[] { new ModalButton() { Text = "OK", Callback = start_revision } });
                time = 0f;
                connect_time_text.gameObject.SetActive(false);
                connect_Label.gameObject.SetActive(false);
                break;
            }
            yield return null;
        }
    }

    private void start_revision()
    {
        ModalManager.Show("Angle Revision", "각도보정을 시작합니다. \n다음과 같이 휴대폰 머리가 모니터를 가리키도록 해주세요. ", new[] { new ModalButton() { Text = "OK", Callback = revision } });
    }

    private void revision()
    {
        Quaternion rot = cd.getData();
        Vector3 rotate_plain = new Vector3(0, 180, 0);

        x = rot.eulerAngles.x;
        y = rot.eulerAngles.y;
        z = rot.eulerAngles.z;

        dx = x - rotate_plain.x;
        dy = y - rotate_plain.y;
        dz = z - rotate_plain.z;

        returnData[0] = dx;
        returnData[1] = dy;
        returnData[2] = dz;
        isConnect = true;

        Rotate_Revision rr = GameObject.Find("Network").GetComponent<Rotate_Revision>();
        rr.setData();

        ModalManager.Show("Angle Revision Ended", "메인화면으로 돌아갑니다. ", new[] { new ModalButton() { Text = "OK", Callback = onClick_Back } });
    }

    public float[] getData()
    {
        return returnData;
    }

    void onClick_option()
    {
        audioSource.Play();
        main.DOAnchorPos(new Vector2(0, Screen.height), 0.25f);
        option.DOAnchorPos(Vector2.zero, 0.25f);
        isOption = true;
        back_btn.gameObject.SetActive(true);
        Volume_Slider.value = audioSource.volume;

        Volume_Slider.onValueChanged.AddListener(delegate { setVolume(Volume_Slider.value); });
    }

    void onClick_score()
    {
        Text result_label = GameObject.Find("result_Label").GetComponent<Text>();
        Text time_label = GameObject.Find("time_Label").GetComponent<Text>();
        Text score_label = GameObject.Find("score_Label").GetComponent<Text>();

        audioSource.Play();
        main.DOAnchorPos(new Vector2(Screen.width, 0), 0.25f);
        score.DOAnchorPos(Vector2.zero, 0.25f);
        isScore = true;
        back_btn.gameObject.SetActive(true);

        
        ArrayList data = sc.getScoreData();

        if (firstOption)
        {
            foreach (string i in data)
            {
                string[] parse = i.Split('|');
                result_label.text += parse[0] + '\n';
                time_label.text += parse[1] + '\n';
                score_label.text += parse[2] + '\n';
            }

            firstOption = !firstOption;
        }
    }

    void onClick_Back()
    {
        audioSource.Play();
        if (isStart)
        {
            game_start.DOAnchorPos(new Vector2(-Screen.width, 0), 0.25f);
            isStart = false;
        }
        else if (isOption)
        {
            option.DOAnchorPos(new Vector2(0, Screen.height), 0.25f);
            isOption = false;
        }
        else if (isScore)
        {
            score.DOAnchorPos(new Vector2(Screen.width, 0), 0.25f);
            isScore = false;
        }
        else if (isConnect)
        {
            connect_time_text.gameObject.SetActive(false);
            connect.DOAnchorPos(new Vector2(0, -Screen.height), 0.25f);
            isConnect = false;
        }
        main.DOAnchorPos(new Vector2(0, 0), 0.25f);
        back_btn.gameObject.SetActive(false);
    }

    private void setVolume(float sliderValue)
    {
        audioSource.volume = sliderValue;
    }

    private void setWindow(Button b)
    {
        audioSource.Play();
        if (b == fullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else if (b == fullScreenWindow)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else if (b == Window)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Screen.SetResolution(1100, 619, false);
        }
    }

    private void reInteractive()
    {
        gameStart_btn.interactable = true;
        connect_btn.interactable = true;
        option_btn.interactable = true;
        score_btn.interactable = true;
    }
}
