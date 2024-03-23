using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Charac2Gimic : MonoBehaviourPunCallbacks
{
    public PlayerCtrl player;
    private float time;
    private float MAXTIME = 5.0f;
    private PhotonView pv;
    private GameManager gm = GameManager.Instance;

    Skill4_MouseOver skill4script1; //인게임 skill에 적용되어있는 스크립트 
    Skill4_MouseOver skill4script2;
    SkillCtrl skillctrl;
    public GameObject effObj;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        time = 0;

        skill4script1 = GameObject.Find("Canvas").transform.Find("Panel_Skill4").GetComponent<Skill4_MouseOver>();
        skill4script2 = GameObject.Find("Canvas").transform.Find("Panel_Skill4_disable").GetComponent<Skill4_MouseOver>();
        skill4script1.character = 2;  //인게임 skill 스크립트에 캐릭터 번호가 2번임을 전달
        skill4script2.character = 2;
        skillctrl = GameObject.Find("SkillManager").GetComponent<SkillCtrl>();
        effObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(pv.IsMine)
        {
            skillctrl.characIndex = 1;
            skillctrl.cost = 0;
        }
        if(pv.IsMine && gm.time > 3)
        {
            time = time + Time.deltaTime;
            if(time >= MAXTIME)
            {
                StartCoroutine(ShowCoin());
                player.coin = player.coin + 1;
                time = 0;
            }
        }
    }

    IEnumerator ShowCoin()
    {
        effObj.SetActive(true);
        pv.RPC("syncAppearCoin", RpcTarget.Others);
        yield return new WaitForSeconds(1.0f);
        effObj.SetActive(false);
        pv.RPC("syncDisappearCoin", RpcTarget.Others);
    }

    [PunRPC]
    void syncAppearCoin()
    {
        effObj.SetActive(true);
    }
    [PunRPC]
    void syncDisappearCoin()
    {
        effObj.SetActive(false);
    }
}
