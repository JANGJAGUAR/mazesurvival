using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro; 
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private readonly string version = "1.0";
    private string userId = "Chris";                // 편의상 user ID 고정, 이후 변경해야 함
    public int winNum; 
    public int loseNum; 
    public float winPCT; 


    public TMP_InputField userIF;
    public TMP_Text HasRecord;
    public TMP_Text Stat;
    public TMP_Text T_Nick;

    //private int StatClickNum = 0;    //버튼 클릭 횟수(짝수 번째에는 Statistics panel 비활성화, 홀수 번째에는 활성화)
    //private int CreditClickNum = 0;
    //private int HowToPlayClickNum = 0;

    public GameObject statPanel;
    public GameObject creditPanel;
    public GameObject how2PlayPanel;
    public GameObject nickSavePanel;
    bool UI_on;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = version;
        PhotonNetwork.NickName = userId;
        PhotonNetwork.ConnectUsingSettings();
    }

    void Start(){
        statPanel = GameObject.Find("Canvas").transform.Find("Panel_Statistics").gameObject;
        creditPanel = GameObject.Find("Canvas").transform.Find("Panel_Credits").gameObject;
        how2PlayPanel = GameObject.Find("Canvas").transform.Find("Panel_HowToPlay").gameObject;
        nickSavePanel = GameObject.Find("Canvas").transform.Find("Panel_NickSave").gameObject;

        statPanel.SetActive(false);
        creditPanel.SetActive(false);
        how2PlayPanel.SetActive(false);
        nickSavePanel.SetActive(false);

        UI_on = false;


        if(PlayerPrefs.HasKey("user_id")){      //저장된 통계를 불러옴 
            userId = PlayerPrefs.GetString("user_id");
            PhotonNetwork.NickName = userId;
 
            winNum = PlayerPrefs.GetInt("winNum");    
            loseNum = PlayerPrefs.GetInt("loseNum");
            winPCT = PlayerPrefs.GetFloat("winPCT");
        }

        else{
            GameObject.Find("Canvas").transform.Find("Panel_NickSave").gameObject.SetActive(true);  //저장된 정보가 없으면 닉네임 만들기부터 시작 
            userId = PlayerPrefs.GetString("user_id", $"user_{Random.Range(1,3)}");   //닉네임을 정하지 않고 save를 누르면 게임이 자동으로 닉네임 생성
            PhotonNetwork.NickName = userId;  //닉네임 저장 (저장 이후부턴 더 이상 닉네임을 바꿀 수 없음)

            winNum = 0;    //아직 게임 정보가 없으므로 모두 0
            loseNum = 0;
            winPCT = 0.0f;   
            PlayerPrefs.SetInt("winNum", winNum);
            PlayerPrefs.SetInt("loseNum", loseNum);
            PlayerPrefs.SetFloat("winPCT", winPCT);
        
        }
    }

    public void SetUserId(){
        if(string.IsNullOrEmpty(userIF.text)){       //닉네임 save 버튼에 적용될 함수
            userId = $"user_{Random.Range(1,3)}";
        }
        else{
            userId = userIF.text;
        }
        PlayerPrefs.SetString("user_id", userId);
        PhotonNetwork.NickName = userId; 
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 2;
        ro.IsOpen = true;
        ro.IsVisible = true;

        PhotonNetwork.CreateRoom("Room1", ro);
    }

    public override void OnJoinedRoom()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Final_InGame");
            //PhotonNetwork.LoadLevel("Re_InGame");
            //PhotonNetwork.LoadLevel("InGame");
        }
    }

#region  UI_BUTTON_EVENT
    public void OnExittClick()
    {
        //if(!UI_on)
        Application.Quit();
    }

    public void OnLoginClick(){
        SetUserId();
        nickSavePanel.SetActive(false);
    }

    public void OnStartClick()
    {
        //if(!UI_on)
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnStatisticsClick(){
        //다른 panel을 닫지 않은 상태에서 Statistics panel을 열면 다른 panel을 자동으로 닫음 
        creditPanel.SetActive(false);
        how2PlayPanel.SetActive(false);
        nickSavePanel.SetActive(false);
        statPanel.SetActive(true);

        T_Nick.text = $"<color=#fbbb0e>{userId}</color>님의 플레이어 정보";

        float roundWinPCT = Mathf.Round(winPCT*10)*0.1f;

        if(PlayerPrefs.HasKey("winPCT")){
            HasRecord.text = $"플레이한 게임 : {winNum+loseNum}판";
            if(roundWinPCT >= 65)
                Stat.text = $"승 : {winNum} \n패 : {loseNum} \n승률 : <color=#50bcdf>{roundWinPCT}%</color>";
            else if(roundWinPCT >= 55)
                Stat.text = $"승 : {winNum} \n패 : {loseNum} \n승률 : <color=#00ff80>{roundWinPCT}%</color>";
            else if(roundWinPCT >= 40)
                Stat.text = $"승 : {winNum} \n패 : {loseNum} \n승률 : <color=#f6bb43>{roundWinPCT}%</color>";
            else
                Stat.text = $"승 : {winNum} \n패 : {loseNum} \n승률 : <color=#fb6544>{roundWinPCT}%</color>";
        }

        else{
            HasRecord.text = $"You haven't played any game yet.";
            Stat.text = $"승 : {winNum} \n패 : {loseNum} \n승률 : {roundWinPCT}%";
        }
    }

    public void OnStatisticsExitClick()
    {
        statPanel.SetActive(false);
    }

    public void OnCreditsClick(){
        statPanel.SetActive(false);
        how2PlayPanel.SetActive(false);
        nickSavePanel.SetActive(false);
        creditPanel.SetActive(true);
    }

    public void OnCreditsExitClick()
    {
        creditPanel.SetActive(false);
    }

    public void OnHowToPlay(){
        statPanel.SetActive(false);
        creditPanel.SetActive(false);
        nickSavePanel.SetActive(false);
        how2PlayPanel.SetActive(true);
    }

    public void OnHow2PlayExitClick()
    {
        how2PlayPanel.SetActive(false);
    }
#endregion
}