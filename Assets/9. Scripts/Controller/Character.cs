using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ���� ���� ����
//public enum EquipmentSlot
//{
//    WEAPON,         // ���� 
//    ARMOR,          // ���
//    WHEEL,          // ����
//    ACCSESORRY_1,   // �Ǽ��縮 1
//    ACCSESORRY_2,   // �Ǽ��縮 1
//    ACCSESORRY_3,   // �Ǽ��縮 3
//}


[System.Serializable]
public class Character
{
    /*  �÷��̾� ��ü ����
     *  CharStat�� EquipItem�� �ʵ�� ���´�
    */
    public uint objectID;

    [SerializeField]
    private int playerID; // �÷��̾� ID ��� ������ ����� ã�� �� 

    [SerializeField]
    private CharStat charStat = null;
    private int playerHP;
    private int playerMP;


    //[SerializeField]
    // ĳ���Ͱ� ������ ��� 
    public Dictionary<EquipType, EquipItem> equipItems = new Dictionary<EquipType, EquipItem>();

    // ĳ���Ͱ� ������ ��ų 
    private int chainIdx;
    private Skill[] skills = new Skill[4];
    private Skill[] chainsSkills = new Skill[3];

    // ������ ���
    public MagicalDrone drone;

    // �ش� ĳ���Ͱ� ����޴� ���ڵ� ��� < �̰� ���Ŀ� ������ ���� 
    public List<RecordInfo> selectRecordInfos = new List<RecordInfo>();

    public Character()
    {
        InitailizeEquipment(); 
    }


    public int MyID
    {
        get { return playerID; }
        set { playerID = value; }
    }

    public CharStat MyStat {
        get { return charStat; }
        set
        {
            charStat = value;
        }
    }
    public void InitCurrentHP()
    {
        MyCurrentHP = MyMaxHP;
    }

    public int MyCurrentHP
    {
        get
        {
            return playerHP;
        }

        set
        {
            playerHP = value;
        }
    }


    public void InitCurrentMP()
    {
        MyCurrentMP = MyMaxMP;
    }

    public int MyMaxHP
    {
        get { return MyStat.totalHP; }
    }


    public int MyCurrentMP
    {
        get
        {
            return playerMP;
        }
        set
        {
            playerMP = value;

        }
    }

    public int MyMaxMP
    {
        get { return MyStat.totalMP; }
    }

    public int GetExp 
    { 
        get { return MyStat.giveChanceExp; } 
    }

    public int MyTotalAttack
    {
        get
        {
            if(MyStat == null)
            {
                return 1; 
            }

            return MyStat.totalATK;
        }

        set { MyStat.totalATK += value; }
    }


    public Skill[] MySkills
    {
        get { return skills; }
    }
    
    public Skill[] MyChains
    {
        get { return chainsSkills; }
    }


    // ���� ��� �ʱ�ȭ 
    public void InitailizeEquipment()
    {
        equipItems.Clear();
        equipItems.Add(EquipType.WEAPON, null);
        equipItems.Add(EquipType.ARMOR, null);
        equipItems.Add(EquipType.WHEEL, null);
        equipItems.Add(EquipType.ACCSESORRY_1, null);
        equipItems.Add(EquipType.ACCSESORRY_2, null);
        equipItems.Add(EquipType.ACCSESORRY_3, null);
        equipItems.Add(EquipType.DRONE, null);
    }

    // ���� ��ų �ʱ�ȭ 

    // �����Ϸ��� �������� �ִ��� �˻� 
    public bool CheckHadItem(EquipItem p_EquipItem)
    {
        foreach(var equipment in equipItems)
        {
            if (equipment.Equals(p_EquipItem))
                return true;
        }
        return false;
        
    }


    public Item GetEquipItemSlot(EquipType _slot)
    {
        return equipItems[_slot];
    }


    // ��� ���� 
    public void EquipItem(EquipItem equipItem)
    {
        if (equipItem == null) return;

        // ������ �ش� ������ ��� �����ߴ��� �˻�
        if (equipItems.ContainsKey(equipItem.equipType) == true)
        {
            // ������ ������ ��� ���� ����
            RemoveEquipment(equipItem.equipType);
        }
        
        // ��� ����
        equipItems[equipItem.equipType] = equipItem;

        // ������ �ɷ�ġ ���� 
        // ����
        MyStat.extraStat.ApplyOptionExtraStat(equipItem.itemMainAbility);
        // ����
        foreach (var ability in equipItem.itemAbilities)
        {
            MyStat.extraStat.ApplyOptionExtraStat(ability);
        }

        MyStat.ApplyOption();
    }

    // ��� ���� 
    public void RemoveEquipment(EquipType _typeSlot)
    {
        var equipItem = equipItems[_typeSlot];
        // ������ �ɷ�ġ ���� 
        if (equipItem != null)
        {
            // ����
            MyStat.extraStat.ApplyOptionExtraStat(equipItem.itemMainAbility, true);
            // ����
            foreach (var ability in equipItem.itemAbilities)
            {
                MyStat.extraStat.ApplyOptionExtraStat(ability, true);
            }

            MyStat.ApplyOption();
        }
        equipItems[_typeSlot] = null;
    }
 
    // ��ų ���� 
    public void SetSkills(Skill[] p_Skills, Skill[] p_Chains)
    {
        this.skills = (Skill[])p_Skills.Clone(); 
        this.chainsSkills = (Skill[])p_Chains.Clone();
    }

    public void SetSkill(Skill p_Target, int p_idx, bool isChain )
    {
        if (p_idx < 0 || skills.Length < p_idx)
            return;

        if (!isChain)
            skills[p_idx] = p_Target;
        else
            chainsSkills[p_idx] = p_Target;
    }

    

    public void SetStartChainSkill(int p_TargetIdx)
    {
        chainIdx = p_TargetIdx;
    }


    public void ClearChains()
    {
        for (int i = 0; i < chainsSkills.Length; i++)
        {
            chainsSkills[i] = null;
        }
    }



    // ĳ���� ����ġ ���ý�Ű�� ����ġ�� �Ǹ� �������� ��Ų��.  
    public void GrowUp(int _exp)
    {
        if(this.MyStat == null)
        {
            return; 
        }

        // �� ĳ������ ����Ŭ������ ����ġ ����
        this.MyStat.GrowUp(_exp);
    }



    // ���ڵ� �ɷ� ���� 
    public void ApplyRecordAbility(RecordInfo record)
    {
        if (record == null || selectRecordInfos.Contains(record) == true) return;

        // ȿ�� ������ �ִ��� �˻� 
        if (record.specialOption == null)
        {
            // ���ٸ� �Ŵ����� ���� �Ҵ��Ų��. 
            if (RecordManager.instance == null) return;

            record.specialOption = RecordManager.instance.GetSpecialOptionToRecordInfo(record.specialOptionID);
        }


        this.MyStat.extraStat.ApplyOptionExtraStat(
        record.specialOption.abilityType, record.specialOption.value, record.specialOption.isPercentage);

        MyStat.ApplyOption();
    }

    // ���ڵ� �ɷ� ���� ����
    public void RemoveRecordAbility(RecordInfo record)
    {
        if (record == null) return;

        this.MyStat.extraStat.ApplyOptionExtraStat(
            record.specialOption.abilityType, -record.specialOption.value, record.specialOption.isPercentage);

        //selectRecordInfos.Remove(record);

        MyStat.ApplyOption();
    }
}
