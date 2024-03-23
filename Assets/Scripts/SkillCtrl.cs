using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCtrl : MonoBehaviour
{
    public int coin;
    private GameObject Skill1_Disable;     //사용할 수 없는 스킬의 스킨을 넣은 패널
    private GameObject Skill2_Disable;
    private GameObject Skill3_Disable;
    private GameObject Skill4_Disable;

    public int characIndex;
    public int cost;

    void Start() 
    {
        Skill1_Disable = GameObject.Find("Canvas").transform.Find("Panel_Skill1_disable").gameObject;
        Skill2_Disable = GameObject.Find("Canvas").transform.Find("Panel_Skill2_disable").gameObject;
        Skill3_Disable = GameObject.Find("Canvas").transform.Find("Panel_Skill3_disable").gameObject;
        Skill4_Disable = GameObject.Find("Canvas").transform.Find("Panel_Skill4_disable").gameObject;
        Skill4_Disable.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(coin<2){   //PlayerCtrl에서 coin 개수의 변화를 업데이트받음
            Skill1_Disable.SetActive(true);    //모든 스킬 사용 불가능(disable 패널 활성화) 
            Skill2_Disable.SetActive(true);
            Skill3_Disable.SetActive(true);
        }
        else if(coin==2){
            //Debug.Log("skill 1 got color");
            Skill1_Disable.SetActive(false);     //1번 스킬 사용 가능(disable 패널 비활성화) 
            Skill2_Disable.SetActive(true);      //나머진 disable 패널 활성화
            Skill3_Disable.SetActive(true);
        }
        else if(coin<8){
            //Debug.Log("skill 1 got color");
            Skill1_Disable.SetActive(false);     //1번 스킬 사용 가능(disable 패널 비활성화) 
            Skill2_Disable.SetActive(false);      //나머진 disable 패널 활성화
            Skill3_Disable.SetActive(true);
        }
        else{
            //Debug.Log("skill 1, 2 got color");
            Skill1_Disable.SetActive(false);
            Skill2_Disable.SetActive(false);     //1,2번 스킬 사용 가능 
            Skill3_Disable.SetActive(false);
        } 

        if(characIndex == 1)
        {
            if(coin>= cost)
                Skill4_Disable.SetActive(false);
            else
                Skill4_Disable.SetActive(true);
        }
        else if(characIndex == 2)
            Skill4_Disable.SetActive(false);
        else if(characIndex == 3)
        {
            if(coin>= cost)
                Skill4_Disable.SetActive(false);
            else
                Skill4_Disable.SetActive(true);
        }
    }
}
