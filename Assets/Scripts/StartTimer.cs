using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class StartTimer : MonoBehaviourPunCallbacks
{
    private float startTime;
    private float showTime;
    public TextMeshProUGUI startTimerUI;
    public GameManager gm;

    void Start()
    {
        startTimerUI.gameObject.SetActive(false);
        startTime = 3.0f;
    }

    void Update()
    {
        if(gm.time>0 && gm.time<=3)
        {
            startTimerUI.gameObject.SetActive(true);
            startTime = 3.0f - gm.time;
            showTime = Mathf.Round(startTime);
            startTimerUI.text = $"{showTime}";
        }
        else
        {
            startTimerUI.gameObject.SetActive(false);
        }
        
    }
}
