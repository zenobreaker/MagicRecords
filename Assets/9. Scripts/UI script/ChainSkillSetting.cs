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

    public static ChainSkillSetting instance; 
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
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
