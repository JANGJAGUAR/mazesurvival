using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Charac1Gimic : MonoBehaviourPunCallbacks
{
    public PlayerCtrl player;
    public int increaseCost;            // 기믹 사용시 값 증가
    public bool checker = true;
    private PhotonView pv;
    private GameManager gm = GameManager.Instance;

    Skill4_MouseOver skill4script1;   //인게임 skill에 적용되어있는 스크립트 
    Skill4_MouseOver skill4script2;

    SkillCtrl skillctrl;
    private GameObject enoughCoinUI;

    public GameObject effectObj;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        skill4script1 = GameObject.Find("Canvas").transform.Find("Panel_Skill4").GetComponent<Skill4_MouseOver>();
        skill4script2 = GameObject.Find("Canvas").transform.Find("Panel_Skill4_disable").GetComponent<Skill4_MouseOver>();
        skill4script1.character = 1;   //인게임 skill 스크립트에 캐릭터 번호가 1번임을 전달
        skill4script2.character = 1;
        increaseCost = 5;
        skillctrl = GameObject.Find("SkillManager").GetComponent<SkillCtrl>();
        enoughCoinUI = GameObject.Find("Canvas").transform.Find("Txt_NotEnoughCoins").GetComponent<GameObject>();
        effectObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(pv.IsMine)
        {
            skillctrl.characIndex = 1;
            skillctrl.cost = increaseCost;
            skill4script1.gimic1Cost = increaseCost;
            skill4script2.gimic1Cost = increaseCost;
        }

        if(pv.IsMine && Input.GetKeyDown(KeyCode.Space) && checker && (player.coin - increaseCost)>=0 && gm.time > 3)
        {
            //checker = false;
            player.coin = player.coin - increaseCost;
            //increaseCost++;
            player.moveSpeed=9.0f;
            effectObj.SetActive(true);
            pv.RPC("syncEffectOn", RpcTarget.Others);
            StartCoroutine(speedFast());
            //StartCoroutine(speedDelay());
        }
        else if(pv.IsMine && Input.GetKeyDown(KeyCode.Space) && checker && player.coin < increaseCost && gm.time > 3)
        {
            StartCoroutine(CoinErrorOn());
        }
    }
    IEnumerator speedFast() 
    {
        yield return new WaitForSeconds(2.0f);
        effectObj.SetActive(false);
        pv.RPC("syncEffectOff", RpcTarget.Others);
        player.moveSpeed=6.0f;
    }

    IEnumerator CoinErrorOn()
    {
        GameObject.Find("Canvas").transform.Find("Txt_NotEnoughCoins").gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        GameObject.Find("Canvas").transform.Find("Txt_NotEnoughCoins").gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        GameObject.Find("Canvas").transform.Find("Txt_NotEnoughCoins").gameObject.SetActive(false);
    }

    /*
    IEnumerator speedDelay()
    {
        yield return new WaitForSeconds(4.0f);
        checker=true;
    }
    */
    [PunRPC]
    void syncEffectOff()
    {
        effectObj.SetActive(false);
    }
    [PunRPC]
    void syncEffectOn()
    {
        effectObj.SetActive(true);
    }
}
