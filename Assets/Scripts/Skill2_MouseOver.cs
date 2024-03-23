using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;


public class Skill2_MouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject ExplSkill2;    //스킬 설명 배경(말풍선) 패널 

    void Start(){
        ExplSkill2 = GameObject.Find("Canvas").transform.Find("Panel_ExplSkill2").gameObject;
        ExplSkill2.SetActive(false);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData){
        ExplSkill2.SetActive(true);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData){
        ExplSkill2.SetActive(false);
    }
}
