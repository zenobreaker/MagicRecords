using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 장착 슬롯 관련
//public enum EquipmentSlot
//{
//    WEAPON,         // 무기 
//    ARMOR,          // 장비
//    WHEEL,          // 바퀴
//    ACCSESORRY_1,   // 악세사리 1
//    ACCSESORRY_2,   // 악세사리 1
//    ACCSESORRY_3,   // 악세사리 3
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
    /*  플레이어 객체 정의
     *  CharStat과 EquipItem을 필드로 갖는다
    */
    public uint objectID;

    [SerializeField]
    private int playerID; // 플레이어 ID 장비 착용한 대상을 찾을 때 

    [SerializeField]
    private CharStat charStat = null;
    private int playerHP;
    private int playerMP;

    public List<BuffDebuff> buffDebuffs = new List<BuffDebuff>();
    //[SerializeField]
    // 캐릭터가 장착한 장비 
    public Dictionary<EquipType, EquipItem> equipItems = new Dictionary<EquipType, EquipItem>();

    // 캐릭터가 장착한 스킬 
    private int chainIdx;
    public Dictionary<SkillSlotNumber, Skill> skills = new Dictionary<SkillSlotNumber, Skill>();
    public Dictionary<SkillSlotNumber, Skill> chainsSkills = new Dictionary<SkillSlotNumber, Skill>();

    // 장착한 드론
    public MagicalDrone drone;

    // 해당 캐릭터가 적용받는 레코드 목록 < 이건 추후에 유물로 변경 
    public List<RecordInfo> selectRecordInfos = new List<RecordInfo>();

    public Character()
    {
        InitailizeEquipment();
        InitializeSkillSlot();
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

    // 버프 관련
    public void ApplyBuffDebuff(BuffDebuff buffDebuff)
    {
        if (buffDebuff == null) return;
        buffDebuffs.Add(buffDebuff);

        buffDebuff.Execute(this);
    }

    public void RemoveBuffDebuff(BuffDebuff buffDebuff)
    {
        buffDebuffs.Remove(buffDebuff);
    }

    // 장착 장비 초기화 
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

    // 장착 스킬 초기화 
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

    // 장착하려는 아이템이 있는지 검사 
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


    // 장비 장착 
    public void EquipItem(EquipItem equipItem)
    {
        if (equipItem == null) return;

        // 기존에 해당 부위에 장비를 장착했는지 검사
        if (equipItems.ContainsKey(equipItem.equipType) == true)
        {
            // 기존에 장착한 장비 정보 해제
            RemoveEquipment(equipItem.equipType);
        }
        
        // 장비 장착
        equipItems[equipItem.equipType] = equipItem;

        // 아이템 능력치 적용 
        // 메인
        MyStat.extraStat.ApplyOptionExtraStat(equipItem.itemMainAbility);
        // 서브
        foreach (var ability in equipItem.itemAbilities)
        {
            MyStat.extraStat.ApplyOptionExtraStat(ability);
        }

        MyStat.ApplyOption();
    }

    // 장비 해제 
    public void RemoveEquipment(EquipType _typeSlot)
    {
        var equipItem = equipItems[_typeSlot];
        // 아이템 능력치 적용 
        if (equipItem != null)
        {
            // 메인
            MyStat.extraStat.ApplyOptionExtraStat(equipItem.itemMainAbility, true);
            // 서브
            foreach (var ability in equipItem.itemAbilities)
            {
                MyStat.extraStat.ApplyOptionExtraStat(ability, true);
            }

            MyStat.ApplyOption();
        }
        equipItems[_typeSlot] = null;
    }
 
    // 스킬 장착 
    public void SetSkill(Skill p_Target, int p_idx, bool isChain )
    {
        if (p_idx < 0 || skills.Count < p_idx)
            return;

        if (!isChain)
            skills[(SkillSlotNumber)p_idx] = p_Target;
        else
            chainsSkills[(SkillSlotNumber)p_idx] = p_Target;
    }

    // 스킬 장착 해제 
    public void UnequipSkill(Skill skill)
    {

        if (skill == null) return; 

        SkillSlotNumber slot = SkillSlotNumber.SLOT1;
        foreach (var skillPair in skills)
        {
            if (skillPair.Value == null) continue; 

            // 찾는 스킬이 맞다면 해당 슬롯에서 제거
            if(skillPair.Value.Equals(skill))
            {
                slot = skillPair.Key;
                break; 
            }
        }

        // 해당 슬롯의 스킬이 체인이 걸려 있는지 검사
        if (skills[slot] != null && skills[slot].IsChain == true)
        {
            // 체인이라면 모든 체인 스킬 해제 
            chainsSkills[SkillSlotNumber.CHAIN1] = null;
            chainsSkills[SkillSlotNumber.CHAIN2] = null;
            chainsSkills[SkillSlotNumber.CHAIN3] = null;
        }

        // 해당 슬롯의 스킬 제거 
        skills[slot] = null; 
    }

    public void SetSkill(SkillSlotNumber slot, Skill skill, bool isChain = false)
    {
        // 스킬을 세팅하는데 스킬이 없으면 비어지게 한다. 
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
        // 스킬 정보가 있는 경우 
        else 
        {
            // 슬롯에 위치 확인
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

    
    // 스킬 keycode를 받으면 해당 스킬을 장착했는지 여부 반환
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
    // keycode를 받으면 체인 스킬로 장착했는지 여부 반환 
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



    // 캐릭터 경험치 세팅시키고 경험치가 되면 레벨업을 시킨다.  
    public void GrowUp(int _exp)
    {
        if(this.MyStat == null)
        {
            return; 
        }

        // 이 캐릭터의 스탯클래스에 경험치 지급
        this.MyStat.GrowUp(_exp);
    }



    // 레코드 능력 적용 
    public void ApplyRecordAbility(RecordInfo record)
    {
        if (record == null || selectRecordInfos.Contains(record) == true) return;

        // 효과 정보가 있는지 검사 
        if (record.specialOption == null)
        {
            // 없다면 매니저를 통해 할당시킨다. 
            if (RecordManager.instance == null) return;

            record.specialOption = RecordManager.instance.GetSpecialOptionToRecordInfo(record.specialOptionID);
        }


        this.MyStat.extraStat.ApplyOptionExtraStat(
        record.specialOption.abilityType, record.specialOption.value, record.specialOption.isPercentage);

        MyStat.ApplyOption();
    }

    // 레코드 능력 적용 제거
    public void RemoveRecordAbility(RecordInfo record)
    {
        if (record == null) return;

        this.MyStat.extraStat.ApplyOptionExtraStat(
            record.specialOption.abilityType, -record.specialOption.value, record.specialOption.isPercentage);

        //selectRecordInfos.Remove(record);

        MyStat.ApplyOption();
    }
}
