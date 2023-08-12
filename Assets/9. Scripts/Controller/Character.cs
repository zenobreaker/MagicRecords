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
    private Dictionary<EquipType, EquipItem> equipItems = new Dictionary<EquipType, EquipItem>();
    public Dictionary<EquipType, int> equipSlotList = new Dictionary<EquipType, int>();

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
        equipSlotList.Clear();
        equipItems.Add(EquipType.WEAPON, null);
        equipSlotList.Add(EquipType.WEAPON, 0);
        equipItems.Add(EquipType.ARMOR, null);
        equipSlotList.Add(EquipType.ARMOR, 0);
        equipItems.Add(EquipType.WHEEL, null);
        equipSlotList.Add(EquipType.WHEEL, 0);
        equipItems.Add(EquipType.ACCSESORRY_1, null);
        equipSlotList.Add(EquipType.ACCSESORRY_1, 0);
        equipItems.Add(EquipType.ACCSESORRY_2, null);
        equipSlotList.Add(EquipType.ACCSESORRY_2, 0);
        equipItems.Add(EquipType.ACCSESORRY_3, null);
        equipSlotList.Add(EquipType.ACCSESORRY_3, 0);
        equipItems.Add(EquipType.DRONE, null);
        equipSlotList.Add(EquipType.DRONE, 0);
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



    //public void ChangeEquimnet(EquipType _typeSlot, ref EquipItem _equipItem, bool isEquip)
    //{
    //    if (isEquip == true)
    //    {
    //        SetEquipment(_typeSlot, ref _equipItem);
    //    }
    //    else
    //    {
    //        SetEmptyEquip(_typeSlot, ref _equipItem);
    //    }
            
    //}

    // 장비 장착 
    public void SetEquipment(EquipType _typeSlot, ref EquipItem _equipItem)
    {
        if (_equipItem == null) return; 

        switch (_typeSlot)
        {
            case EquipType.WEAPON:
                {
                    equipItems[EquipType.WEAPON] = _equipItem;
                    equipSlotList[EquipType.WEAPON] = _equipItem.itemUID;
                }
                break;
            case EquipType.ARMOR:
                {
                    equipItems[EquipType.ARMOR] = _equipItem;
                    equipSlotList[EquipType.ARMOR] = _equipItem.itemUID;
                }
                break;
            case EquipType.WHEEL:
                {
                    equipItems[EquipType.WHEEL] = _equipItem;
                    equipSlotList[EquipType.WHEEL] = _equipItem.itemUID;
                }
                break;
            case EquipType.ACCSESORRY_1:
                {
                    equipItems[EquipType.ACCSESORRY_1] = _equipItem;
                    equipSlotList[EquipType.ACCSESORRY_1] = _equipItem.itemUID;
                }
                break;
            case EquipType.ACCSESORRY_2:
                {
                    equipItems[EquipType.ACCSESORRY_2] = _equipItem;
                    equipSlotList[EquipType.ACCSESORRY_2] = _equipItem.itemUID;
                }
                break;
            case EquipType.ACCSESORRY_3:
                {
                    equipItems[EquipType.ACCSESORRY_3] = _equipItem;
                    equipSlotList[EquipType.ACCSESORRY_3] = _equipItem.itemUID;
                }
                break;
            case EquipType.DRONE:
                {
                    equipItems[EquipType.DRONE] = _equipItem;
                    equipSlotList[EquipType.DRONE] = _equipItem.itemUID;
                }
                break;

        }
        _equipItem.uniqueID = this.MyID;
        _equipItem.isEquip = true;

        // 아이템 능력치 적용 
        
    }

    // 아이템 장착 해제 

    public void RemoveEquipment(EquipType _typeSlot)
    {
        var _equipItem = equipItems[_typeSlot];
        equipItems[_typeSlot] = null;
        equipSlotList[_typeSlot] = 0;
        // todo db 관련 통신 후 처리하게 해보기 
        if (_equipItem != null)
        {
            _equipItem.isEquip = false; 
        }
    }
    public void SetEmptyEquip(EquipType _typeSlot, ref EquipItem _equipItem)
    {
        switch (_typeSlot)
        {
            case EquipType.WEAPON:
                {
                    _equipItem = equipItems[EquipType.WEAPON];
                    equipItems[EquipType.WEAPON] = null;
                    equipSlotList[EquipType.WEAPON] = 0;
                }
                break;
            case EquipType.ARMOR:
                {
                    _equipItem = equipItems[EquipType.ARMOR];
                    equipItems[EquipType.ARMOR] = null;
                    equipSlotList[EquipType.ARMOR] = 0;
                }
                break;
            case EquipType.WHEEL:
                {

                    _equipItem = equipItems[EquipType.WHEEL];
                    equipItems[EquipType.WHEEL] = null;
                    equipSlotList[EquipType.WHEEL] = 0;
                }
                break;
            case EquipType.ACCSESORRY_1:
                {
                    _equipItem = equipItems[EquipType.ACCSESORRY_1];
                    equipItems[EquipType.ACCSESORRY_1] = null;
                    equipSlotList[EquipType.ACCSESORRY_1] = 0;
                }
                break;
            case EquipType.ACCSESORRY_2:
                {
                    _equipItem = equipItems[EquipType.ACCSESORRY_2];
                    equipItems[EquipType.ACCSESORRY_2] = null;
                    equipSlotList[EquipType.ACCSESORRY_2] = 0;
                }
                break;
            case EquipType.ACCSESORRY_3:
                {
                    _equipItem = equipItems[EquipType.ACCSESORRY_3];
                    equipItems[EquipType.ACCSESORRY_3] = null;
                    equipSlotList[EquipType.ACCSESORRY_3] = 0;
                }
                break;
            case EquipType.DRONE:
                {
                    _equipItem = equipItems[EquipType.DRONE];
                    equipItems[EquipType.DRONE] = null;
                    equipSlotList[EquipType.DRONE] = 0;
                }
                break;

        }

        if (_equipItem != null)
        {
            _equipItem.isEquip = false; 
        }
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
