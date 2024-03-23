using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor;

public class PlayerCtrl : MonoBehaviourPunCallbacks
{

    public int curWin;   //게임 시작 전 win 횟수(playerpref)
    public int curLose;  //게임 시작 전 lose 횟수(playerpref)

    public int newWin; //이기면 증가
    public int newLose; //지면 증가

    public int check; //newWin 혹은 loseWin이 올라간 횟수를 셈. 첫 번째 증가에서만(check==0일 때만) 이벤트가 일어나도록함.
    // MonsterCtrl에서 Destroy(Player) 함수를 사용하지 못한 관계로, 여러 번 충돌하여 점수가 여러 번 올라가는 것을 방지하기 위해 사용함. 

    public float pct; //PlayerPrefs 승률변수(winPCT)에 저장할 값을 계산

    [SerializeField]
    private CharacterController controller;
    private new Transform transform;
    private Animator anim;
    private new Camera camera;
    private GameManager gm = GameManager.Instance;

    Vector3 moveVec;

    private PhotonView pv;
    public float moveSpeed = 6.0f;

    public bool GimicFearRun = false;

    public int coin;
    //public int increaseCost;            // 기믹 사용시 값 증가
    private TextMeshProUGUI coinUI;
    public TextMeshProUGUI WinOrLoseUI;
    private TextMeshProUGUI Skill1_UI;
    private TextMeshProUGUI Skill2_UI;
    private TextMeshProUGUI Skill3_UI;
    private TextMeshProUGUI Skill4_UI;

    SkillCtrl skillctrl;
    private MaterialPropertyBlock mpb;

    private GameObject arrow1;
    private GameObject arrow2;

    private GameObject exitUI;
    private SoundEffect seff;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        transform = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        camera = Camera.main;

        pv = GetComponent<PhotonView>();
        coinUI = GameObject.FindGameObjectWithTag("CoinNum")?.GetComponent<TextMeshProUGUI>();
        coin = 5;

        curWin = PlayerPrefs.GetInt("winNum");
        curLose = PlayerPrefs.GetInt("loseNum");
        newWin = 0;
        newLose = 0;
        check=0;
        pct = 0.0f;

        //WinOrLoseUI = GameObject.FindGameObjectWithTag("WinOrLose")?.GetComponent<TextMeshProUGUI>();
        GameObject.Find("Canvas").transform.Find("Panel_WinOrLose").gameObject.SetActive(false);
        skillctrl = GameObject.Find("SkillManager").GetComponent<SkillCtrl>();
 
        //increaseCost = 1;
        mpb = new MaterialPropertyBlock();

        seff = GameObject.FindGameObjectWithTag("SoundEff").GetComponent<SoundEffect>();


        arrow1 = GameObject.Find("Canvas").transform.Find("Arrow1").gameObject;
        arrow2 = GameObject.Find("Canvas").transform.Find("Arrow2").gameObject;
        //arrow1 = GameObject.FindGameObjectWithTag("Arrow1");
        //arrow2 = GameObject.FindGameObjectWithTag("Arrow2");
        exitUI = GameObject.Find("Canvas").transform.Find("RealExit").gameObject;

