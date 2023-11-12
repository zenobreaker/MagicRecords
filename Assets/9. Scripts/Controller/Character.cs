using System.Collections.Generic;
using System.Linq;
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
public enum SkillSlotNumber
{ 
    SLOT1 = 0,
    SLOT2,
    SLOT3,
    SLOT4,
    MAXSLOT = SLOT4,

    CHAIN1 = MAXSLOT+1,
    CHAIN2,
    CHAIN3,
    MAXCHAINSLOT = CHAIN3,
}


[System.Serializable]
public class Character
{
    /*  �÷��̾� ��ü ����
     *  CharStat�� EquipItem�� �ʵ�� ���´�
    */
    public int objectID = 0;

    [SerializeField]
    private int playerID; // �÷��̾� ID ��� ������ ����� ã�� �� 

    [SerializeField]
    private CharStat charStat = null;
    private int playerHP;
    private int playerMP;

    // ĳ���Ͱ� ������ ��� 
    public Dictionary<EquipType, EquipItem> equipItems = new Dictionary<EquipType, EquipItem>();

    // ĳ���Ͱ� ������ ��ų 
    private int chainIdx;
    public Dictionary<SkillSlotNumber, Skill> skills = new Dictionary<SkillSlotNumber, Skill>();
    public Dictionary<SkillSlotNumber, Skill> chainsSkills = new Dictionary<SkillSlotNumber, Skill>();
    public List<PassiveSkill> equippedPassiveSkills = new List<PassiveSkill>();
    
    //public PassiveClass myPassiveClass;// �ڽ��� ĳ����(����)
    // ������ ���
    public MagicalDrone drone;

    // �ش� ĳ���Ͱ� ����޴� ���ڵ� ��� < �̰� ���Ŀ� ������ ���� 
    public List<RecordInfo> selectRecordInfos = new List<RecordInfo>();


    public bool isAction = true;    // �ൿ �������� üũ flag
    public bool isDead = false; // �׾����� �Ǻ�

