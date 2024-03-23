using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Charac3Gimic : MonoBehaviourPunCallbacks
{
    public PlayerCtrl player;
    public bool checker = true;
    private PhotonView pv;
    private GameManager gm = GameManager.Instance;

    Skill4_MouseOver skill4script1;   //인게임 skill에 적용되어있는 스크립트 
    Skill4_MouseOver skill4script2;
    SkillCtrl skillctrl;
    CapsuleCollider coll;
    private GameObject enoughCoinUI;
    public GameObject effectObj;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        coll = GetComponent<CapsuleCollider>();
        skill4script1 = GameObject.Find("Canvas").transform.Find("Panel_Skill4").GetComponent<Skill4_MouseOver>();
        skill4script2 = GameObject.Find("Canvas").transform.Find("Panel_Skill4_disable").GetComponent<Skill4_MouseOver>();
        skill4script1.character = 3;  //인게임 skill 스크립트에 캐릭터 번호가 3번임을 전달
        skill4script2.character = 3;
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
            skillctrl.cost = 12;
        }

        if(pv.IsMine && Input.GetKeyDown(KeyCode.Space) && gm.time > 3 && player.coin >= 12)
        {
            checker = false;
            /*
            if(gm.time > 93)
                player.coin = player.coin - 3;
            else
            */
            player.coin = player.coin - 12;
            effectObj.SetActive(true);
            pv.RPC("syncEffectOn", RpcTarget.Others);
            //ShowSkillEffect(gameObject.transform.position, gameObject.transform.rotation);
            //gameObject.GetComponent<CapsuleCollider>().height = 2;
            //gameObject.GetComponent<CapsuleCollider>().center = new Vector3(0,-1,0);
            pv.RPC("syncGravityOff", RpcTarget.Others);
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            //gameObject.GetComponent<CapsuleCollider>().enabled = false;
            coll.center = new Vector3(0, 18, 0);
            pv.RPC("syncColliderOff", RpcTarget.Others);
            StartCoroutine(resetCollider());
            //StartCoroutine(delayCollider());
        }
        else if(pv.IsMine && Input.GetKeyDown(KeyCode.Space) && gm.time > 3 && player.coin < 12)
            StartCoroutine(CoinErrorOn());
    }

    IEnumerator resetCollider() 
    {
        yield return new WaitForSeconds(2.0f);
        //gameObject.GetComponent<CapsuleCollider>().enabled = true;
        effectObj.SetActive(false);
        pv.RPC("syncEffectOff", RpcTarget.Others);
        coll.center = new Vector3(0, 1.8f, 0);
        gameObject.GetComponent<Rigidbody>().useGravity = true;
        pv.RPC("syncGravityOn", RpcTarget.Others);
        pv.RPC("syncColliderOn", RpcTarget.Others);
        //gameObject.GetComponent<CapsuleCollider>().height = 4;
        //gameObject.GetComponent<CapsuleCollider>().center = new Vector3(0,0,0);
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
    IEnumerator delayCollider()
    {
        if(gm.time > 93) yield return new WaitForSeconds(5.0f);
        else 
        yield return new WaitForSeconds(3.0f);
        checker=true;
    }
*/
/*
    void ShowSkillEffect(Vector3 pos, Quaternion rot)
    {
        GameObject effect = Instantiate<GameObject>(skillEffect, pos, rot);
        Destroy(effect, 2.0f);
    }
*/
    [PunRPC]
    void syncColliderOn()
    {
        gameObject.GetComponent<CapsuleCollider>().enabled = true;
    }

    [PunRPC]
    void syncColliderOff()
    {
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
    }

    [PunRPC]
    void syncGravityOn()
    {
        gameObject.GetComponent<Rigidbody>().useGravity = true;
    }
    [PunRPC]
    void syncGravityOff()
    {
        gameObject.GetComponent<Rigidbody>().useGravity = false;
    }

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
