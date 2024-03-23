using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyManager : MonoBehaviour
{
    public GameObject PhotonManagerOJ;

    void Awake()
    {
        PhotonManagerOJ = GameObject.FindWithTag("PhotonManager");
        DontDestroyOnLoad(PhotonManagerOJ);
    }
}