    public Character()
    {
        InitailizeEquipment();
        InitializeSkillSlot();
        //if(myPassiveClass != null)
        //{
        //    myPassiveClass.InitStat(charStat);
        //}
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
            //if (myPassiveClass != null)
            //{
            //    myPassiveClass.InitStat(charStat);
            //}
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

   
    public void Damage(int damage, bool isCrit = false)
    {
        // �������� ���� ü�� �����غ��� 
        MyCurrentHP -= damage;
        if(MyCurrentHP <= 0)
        {
            isDead = true;
        }
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
    public void InitializeSkillSlot()
    {
        skills.Clear();
        skills.Add(SkillSlotNumber.SLOT1, null);
        skills.Add(SkillSlotNumber.SLOT2, null);
        skills.Add(SkillSlotNumber.SLOT3, null);
        skills.Add(SkillSlotNumber.SLOT4, null);

        chainsSkills.Clear();
        chainsSkills.Add(SkillSlotNumber.CHAIN1, null);
        chainsSkills.Add(SkillSlotNumber.CHAIN2, null);
        chainsSkills.Add(SkillSlotNumber.CHAIN3, null);
    }


  
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
    public void SetSkill(Skill p_Target, int p_idx, bool isChain )
    {
        if (p_idx < 0 || skills.Count < p_idx)
            return;

        if (!isChain)
            skills[(SkillSlotNumber)p_idx] = p_Target;
        else
            chainsSkills[(SkillSlotNumber)p_idx] = p_Target;
    }

    // ��ų ���� ���� 
    public void UnequipSkill(Skill skill)
    {

        if (skill == null) return; 

        SkillSlotNumber slot = SkillSlotNumber.SLOT1;
        foreach (var skillPair in skills)
        {
            if (skillPair.Value == null) continue; 

            // ã�� ��ų�� �´ٸ� �ش� ���Կ��� ����
            if(skillPair.Value.Equals(skill))
            {
                slot = skillPair.Key;
                break; 
            }
        }

        foreach(var skillPair in chainsSkills)
        {
            if (skillPair.Value == null) continue;

            // ã�� ��ų�� �´ٸ� �ش� ���Կ��� ����
            if (skillPair.Value.Equals(skill))
            {
                slot = skillPair.Key;
                break;
            }
        }

        // �ش� ������ ��ų�� ü���� �ɷ� �ִ��� �˻�
        if (skills[slot] != null && skills[slot].IsChain == true)
        {
            // ü���̶�� ��� ü�� ��ų ���� 
            chainsSkills[SkillSlotNumber.CHAIN1] = null;
            chainsSkills[SkillSlotNumber.CHAIN2] = null;
            //chainsSkills[SkillSlotNumber.CHAIN3] = null;
        }

        // �ش� ������ ��ų ���� 
        if (slot >= SkillSlotNumber.SLOT1 && slot <= SkillSlotNumber.MAXSLOT)
        {
            skills[slot] = null;
        }
        else
        {
            chainsSkills[slot] = null; 
        }
    }


    // ��ų�� �����Ѵ�.
    public void EquipSkill(SkillSlotNumber slot, Skill skill, bool isChain = false)
    {
        // ��ų�� �����ϴµ� ��ų�� ������ ������� �Ѵ�. 
        if(skill== null)
        {
            if (slot >= SkillSlotNumber.SLOT1 && slot <= SkillSlotNumber.MAXSLOT)
            {
                skills[slot] = null;
            }
            else
            {
                chainsSkills[slot] = null; 
            }
        }
        // ��ų ������ �ִ� ��� 
        else 
        {
            // ���Կ� ��ġ Ȯ��
            if (slot >= SkillSlotNumber.SLOT1 && slot <= SkillSlotNumber.MAXSLOT)
            {
                if (skills.ContainsValue(skill))
                {
                    SkillSlotNumber prevSlot = SkillSlotNumber.SLOT1;
                    foreach (var skillPair in skills)
                    {
                        if (skill.keycode == skillPair.Value.keycode)
                        {
                            prevSlot = skillPair.Key;
                            break;
                        }
                    }

                    skills[prevSlot] = null;
                    skills[slot] = skill;
                }
                else
                {
                    skills[slot] = skill;
                }
            }
            else
            {
                if (chainsSkills.ContainsValue(skill))
                {
                    SkillSlotNumber prevSlot = SkillSlotNumber.CHAIN1;
                    foreach (var skillPair in chainsSkills)
                    {
                        if (skill.keycode == skillPair.Value.keycode)
                        {
                            prevSlot = skillPair.Key;
                            break;
                        }
                    }
                    chainsSkills[prevSlot] = null;
                    chainsSkills[slot] = skill;
                }
                else
                {
                    chainsSkills[slot] = skill;
                }
            }
        }
    }


    // �Ϲ� ��ų���� ü������ �Ǿ� �ִ��� ��� ��ȯ
    public bool GetSlotisChain(SkillSlotNumber slot)
    {
        if (skills[slot] == null) return false;

        return skills[slot].isChain;
    }


    // �Ϲ� ��ų ���Կ��� ü���� �� ��찡 �ִ��� ��ȯ
    public bool GetWasChianSkill()
    {
        foreach (var skillPair in skills)
        {
            if (skillPair.Value == null) continue;

            if (skillPair.Value.IsChain == true)
                return true;
        }

        return false; 
    }

    
    // ��ų keycode�� ������ �ش� ��ų�� �����ߴ��� ���� ��ȯ
    public bool CheckEquppiedSkillBySkillKeycode(string keycode)
    {
        foreach(var skillPair in skills)
        {
            if (skillPair.Value == null) continue; 

            if(skillPair.Value.keycode == keycode)
            {
                return true; 
            }
        }

        return false; 
    }
    // keycode�� ������ ü�� ��ų�� �����ߴ��� ���� ��ȯ 
    public bool CheckEquippedChainSkillBySkillKeycode(string keycode)
    {
        foreach (var skillPair in chainsSkills)
        {
            if (skillPair.Value == null) continue;

            if (skillPair.Value.keycode == keycode)
            {
                return true;
            }
        }

        return false;
    }


    // ù ��° ü�� ��ų�� keycode �� ��ȯ 
    public string GetFirstChainSkillID()
    {
        if(chainsSkills.ContainsKey(SkillSlotNumber.CHAIN1))
        {
            if (chainsSkills[SkillSlotNumber.CHAIN1] != null)
            {
                return chainsSkills[SkillSlotNumber.CHAIN1].keycode;
            }
        }

        return ""; 
    }

    // ������ ���Կ� ��ų�� ü�� ��ų ���
    public void SetChainSkillByNormalSlot(SkillSlotNumber slot)
    {
        if (skills.ContainsKey(slot) == false)
        {
            return; 
        }

        // ��ü ��ų����Ʈ���� ü���� �ɷ��ִ°� �ִٸ� ����
        foreach(var skill in skills)
        {
            if (skill.Value == null) continue;
            skill.Value.isChain = false; 
        }

        // �ش� ��ų ü�� 
        skills[slot].IsChain = true;
    }

    public void SetStartChainSkill(int p_TargetIdx)
    {
        chainIdx = p_TargetIdx;
    }


    public void ClearChains()
    {
        for (int i = 0; i < chainsSkills.Count; i++)
        {
            chainsSkills[(SkillSlotNumber)i] = null;
        }
    }

    // �нú� ��ų ����

    public  virtual void ApplyPassiveSkillEffects(Character target)
    {
        //if(myPassiveClass != null)
        //{
        //    myPassiveClass.ApplyPassiveSkillEffects(target);
        //}
    }

    public virtual void ApplyPassiveSkillEffectsByWC(WheelerController wheelerController)
    {
        
    }

    // �нú� ��ų ����
    public void SetPassiveSkill(PassiveSkill skill)
    {
        if (skill == null ) return;

        //myPassiveClass.SetPassiveSkill(skill);
        // ������ ��ų�� �ִ��� �˻� 
        var existSkill = equippedPassiveSkills.FirstOrDefault(passive => passive.keycode ==
        skill.keycode);
        if (existSkill != null)
        {
            existSkill = skill;
        }
        else
        {
            equippedPassiveSkills.Add(skill);
        }
        // ��ų ȿ�� ���� 
        ApplyPassiveSkillEffects(null);
    }

    // �нú� ��ų �˻� 
    public bool CheckLearendPassiveSkill(string keycode)
    {
        return equippedPassiveSkills.Any(skill => skill.keycode == keycode);
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
