using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviourPunCallbacks
{
    public enum State
    {
        IDLE,
        TRACE,
        FEAR
    }
    public State state = State.IDLE;
    private float traceDist = 12.0f;


    private Transform monsterTr;
    private Transform playerTr1;
    private Transform playerTr2;
    private Transform goToPos;
    private NavMeshAgent agent;
    private Animator anim;
    private GameManager gm;
    private PhotonView pv;

    public int losePlayer = 0;

    PlayerCtrl playerctrl1;   //각 player의 스크립트를 저장할 곳(승패를 각각 전송해주기 위해)
    PlayerCtrl playerctrl2;

    public GameObject playerobject1;   
    public GameObject playerobject2; 

    public int CollNumCheck;    //플레이어들과의 충돌 횟수를 셈. 첫 번째 충돌에서만(CollNumCheck가 0일 때만) 이벤트가 일어나도록 
    // 원래에는 충돌 횟수를 세는 대신 Destroy(Player)를 활용하려 하였으나, destroy 하니 PlayerCtrl 스크립트에서 승패 PlayerPref를 저장할 시간이 없어져 쓰지 못하였음. 

    public GameObject effObj;

    void Start()
    {
        gm = GameManager.Instance;
        pv = GetComponent<PhotonView>();
        monsterTr = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        playerTr1 = GameObject.FindWithTag("Player1").GetComponent<Transform>();
        playerTr2 = GameObject.FindWithTag("Player2").GetComponent<Transform>();

        playerobject1 = GameObject.FindWithTag("Player1");  
        playerobject2 = GameObject.FindWithTag("Player2");
        playerctrl1 = playerobject1.GetComponent<PlayerCtrl>();   //player1의 스크립트를 불러옴      // 이부분을 수정해야 여러명의 캐릭터 사용 가능
        playerctrl2 = playerobject2.GetComponent<PlayerCtrl>();   //player2의 스크립트를 불러옴 
        goToPos = monsterTr;

        agent = GetComponent<NavMeshAgent>();
        CollNumCheck = 0;
        effObj.SetActive(false);
    }

    
    void Update()
    {   
        state = (playerctrl1.GimicFearRun || playerctrl2.GimicFearRun) ? State.FEAR : State.IDLE;
        if(state != State.FEAR) {
            effObj.SetActive(false);
            pv.RPC("syncEffectOff", RpcTarget.Others);
            float distance1 = Vector3.Distance(playerTr1.position, monsterTr.position);
            float distance2 = Vector3.Distance(playerTr2.position, monsterTr.position);

            if(distance1 <= traceDist || distance2 <= traceDist)
            {
                state = State.TRACE; 
                agent.isStopped = false;
                anim.SetBool("isTrace", true);
                
                if(distance1 <= distance2)
                    goToPos = playerTr1;
                else
                    goToPos = playerTr2;
                //Debug.Log("x : "+ goToPos.transform.position.x + ", z : "+ goToPos.transform.position.z);
            }
            else
            {
                state = State.IDLE;
                agent.isStopped = true;
                anim.SetBool("isTrace", false);
            }

            agent.SetDestination(goToPos.position);

            agent.speed = ((GameManager.Instance.time > 240) ? 1.2f : 0.8f + 0.4f * GameManager.Instance.time / 240) * 5;
            //Debug.Log("monster speed : "+ agent.speed);
        }
        else if(state == State.FEAR)
        {
            pv.RPC("syncEffectOn", RpcTarget.Others);
            effObj.SetActive(true);
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        if(coll.collider.tag == "Player1" && CollNumCheck == 0)
        {
            losePlayer = 1; 
            playerctrl1.newLose++;
            playerctrl2.newWin++; 
            CollNumCheck = 1;
            //Debug.Log($"player{losePlayer} died");
            
        }

        if(coll.collider.tag == "Player2" && CollNumCheck == 0)
        {
            losePlayer = 2;
            playerctrl1.newWin++;
            playerctrl2.newLose++;
            CollNumCheck = 1;
            //Debug.Log($"player{losePlayer} died");
        }
    }

    [PunRPC]
    void syncEffectOff()
    {
        effObj.SetActive(false);
    }
    [PunRPC]
    void syncEffectOn()
    {
        effObj.SetActive(true);
    }
}
