using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    private static GameManager instance = null;
    public static GameManager Instance => instance;

    public static int row = 9;
    public static int column = 15;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }

        // DontDestroyOnLoad(this.gameObject);
        CreatePlayer();
    }

    void CreatePlayer()
    {
        foreach(var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}");
        }
        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            int randNum = Random.Range(1, 4);
            string player1_str = "Player1_" + randNum.ToString();
            //PhotonNetwork.Instantiate("Player1", points[i].position, points[i].rotation, 0);
            PhotonNetwork.Instantiate(player1_str, points[1].position, points[1].rotation, 0);
        }
        else if(PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            int randNum = Random.Range(1, 4);
            string player2_str = "Player2_" + randNum.ToString();
            PhotonNetwork.Instantiate(player2_str, points[2].position, points[2].rotation, 0);
            
            StartCoroutine(MonsterGen(points));
        }
        else
            Debug.LogError("Error!");
    }

    public float time = 0;

    void Update() 
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == 2) 
        {
            time += Time.deltaTime;
        }
        // int min = (int) (time / 60), sec = (int) (time % 60);
        // Debug.Log($"min: {min}, sec: {sec}");
    }

    IEnumerator MonsterGen(Transform[] points)
    {
        yield return new WaitForSeconds(1);
        PhotonNetwork.Instantiate("Monster", points[3].position, points[3].rotation, 0);
    }


    public void OnExitClick()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("StartUI");
    }
}