        if (PhotonNetwork.IsMasterClient)
            arrow2.SetActive(false);
        else
            arrow1.SetActive(false);
    }

    void Update()
    {
        if(pv.IsMine)
        {
            skillctrl.coin = coin;
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && gm.time > 3)
            {
                Move();
                Turn();
                AddBlock();
                RemoveBlock();
                GimicFear();
            }

            if(coin >= 0)
            {
                coinUI.text = $"{coin}";
            }

            if(coin >= 20)
                coin = 20;
        }

        if(gm.time > 3)
        {
            arrow1.SetActive(false);
            arrow2.SetActive(false);
        }
    }
    
    float hAxis => Input.GetAxis("Horizontal");
    float vAxis => Input.GetAxis("Vertical");
    //public bool checker = true;
    
    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        
        transform.position += moveVec*moveSpeed*Time.deltaTime;
        /*
        if(Input.GetKeyDown(KeyCodSpace) && checker && (coin - increaseCost)>=0)
        {
            checker = false;
            coin = coin - increaseCost;
            increaseCost++;
            moveSpeed=9.0f;
            StartCoroutine(speedFast());
            StartCoroutine(speedDelay());
        }
        */
        anim.SetBool("isWalk", hAxis != 0.0f || vAxis != 0.0f);

        if(PhotonNetwork.IsMasterClient)
        {
            if(newWin == 1 && check == 0)
            {
                pv.RPC("syncMasterClientWin", RpcTarget.Others);
                curWin++;
                check++;
                PlayerPrefs.SetInt("winNum", curWin);
                pct =  ((float)curWin/ (float)(curWin+curLose)) * 100;
                Debug.Log($"{pct}");
                PlayerPrefs.SetFloat("winPCT", pct);

                GameObject.Find("Canvas").transform.Find("Panel_WinOrLose").gameObject.SetActive(true);
                WinOrLoseUI = GameObject.FindGameObjectWithTag("WinOrLose").GetComponent<TextMeshProUGUI>();
                WinOrLoseUI.text = "You Win!";
                seff.WinSound();
            }
            else if(newLose==1 && check==0)
            {
                pv.RPC("syncMasterClientLose", RpcTarget.Others);
                curLose++;
                check++;
                PlayerPrefs.SetInt("loseNum", curLose);
                pct =  ((float)curWin/ (float)(curWin+curLose)) * 100;
                Debug.Log($"{pct}");
                PlayerPrefs.SetFloat("winPCT", pct);

                GameObject.Find("Canvas").transform.Find("Panel_WinOrLose").gameObject.SetActive(true);
                WinOrLoseUI = GameObject.FindGameObjectWithTag("WinOrLose").GetComponent<TextMeshProUGUI>();
                WinOrLoseUI.text = "You Lose!";
            }
        }
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

    void GimicFear() {
        if(Input.GetKeyDown(KeyCode.Q) && coin-8 >= 0) {
            coin -= 8;
            StartCoroutine(UpdateGimicFearRun());
            seff.FearSound();
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if(coll.tag == "BronzeCoin")
        {
            coin = coin + 1;
            //skillctrl.coin = coin;
            Destroy(coll.gameObject);
            seff.CoinSound();
        }
        else if(coll.tag == "SilverCoin")
        {
            coin = coin + 3;
            //skillctrl.coin = coin;
            Destroy(coll.gameObject);
            seff.CoinSound();
        }
        if(coll.tag == "GoldCoin")
        {
            coin = coin + 5;
            //skillctrl.coin = coin;
            Destroy(coll.gameObject);
            seff.CoinSound();
        }
    }

    void AddBlock()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f))
        {
            if(Input.GetMouseButtonDown(0) && hit.collider.gameObject.tag == "Ground" && (coin - 3)>=0 && exitUI.activeSelf == false)
            {
                coin = coin - 3;
                //Vector3 newPos = hit.transform.position;
                //GameObject newBlock = PhotonNetwork.Instantiate("ObstacleBlock", new Vector3(6,7,0), Quaternion.Euler(new Vector3(-90,0,0)), 0);
                //newBlock.transform.position = newPos;
                //newBlock.transform.Translate(new Vector3(0,0,2));       // 002
                GameObject aboveblock = hit.collider.gameObject.GetComponent<FloorBlock>().overBlock;
                StartCoroutine(Creat(aboveblock));
                seff.CreateSound();
            }
        }
    }

    void RemoveBlock()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f))
        {
            if(Input.GetMouseButtonDown(0) && hit.collider.gameObject.tag == "Wall" && (coin - 2)>=0 && exitUI.activeSelf == false)
            {
                coin = coin - 2;
                StartCoroutine(Dest(hit));
                seff.CrushSound();
            }
        }
    }

    IEnumerator Creat(GameObject block)
    {
        yield return new WaitForSeconds(0.1f);
        block.SetActive(true);
        pv.RPC("syncBlockAppear", RpcTarget.Others, block.name);
        Debug.Log(block.name);
        yield return new WaitForSeconds(2.0f);
        block.SetActive(false);
        pv.RPC("syncBlockDisappear", RpcTarget.Others, block.name);
    }
    

    IEnumerator Dest(RaycastHit hit)
    {
        yield return new WaitForSeconds(0.1f);
        GameObject block = hit.collider.gameObject;
        block.SetActive(false);
        pv.RPC("syncBlockDisappear", RpcTarget.Others, block.name);
        yield return new WaitForSeconds(2.0f);
        block.SetActive(true);
        pv.RPC("syncBlockAppear", RpcTarget.Others, block.name);

        Color originalColor = new Color(1f, 1f, 1f, 1.0f);
        mpb.SetColor("_Color", originalColor);
        hit.collider.gameObject.GetComponentInChildren<Renderer>().SetPropertyBlock(mpb);
    }

    IEnumerator UpdateGimicFearRun() {
        yield return new WaitForSeconds(0.1f);
        GimicFearRun = true;
        pv.RPC("syncGimicFearRunTrue", RpcTarget.Others);
        yield return new WaitForSeconds(3.0f);
        GimicFearRun = false;
        pv.RPC("syncGimicFearRunFalse", RpcTarget.Others);
    }

