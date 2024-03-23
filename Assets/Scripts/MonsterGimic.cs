using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;
using System;

public class Pair<A, B> {
    public Pair() { }

    public Pair(A _first, B _second) {
        this.first = _first;
        this.second = _second;
    }

    public A first { get; set; }
    public B second { get; set; }
};

public class MonsterGimic : MonoBehaviour
{
    private Transform monsterTr;
    private Transform playerTr1;
    private Transform playerTr2;
    private NavMeshAgent agent;
    private GameManager gm = GameManager.Instance;
    private Animator anim;
    private PhotonView pv;

    PlayerCtrl playerctrl1;
    PlayerCtrl playerctrl2;
    MonsterCtrl monsterctrl;

    private GameObject player1;
    private GameObject player2;

    private bool[,] blocks = new bool[GameManager.row, GameManager.column];
    private int[,] dist_player1 = new int[GameManager.row, GameManager.column];             // 각 floor가 player로부터 떨어진 거리 측정 (BFS로 계산)
    private int[,] dist_player2 = new int[GameManager.row, GameManager.column];
    private int[,] availabledist_player1 = new int[GameManager.row, GameManager.column];    // 각 floor에서 player로부터 얼마나 더 멀어질 수 있는지 측정
    private int[,] availabledist_player2 = new int[GameManager.row, GameManager.column];

    private int [,] dir = {{-1, 0}, {0, 1}, {1, 0}, {0, -1}};

    // Start is called before the first frame update
    void Start()
    {
        monsterTr = GetComponent<Transform>();
        playerTr1 = GameObject.FindWithTag("Player1").GetComponent<Transform>();
        playerTr2 = GameObject.FindWithTag("Player2").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        pv = GetComponent<PhotonView>();

        player1 = GameObject.FindWithTag("Player1");
        player2 = GameObject.FindWithTag("Player2");

        playerctrl1 = player1.GetComponent<PlayerCtrl>();
        playerctrl2 = player2.GetComponent<PlayerCtrl>();
        monsterctrl = GetComponent<MonsterCtrl>();
    }

    void updateblocks() {
        for(int i=0; i<GameManager.row; i++) for(int j=0; j<GameManager.column; j++) blocks[i, j] = false;
        Transform[] blockpoints = GameObject.Find("BlockBlock").GetComponentsInChildren<Transform>();
        for(int i=1; i<blockpoints.Length; i++) blocks[(int) (blockpoints[i].position.z+0.5)/2, (int) (blockpoints[i].position.x+0.5)/2] = true;
    }

    void calculatedist(Pair<int, int> startloc, int[,] dist) {
        bool[,] visited = new bool[GameManager.row, GameManager.column];
        for(int i=0; i<GameManager.row; i++) for(int j=0; j<GameManager.column; j++) dist[i, j] = -1;
        for(int i=0; i<GameManager.row; i++) for(int j=0; j<GameManager.column; j++) visited[i, j] = false;
        Queue<Pair<int, int>> Q = new Queue<Pair<int, int>>();
        Q.Enqueue(startloc);
        dist[startloc.first, startloc.second] = 0; 
        visited[startloc.first, startloc.second] = true;
        while(Q.Count != 0) {
            Pair<int, int> curloc = Q.Peek(); Q.Dequeue();
            for(int d=0; d<4; d++) {
                Pair<int, int> adjloc = new Pair<int, int>(curloc.first + dir[d, 0], curloc.second + dir[d, 1]);
                if(adjloc.first < 0 || adjloc.first >= GameManager.row || adjloc.second < 0 || adjloc.second >= GameManager.column) continue;
                if(blocks[adjloc.first, adjloc.second] || visited[adjloc.first, adjloc.second]) continue;
                Q.Enqueue(adjloc);
                dist[adjloc.first, adjloc.second] = dist[curloc.first, curloc.second] + 1;
                visited[adjloc.first, adjloc.second] = true;
            }
        }
    }

