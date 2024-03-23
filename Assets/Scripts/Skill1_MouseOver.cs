using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;


public class Skill1_MouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject ExplSkill1;    //스킬 설명 배경(말풍선) 패널 

    void Start(){
        ExplSkill1 = GameObject.Find("Canvas").transform.Find("Panel_ExplSkill1").gameObject;
        ExplSkill1.SetActive(false);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData){
        ExplSkill1.SetActive(true);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData){
        ExplSkill1.SetActive(false);
    }

    
}
