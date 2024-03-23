using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class Skill4_MouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject ExplSkill4;    //스킬 설명 배경(말풍선) 패널 
    public TextMeshProUGUI Skill4_Text;  //말풍선 패널에 포함된 스킬 설명 텍스트
    private GameObject EnableSkillUI;   //캐릭터별 고유 스킬의 활성화 배널 
    private GameObject DisableSkillUI;  //캐릭터별 고유 스킬의 비활성화 패널
    public Image EnableSkillImage;   //활성화 스킬 패널의 이미지
    public Image DisableSkillImage; //비활성화 스킬 패널의 이미지
    public Sprite char1EnableSprite;   //캐릭터1의 고유스킬 활성화이미지 sprite
    public Sprite char1DisableSprite;  //캐릭터1의 고유스킬 비활성화이미지 sprite 
    public Sprite char2EnableSprite;   //캐릭터2의 고유스킬 활성화이미지 sprite
    public Sprite char2DisableSprite;  //캐릭터2의 고유스킬 비활성화이미지 sprite 
    public Sprite char3EnableSprite;   //캐릭터3의 고유스킬 활성화이미지 sprite \
    public Sprite char3DisableSprite;  //캐릭터3의 고유스킬 비활성화이미지 sprite  
    public TextMeshProUGUI gimicCost;
    public int gimic1Cost = 1;
    private PhotonView pv;
    

    public int character=0; //캐릭터 번호 확인(Charac1Gimic Charac2... 스크립트에서 캐릭터 번호가 전달됨) 

    void Start(){
        ExplSkill4 = GameObject.Find("Canvas").transform.Find("Panel_ExplSkill4").gameObject;
        Skill4_Text = ExplSkill4.transform.Find("T_Skill4").GetComponent<TextMeshProUGUI>();

        EnableSkillUI = GameObject.Find("Canvas").transform.Find("Panel_Skill4").gameObject;
        DisableSkillUI = GameObject.Find("Canvas").transform.Find("Panel_Skill4_disable").gameObject;
        EnableSkillImage = EnableSkillUI.GetComponent<Image>(); 
        DisableSkillImage = DisableSkillUI.GetComponent<Image>();
        pv = GetComponent<PhotonView>();

        if(character == 1){   
            EnableSkillImage.sprite = char1EnableSprite;
            DisableSkillImage.sprite = char1DisableSprite;
            gimicCost.text = "5 coin";
            Skill4_Text.text = "에밀리의 속도가 2초간 1.5배 증가합니다.";
            ExplSkill4.SetActive(false);
            
        } 

        else if(character == 2){
            EnableSkillImage.sprite = char2EnableSprite;
            DisableSkillImage.sprite = char2DisableSprite;
            Skill4_Text.text = "루슬라는 3초마다 1코인을 획득합니다.";
            gimicCost.text = " ";
            ExplSkill4.SetActive(false);
        }

        else if(character == 3){
            EnableSkillImage.sprite = char3EnableSprite;
            DisableSkillImage.sprite = char3DisableSprite;
            gimicCost.text = "12 coin";
            Skill4_Text.text = "알렉스가 유령화하여 2초간 \n벽과 몬스터를 통과합니다.";
            ExplSkill4.SetActive(false);
        }
    }

    void Update()
    {
        /*
        if(character == 1 && pv.IsMine)
            gimicCost.text = $"{gimic1Cost} coin";
        */
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData){
        ExplSkill4.SetActive(true);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData){
        ExplSkill4.SetActive(false);
    }
}
