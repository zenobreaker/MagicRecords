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
    private Dictionary<EquipType, EquipItem> equipItems = new Dictionary<EquipType, EquipItem>();
    public Dictionary<EquipType, int> equipSlotList = new Dictionary<EquipType, int>();

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

    // ��� ���� 
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

        // ������ �ɷ�ġ ���� 
        
    }

    // ������ ���� ���� 

    public void RemoveEquipment(EquipType _typeSlot)
    {
        var _equipItem = equipItems[_typeSlot];
        equipItems[_typeSlot] = null;
        equipSlotList[_typeSlot] = 0;
        // todo db ���� ��� �� ó���ϰ� �غ��� 
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
