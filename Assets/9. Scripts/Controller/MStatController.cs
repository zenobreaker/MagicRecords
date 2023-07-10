using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MStatController : MonoBehaviour
{
    /*
     * ReswpanManager 필드에 포함 
     */

    public int Level_Incre_Value;       // 단계별 증가값
    public float rankValue = 1;         // 몬스터 등급에 따른 밸류 

    public float searchValue_normal = 0;
    public float serchValue_elite = 0;
    public float serchValue_boss = 0;

    // 몬스터 인식 범위를 반환하는 함수
    float GetMonsterSearchDistance(MonsterGrade _type)
    {
        switch (_type)
        {
            // 일반 몬스터 일 때 
            case MonsterGrade.NORMAL:
                return searchValue_normal;

            // 엘리트 등급 몬스터일 때 
            case MonsterGrade.ELITE:
                return serchValue_elite;

             // 보스 몬스터 일때 
            case MonsterGrade.BOSS:
                return serchValue_boss;
            default:
                return searchValue_normal;
        }
    }

    public void SetStatus(ref Status p_Status, MonsterGrade p_MonsterGrade)
    {
        switch (p_MonsterGrade)
        {
            case MonsterGrade.NORMAL:
                rankValue = 1f;
                p_Status.gameObject.GetComponent<Transform>().localScale = new Vector3(1, 1, 1);
                break;
            case MonsterGrade.ELITE:
                rankValue = 1.5f;
                p_Status.gameObject.GetComponent<Transform>().localScale = new Vector3(1.5f, 1.5f, 1.5f);
                break;
            case MonsterGrade.BOSS:
                rankValue = 5f;
                p_Status.gameObject.GetComponent<Transform>().localScale = new Vector3(3f, 3f, 3f);
                break;
        }

        // todo 아래 왜 랜덤으로 세팅하냐.. SetMonsterStatus 이 함수에서 세팅하는데 db에서 가져와서 하는듯
        // 이 함수느 왜 만들고 제대로 세팅안하는것 같다. 
        //CreateEnemy와 SetMonsterStatus 여기서 호출할 때 이 함수를 쓰도록 변경하자 
        p_Status.myGrade = p_MonsterGrade;

        p_Status.MyAttack += Mathf.RoundToInt(p_Status.MyAttack * Level_Incre_Value * rankValue / 100);
        p_Status.MyDeffence +=  Mathf.RoundToInt(p_Status.MyDeffence * Level_Incre_Value * rankValue  / 100);
        p_Status.MyMaxHp +=  Mathf.RoundToInt(p_Status.MyMaxHp * Level_Incre_Value * rankValue / 100);
        //p_Status.MyMaxHp = p_Status.MyHP;
        p_Status.MyEXP = Mathf.RoundToInt(Level_Incre_Value * rankValue);
        
    }

    public void SetRankTypeStatus(MonsterGrade p_MonsterGrade)
    {
        

        
    }


}
