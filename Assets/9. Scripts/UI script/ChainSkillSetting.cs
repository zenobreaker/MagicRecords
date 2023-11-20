using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// ü�ν�ų���� UI Ŭ����
// ü�ν�ų�� �ִ� 3���� �߰��� ����� �� �ִ�.
// ü�ν�ų�� cp�� �ִ�� ������ Ȱ��ȭ�Ǹ� ���̽� ��ų��
// ����ϸ� ü�ν�ų�� ����� ������� �ٲ㰡�� ��밡���ϴ�.
// ���� ����� ��� ü��3�� ȿ���� ���� �� �մ�. 
public class ChainSkillSetting : MonoBehaviour
{
    [SerializeField] SkillQuickSlot[] chainSkillSlots = null;   // ü�ν�ų����

    public Text chainEffectDesc;

    Character drawTarget; // 스킬을 그릴 대상

    public static ChainSkillSetting instance; 
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void OnEnable()
    {
        DrawChainSkillForCharacter(drawTarget);
    }

    public void SetDrawTarget(Character target)
    {
        if (target == null) return;

        drawTarget = target;
    }

    // 캐릭터가 소지한 체인스킬리스트를 찾아 그려낸다. 
    public void DrawChainSkillForCharacter(Character wheeler)
    {
        if (wheeler == null) return;

        int count = 0; 
        foreach(var chain in wheeler.chainsSkills)
        {
            if (chain.Value == null) continue; 

            var chainSkill = chain.Value;

            chainSkillSlots[count].SetSkill(chainSkill);
            
            count++;
        }
    }

    // ü�� ��ų�� ���� ��ġ�ߴ��� �˻� 
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


    // ü�ν�ų Ȱ��ȭ �� ȿ���� �����ش�.
    public void DrawChainSkillEffect(int count)
    {
        if (chainEffectDesc == null)
        {
            return;
        }

        // todo ������ �� ���� 
        if (count < 3)
        {
            // todo ü�� ��ų �� ȿ�� ���� �ؽ�Ʈ �Ҵ�
            chainEffectDesc.text = "";
        }
        else 
        {
            chainEffectDesc.text = "";
        }
    }

}
