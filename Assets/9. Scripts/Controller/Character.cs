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


    //[SerializeField]
    // 캐릭터가 장착한 장비 
    public Dictionary<EquipType, EquipItem> equipItems = new Dictionary<EquipType, EquipItem>();

    // 캐릭터가 장착한 스킬 
    private int chainIdx;
    private Skill[] skills = new Skill[4];
    private Skill[] chainsSkills = new Skill[3];

    // 장착한 드론
    public MagicalDrone drone;

    // 해당 캐릭터가 적용받는 레코드 목록 < 이건 추후에 유물로 변경 
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
