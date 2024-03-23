using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;


public class Skill3_MouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject ExplSkill3;    //스킬 설명 배경(말풍선) 패널 

    void Start(){
        ExplSkill3 = GameObject.Find("Canvas").transform.Find("Panel_ExplSkill3").gameObject;
        ExplSkill3.SetActive(false);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData){
        ExplSkill3.SetActive(true);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData){
        ExplSkill3.SetActive(false);
    }
}