    void calculateavailabledist(int[,] dist, int[,] availabledist) {
        for(int i=0; i<GameManager.row; i++) for(int j=0; j<GameManager.column; j++) availabledist[i, j] = -1;
        int maxdist = 0;
        for(int i=0; i<GameManager.row; i++) for(int j=0; j<GameManager.column; j++) maxdist = Math.Max(maxdist, dist[i, j]);
        List<Pair<int, int>>[] locbydist = new List<Pair<int, int>>[maxdist+1];
        for(int i=0; i<=maxdist; i++) locbydist[i] = new List<Pair<int, int>>();
        for(int i=0; i<GameManager.row; i++) for(int j=0; j<GameManager.column; j++) if(dist[i, j] != -1) locbydist[dist[i, j]].Add(new Pair<int, int>(i, j));
        for(int i=maxdist; i>=0; i--) foreach(Pair<int, int> curloc in locbydist[i]) {
            availabledist[curloc.first, curloc.second] = 0;
            for(int d=0; d<4; d++) {
                Pair<int, int> adjloc = new Pair<int, int>(curloc.first + dir[d, 0], curloc.second + dir[d, 1]);
                if(adjloc.first < 0 || adjloc.first >= GameManager.row || adjloc.second < 0 || adjloc.second >= GameManager.column) continue;
                if(dist[adjloc.first, adjloc.second] == dist[curloc.first, curloc.second] + 1) {
                    availabledist[curloc.first, curloc.second] = Math.Max(availabledist[curloc.first, curloc.second], availabledist[adjloc.first, adjloc.second] + 1);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(monsterctrl.state == MonsterCtrl.State.FEAR) {

            updateblocks();

            Pair<int, int> monsterfloor = new Pair<int, int>((int) (monsterTr.position.z + 1)/2, (int) (monsterTr.position.x + 1)/2);
            Pair<int, int> playerfloor1 = new Pair<int, int>((int) (playerTr1.position.z + 1)/2, (int) (playerTr1.position.x + 1)/2);
            Pair<int, int> playerfloor2 = new Pair<int, int>((int) (playerTr2.position.z + 1)/2, (int) (playerTr2.position.x + 1)/2);

            if(playerctrl1.GimicFearRun) {
                calculatedist(playerfloor1, dist_player1);
                calculateavailabledist(dist_player1, availabledist_player1);
            }
            if(playerctrl2.GimicFearRun) {
                calculatedist(playerfloor2, dist_player2);
                calculateavailabledist(dist_player2, availabledist_player2);
            }

            Pair<int, int> destination = null;
            for(int d=0; d<4; d++) {
                Pair<int, int> adjloc = new Pair<int, int>(monsterfloor.first + dir[d, 0], monsterfloor.second + dir[d, 1]);
                if(adjloc.first < 0 || adjloc.first >= GameManager.row || adjloc.second < 0 || adjloc.second >= GameManager.column) continue;
                if(playerctrl1.GimicFearRun && dist_player1[adjloc.first, adjloc.second] < dist_player1[monsterfloor.first, monsterfloor.second]) continue;
                if(playerctrl2.GimicFearRun && dist_player2[adjloc.first, adjloc.second] < dist_player2[monsterfloor.first, monsterfloor.second]) continue;
                
                bool iscandidate;
                if(destination == null) iscandidate = true;
                else {
                    if((playerctrl1.GimicFearRun && dist_player1[adjloc.first, adjloc.second] > dist_player1[destination.first, destination.second]) || (playerctrl2.GimicFearRun && dist_player2[adjloc.first, adjloc.second] > dist_player2[destination.first, destination.second])) iscandidate = true;
                    else if((playerctrl1.GimicFearRun && availabledist_player1[adjloc.first, adjloc.second] < availabledist_player1[destination.first, destination.second]) || (playerctrl2.GimicFearRun && availabledist_player2[adjloc.first, adjloc.second] < availabledist_player2[destination.first, destination.second])) iscandidate = false;
                    else iscandidate = true;
                }

                if(iscandidate) destination = adjloc;
            }

            if(destination == null) {
                agent.isStopped = true;
                anim.SetBool("isTrace", false);
            }
            else {
                agent.isStopped = false;
                anim.SetBool("isTrace", true);
                agent.SetDestination(new Vector3(destination.second*2, monsterTr.position.y, destination.first*2));
                agent.speed = ((gm.time > 240) ? 1.2f : 0.8f + 0.4f * gm.time / 240) * 2;
            }
        }
    }
}