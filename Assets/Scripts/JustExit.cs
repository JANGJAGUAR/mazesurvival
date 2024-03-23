using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class JustExit : MonoBehaviourPunCallbacks
{
    public GameObject realEndUI;

    void Start()
    {
        realEndUI.SetActive(false);
    }

    public void OnExitClick1()
    {
        realEndUI.SetActive(true);
    }

    public void OnExitExit()
    {
        realEndUI.SetActive(false);
    }

    public void OnExitClick2()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("StartUI");
    }
}
