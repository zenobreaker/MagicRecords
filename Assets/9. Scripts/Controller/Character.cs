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
    NONE = -1,
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
    private int playerID; // 캐릭터의 고유 ID

    [SerializeField]
    private CharStat charStat = null;
    private int playerHP;
    private int playerMP;
    private int playerCP;

    // 장착된 장비 
    public Dictionary<EquipType, EquipItem> equipItems =
        new Dictionary<EquipType, EquipItem>()
        {
            [EquipType.WEAPON] = null,
            [EquipType.ARMOR] = null,
            [EquipType.WHEEL] = null,
            [EquipType.ACCSESORRY_1] = null,
            [EquipType.ACCSESORRY_2] = null,
            [EquipType.ACCSESORRY_3] = null,
            [EquipType.DRONE] = null,

        };


    // 스킬 
    private int chainIdx;
    public Dictionary<SkillSlotNumber, Skill> skills = new Dictionary<SkillSlotNumber, Skill>();
    public Dictionary<SkillSlotNumber, Skill> chainsSkills = new Dictionary<SkillSlotNumber, Skill>();
    public List<PassiveSkill> equippedPassiveSkills = new List<PassiveSkill>();
    
    //public PassiveClass myPassiveClass;// 
    // 드론 
    public MagicalDrone drone;

    // 레코드 정보
    //public List<RecordInfo> selectRecordInfos = new List<RecordInfo>();


    public bool isAction = true;    //  flag
    public bool isDead = false;     //

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


    // Chain Point 
    public void InitCurrentCP()
    {
        MyCurrentCP = 0;    // 초기는 0으로 초기화
    }

    public int MyMaxCP
    {
        get { return MyStat.totalCP; }
    }

    public int MyCurrentCP
    {
        get { return playerCP; }
        set { 
            playerCP = value;
            if(playerCP <= 0)
            {
                playerCP = 0;
            }
        }
    }

    // ChainPoint 증가시키기
    public void IncreaseCP(int value)
    {
        MyCurrentCP += value;
        Debug.Log("현재 CP : " + MyCurrentCP);
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
        // 체력 감소
        MyCurrentHP -= damage;
        if(MyCurrentHP <= 0)
        {
            isDead = true;
        }
    }

    // 장비 슬롯 초기화
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

    // 스킬 슬롯 초기화
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


  
    // 해당 장비를 장착하고 있는지 체크
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

        // 사전에 자악되어 있는지 검사 
        if (equipItems.ContainsKey(equipItem.equipType) == true)
        {
            // 장착된 아이템을 해제한다. 
            RemoveEquipment(equipItem.equipType);
        }
        
        // 해당 슬롯에 해당 아이템 장비
        equipItems[equipItem.equipType] = equipItem;

        // 장비 능력치반영 작업 
        // 장비 메인 능력치를 적용한다. 
        MyStat.ApplyEquipOption(equipItem.itemMainAbility, true);
        // 장비의 서브 능력치를 적용시킨다. 
        foreach (var ability in equipItem.itemAbilities)
        {
            MyStat.ApplyEquipOption(ability, true);
        }

        MyStat.ApplyOption();
    }

    // 장착한 아이템 제거 
    public void RemoveEquipment(EquipType _typeSlot)
    {
        var equipItem = equipItems[_typeSlot];
        // 사전에 장착한 아이템이 있다면
        if (equipItem != null)
        {
            // 아이템 능력치를 처리한다. 
            MyStat.ApplyEquipOption(equipItem.itemMainAbility, false);
            foreach (var ability in equipItem.itemAbilities)
            {
                MyStat.ApplyEquipOption(ability, false);
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

            // 기존에 장착할려는 스킬을 찾아 그 슬롯 값을 찾는다. 
            if(skillPair.Value.Equals(skill))
            {
                slot = skillPair.Key;
                break; 
            }
        }

        foreach(var skillPair in chainsSkills)
        {
            if (skillPair.Value == null) continue;
            // 체인 스킬 슬롯에서도 찾아낸다. 
            if (skillPair.Value.Equals(skill))
            {
                slot = skillPair.Key;
                break;
            }
        }

        //  체인 스킬로 되어 있는 상태라면 
        if (skills[slot] != null && skills[slot].IsChain == true)
        {
            // 체인 스킬들을 전부 해제한다. 
            chainsSkills[SkillSlotNumber.CHAIN1] = null;
            chainsSkills[SkillSlotNumber.CHAIN2] = null;
            chainsSkills[SkillSlotNumber.CHAIN3] = null;
        }

        // 해당 슬롯에 스킬 정보를 제거 처리한다.
        if (slot >= SkillSlotNumber.SLOT1 && slot <= SkillSlotNumber.MAXSLOT)
        {
            skills[slot] = null;
        }
        else
        {
            chainsSkills[slot] = null; 
        }
    }


    // 스킬 장착
    public void EquipSkill(SkillSlotNumber slot, Skill skill, bool isChain = false)
    {
        // 스킬이 없다면 
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
        // 스킬이 있다면 
        else 
        {
            // 슬롯 범위 내로 검사
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


    // 스킬 슬롯 값을 받으면 체인 스킬로 되어 있는지 반환
    public bool GetSlotisChain(SkillSlotNumber slot)
    {
        if (skills[slot] == null) return false;

        return skills[slot].isChain;
    }


    // 자신의 전체 스킬 중 체인 스킬을 한 상태인지 반환
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

    
    //스킬 keycode 값을 받으면 장착한 스킬인지 반환한다. 
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
    // keycode값을 가져오면 장착된 체인 스킬인지 검사
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


    // 체인스킬의 시작 스킬의 ID값 반환
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

    // 해당 스킬 슬롯을 체인스킬로 지정한다.
    public void SetChainSkillByNormalSlot(SkillSlotNumber slot)
    {
        if (skills.ContainsKey(slot) == false)
        {
            return; 
        }

        // 이전에 체인 걸린 스킬슬롯을 해제
        foreach(var skill in skills)
        {
            if (skill.Value == null) continue;
            skill.Value.isChain = false; 
        }

        // 해당스킬 체인
        skills[slot].IsChain = true;
    }

    public void SetOffChainSkill(SkillSlotNumber slot)
    {
        if (skills.ContainsKey(slot) == false)
        {
            return;
        }

        skills[slot].IsChain = false;
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

    // 장착한 스킬 중 사용할 수 있는 상태의 스킬 슬롯 번호 반환.
    public ActiveSkill GetUsableSkill()
    {
        if (MyStat == null) return null;

        foreach (var skillPair in skills)
        {
            if (skillPair.Value == null ||
                skillPair.Value is ActiveSkill == false)
                continue;

            ActiveSkill skill = skillPair.Value as ActiveSkill;

            // 쿨다운 검사
            bool isCoolDown = skill.MyCoolDown;

            // 마나 검사 
            int cost = skill.SkillCost;
            bool isUsed = false;
            if (cost <= MyCurrentMP)
                isUsed = true;

            if (isCoolDown == true && isUsed == true)
                return skill;
        }

        return null;
    }

    // 사용할 수 있는 스킬 슬롯 번호 넘긴다 
    public SkillSlotNumber GetSkillSlotNumber()
    {
        if (MyStat == null) return SkillSlotNumber.NONE;

        foreach (var skillPair in skills)
        {
            if (skillPair.Value == null ||
                skillPair.Value is ActiveSkill == false)
                continue;

            ActiveSkill skill = skillPair.Value as ActiveSkill;

            // 쿨다운 검사
            bool isCoolDown = skill.MyCoolDown;

            // 마나 검사 
            int cost = skill.SkillCost;
            bool isUsed = false;
            if (cost <= MyCurrentMP)
                isUsed = true;

            if (isCoolDown == true && isUsed == true)
                return skillPair.Key;
        }

        return SkillSlotNumber.NONE;
    }

    // 스킬 슬롯 값을 받으면 받은 값이 아닌 스킬이 장착된 슬롯 반환
    public SkillSlotNumber GetAnotherSkillSlotNumber(SkillSlotNumber currentSlotNumber)
    {
        SkillSlotNumber target = SkillSlotNumber.NONE;

        foreach (var skillPair in skills)
        {
            if (skillPair.Value == null ||
                skillPair.Value is ActiveSkill == false ||
                currentSlotNumber == skillPair.Key)
                continue;

            target = skillPair.Key;
            break;
        }

        return target;
    }


    // 장착한 스킬 중 지정한 슬롯의 스킬을 반환한다.
    public ActiveSkill GetEquipedSkill(SkillSlotNumber slotNumber)
    {
        if (MyStat == null) return null;

        if (skills.TryGetValue(slotNumber, out var skill))
        {
            ActiveSkill activeSkill = skill as ActiveSkill;
            return activeSkill;
        }

        return null; 
    }
    
    // 패시브 스킬 관련

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

    // 패시브 스킬 장착
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

    // 패시브 스킬을 배웠었는지 검사
    public bool CheckLearendPassiveSkill(string keycode)
    {
        return equippedPassiveSkills.Any(skill => skill.keycode == keycode);
    }

    // 스탯 성장
    public void GrowUp(int _exp)
    {
        if(this.MyStat == null)
        {
            return; 
        }

       
        this.MyStat.GrowUp(_exp);
    }



    // 레코드 효과 발현
    public void ApplyRecordAbility(RecordInfo record)
    {
        if (record == null) return;

        // 옵션이 없다면
        if (record.specialOption == null)
        {
            if (RecordManager.instance == null) return;

            // 옵션을 찾아서 만들어준다.
            record.specialOption = RecordManager.instance.GetSpecialOptionToRecordInfo(record.specialOptionID);
        }


        this.MyStat.extraStat.ApplyBuffOptionStat(
        record.specialOption.abilityType, record.specialOption.value);

        MyStat.ApplyOption();
    }

    // 레코드 삭제
    public void RemoveRecordAbility(RecordInfo record)
    {
        if (record == null) return;

        this.MyStat.extraStat.ApplyBuffOptionStat(
            record.specialOption.abilityType, -record.specialOption.value);

        //selectRecordInfos.Remove(record);

        MyStat.ApplyOption();
    }

    // 깊은 복사 코드 
    public Character DeepCopy()
    {
        Character character = new Character();

        character.objectID = this.objectID;
        character.MyID = this.MyID;
        character.charStat = this.charStat.Clone(); 
        foreach(var equipItem in equipItems)
        {
            if (equipItem.Value == null)
                continue;

            character.EquipItem(equipItem.Value.Clone() as EquipItem);
        }

        character.SetStartChainSkill(this.chainIdx);
        character.skills = this.skills;
        character.chainsSkills = this.chainsSkills;
        character.equippedPassiveSkills = this.equippedPassiveSkills;
        if(drone != null)
            character.drone = this.drone.Clone();

        
        return character;
    }
}
