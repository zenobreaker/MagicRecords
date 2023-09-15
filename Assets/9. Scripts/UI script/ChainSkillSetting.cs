using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// 체인스킬관련 UI 클래스
// 체인스킬은 최대 3개를 추가로 등록할 수 있다.
// 체인스킬은 cp를 최대로 모으면 활성화되며 베이스 스킬을
// 사용하면 체인스킬을 등록한 순서대로 바꿔가며 사용가능하다.
// 전부 사용할 경우 체인3의 효과를 얻을 수 잇다. 
public class ChainSkillSetting : MonoBehaviour
{
    [SerializeField]
    SkillQuickSlot selectedBaseSlot = null; 
    [SerializeField] SkillQuickSlot[] chainSkillSlots = null;   // 체인스킬슬롯

    public Text chainEffectDesc;

    public static ChainSkillSetting instance; 
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    // 체인 스킬을 전부 배치했는지 검사 
    public void CheckChainSkillCount()
    {
        int count = 0; 
        foreach(var chainSkillSlot in chainSkillSlots)
        {
            if (chainSkillSlot == null) continue; 

            if (chainSkillSlot.GetSkill() != null)
            {
                count++; 
            }
        }
        
        DrawChainSkillEffect(count);
    }


    // 체인스킬 활성화 시 효과를 적어준다.
    public void DrawChainSkillEffect(int count)
    {
        if (chainEffectDesc == null)
        {
            return;
        }

        // todo 관련한 값 수정 
        if (count < 3)
        {
            // todo 체인 스킬 별 효과 설명 텍스트 할당
            chainEffectDesc.text = "";
        }
        else 
        {
            chainEffectDesc.text = "";
        }
    }

}