/*
    IEnumerator speedFast()
    {
        yield return new WaitForSeconds(2.0f);
        moveSpeed=6.0f;
    }
    IEnumerator speedDelay()
    {
        yield return new WaitForSeconds(4.0f);
        checker=true;
    }
*/
    GameObject Finder(string blockname)
    {
        GameObject objectInScene = new GameObject();

        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {   
            /*
            if (EditorUtility.IsPersistent(go.transform.root.gameObject) && !(go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave))
                objectsInScene.Add(go);
            */
            if(go.name == blockname)
                objectInScene = go;
        }

        return objectInScene;
    }

    [PunRPC]
    void syncBlockDisappear(string blockname)
    {
        GameObject block = GameObject.Find(blockname);
        if(block.activeSelf == true)
        {
            block.SetActive(false);
        }
    }
    [PunRPC]
    void syncBlockAppear(string blockname)
    {
        GameObject block = Finder(blockname);
        if(block.activeSelf == false)
        {
            block.SetActive(true);
        }
    }

    [PunRPC]
    void syncGimicFearRunTrue() {
        GimicFearRun = true;
    }

    [PunRPC]
    void syncGimicFearRunFalse() {
        GimicFearRun = false;
    }

    [PunRPC]
    void syncMasterClientWin()
    {
        if(GameObject.Find("Canvas").transform.Find("Panel_WinOrLose").gameObject.activeSelf == false)
        {
            curLose++;
            PlayerPrefs.SetInt("loseNum", curLose);
            pct =  ((float)curWin/ (float)(curWin+curLose)) * 100;
            Debug.Log($"{pct}");
            PlayerPrefs.SetFloat("winPCT", pct);
            GameObject.Find("Canvas").transform.Find("Panel_WinOrLose").gameObject.SetActive(true);
            WinOrLoseUI = GameObject.FindGameObjectWithTag("WinOrLose").GetComponent<TextMeshProUGUI>();
            WinOrLoseUI.text = "You Lose!";
        }
    }

    [PunRPC]
    void syncMasterClientLose()
    {
        if(GameObject.Find("Canvas").transform.Find("Panel_WinOrLose").gameObject.activeSelf == false)
        {
            curWin++;
            PlayerPrefs.SetInt("winNum", curWin);
            pct =  ((float)curWin/ (float)(curWin+curLose)) * 100;
            Debug.Log($"{pct}");
            PlayerPrefs.SetFloat("winPCT", pct);
            GameObject.Find("Canvas").transform.Find("Panel_WinOrLose").gameObject.SetActive(true);
            WinOrLoseUI = GameObject.FindGameObjectWithTag("WinOrLose").GetComponent<TextMeshProUGUI>();
            WinOrLoseUI.text = "You Win!";
            seff.WinSound();
        }
    }
}
