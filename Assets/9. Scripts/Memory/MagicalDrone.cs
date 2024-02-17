using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.XR;

// �ٷ����� ���������� �����Ǹ� ���� �ΰ� ȿ���� ����Ű�� ���!
// �޸� ���̶� �� �����ϸ� ��и��� �ߵ��ϴ� ȿ���� �ٸ���! 
public class MagicalDrone : EquipItem
{
    public int maxSlotCount;                    // ��п� ������ �� �ִ� ���� �ִ� ����
    public int droneFirstEffectRuneCount;       // ����� 1 �ɼ� �߻��ϴ� �� ���� 
    public int droneSecondEffectRuneCount;      // ����� 2 �ɼ� �߻��ϴ� �� ���� 
    public int droneThirdEffectRuneCount;       // ����� 3 �ɼ� �߻��ϴ� �� ���� 



    public Dictionary<int, MemoryRune> dic_RuneSlots;
    public ExtraStat extraStat = new ExtraStat();

    // ��Ʈ ȿ�� ���� 
    // ��е� ������ ������ 
    public MagicalDrone(int _itemUID, string _keycode, string _itemName, ItemType _itemTpye,
        ItemRank _itemRank, string _itemDesc, int _itemEach, int _itemValue,
        string _itemIMG, EquipType equipType, int itemEnchantRank, 
        bool isEquiped, ItemAbility itemMainAbility, bool _isSale = false) :
        base(_itemUID, _keycode, _itemName, _itemTpye, _itemRank, _itemDesc, _itemEach, 
            _itemValue, _itemIMG, equipType, itemEnchantRank, isEquiped, itemMainAbility, _isSale)
    {

    }

    // ����� �����Ѵ�. 
    public MagicalDrone CreateDrone(int _itemUID, string _keycode, string _itemName, ItemType _itemType, 
        ItemRank _itemRank, int _maxSlotCount)
    {
        MagicalDrone drone;
        ItemAbility itemAbility;
        itemAbility.isPercent = false;
        itemAbility.abilityType = AbilityType.NONE;
        itemAbility.power = 0;

        maxSlotCount = _maxSlotCount;

        drone = new MagicalDrone(_itemUID, _keycode, _itemName, _itemType, _itemRank, "", 1, 0, "", EquipType.DRONE,
            0, false, itemAbility);

        return drone;
    }

    // ����� ���� ������ �� �ִ� ���� ���� ��� ���� ������ �ִ�. 
    // ���� ��� ���̶� ��� ���� �䱸 �ɼ� ���� ���� ä��ٸ� ����� ȿ���� �߻��Ѵ� 

    // ������ ���� n/n'/n'' ��ŭ ä��� ȿ���� �ߵ��ȴ� (��и��� ������) 

    // ��� �ʱ�ȭ 
    public void InitializeDrone()
    {
        dic_RuneSlots = new Dictionary<int, MemoryRune>(); 

    }


    public void EquipRune(int _slotNumber, ref MemoryRune _rune)
    {
        if (dic_RuneSlots.Count <= 0) return;

        dic_RuneSlots.Add(_slotNumber, _rune);
    }

    public void UnequipRune(int _slotNumber)
    {
        if (dic_RuneSlots.Count <= 0) return;

        var rune = dic_RuneSlots[_slotNumber];
        rune.isEquip = false;

        dic_RuneSlots.Remove(_slotNumber);
    }
    
    // ������ �� ����Ʈ ��ȯ
    public Dictionary<int, MemoryRune> GetRuneDicionary()
    {
        return dic_RuneSlots;
    }

    // ��п� ������ �� �ɼ� ���� 
    public void ApplyRuneOption()
    {
        foreach(var pair in dic_RuneSlots)
        {
            if (pair.Value == null) continue;

            // ����� �޸𸮷� ���°� �ƴ϶�� �ѱ�� 
            if ((pair.Value is MemoryRune) == false) continue;

            var rune = pair.Value;
            // ���� ���¿� ���� extraStat�� ������ �߰��Ѵ�. 
            extraStat.ApplyOptionExtraStat(rune.itemMainAbility.abilityType,
                rune.itemMainAbility.power);
        }
    }


    // �� ����� ������ ȿ���� �����Ѵ�
    public void ApplyDroneEffect()
    {
        // ù ��° �ɼ� �ߵ� 
        if(droneFirstEffectRuneCount > 0  && dic_RuneSlots.Count >= droneFirstEffectRuneCount)
        {
            ApplyFirstDroneBonusEffect(); 
        }

        // �� ��° �ɼ� �ߵ� 
        if (droneSecondEffectRuneCount > 0 && dic_RuneSlots.Count >= droneSecondEffectRuneCount)
        {
            ApplySecondDroneBonusEffect(); 
        }

        // �� ��° �ɼ� �ߵ� 
        if (droneThirdEffectRuneCount > 0 && dic_RuneSlots.Count >= droneThirdEffectRuneCount)
        {
            ApplyThirdDroneBonusEffect(); 
        }
    }


    // ����� ù ��° �ɼ��� �߻��Ѵ� 
    public virtual void ApplyFirstDroneBonusEffect()
    {
       
    }

    // ����� �� ��° �ɼ��� �߻��Ѵ� 
    public virtual void ApplySecondDroneBonusEffect()
    {
        
    }

    // ����� �� ��° �ɼ��� �߻��Ѵ� 
    public virtual void ApplyThirdDroneBonusEffect()
    {

    }

    public new MagicalDrone Clone()
    {
        MagicalDrone md = CreateDrone(this.itemUID, this.itemKeycode, this.itemName, this.itemType,
           this.itemRank, this.maxSlotCount);

        foreach(var rune in dic_RuneSlots)
        {
            if (rune.Value == null)
                continue; 

            md.dic_RuneSlots.Add(rune.Key, rune.Value.Clone());
        }

        md.extraStat = this.extraStat.Clone();
        md.droneFirstEffectRuneCount = this.droneFirstEffectRuneCount;
        md.droneSecondEffectRuneCount = this.droneSecondEffectRuneCount;
        md.droneThirdEffectRuneCount = this.droneThirdEffectRuneCount;

        return md; 
    }
}
