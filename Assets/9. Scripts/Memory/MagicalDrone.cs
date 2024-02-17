using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.XR;

// 휠러에게 직접적으로 장착되면 여러 부가 효과를 일으키는 드론!
// 메모리 룬이란 걸 장착하며 드론마다 발동하는 효과가 다르다! 
public class MagicalDrone : EquipItem
{
    public int maxSlotCount;                    // 드론에 장착할 수 있는 룬의 최대 개수
    public int droneFirstEffectRuneCount;       // 드론의 1 옵션 발생하는 룬 개수 
    public int droneSecondEffectRuneCount;      // 드론의 2 옵션 발생하는 룬 개수 
    public int droneThirdEffectRuneCount;       // 드론의 3 옵션 발생하는 룬 개수 



    public Dictionary<int, MemoryRune> dic_RuneSlots;
    public ExtraStat extraStat = new ExtraStat();

    // 세트 효과 개수 
    // 드론도 파츠를 나눈다 
    public MagicalDrone(int _itemUID, string _keycode, string _itemName, ItemType _itemTpye,
        ItemRank _itemRank, string _itemDesc, int _itemEach, int _itemValue,
        string _itemIMG, EquipType equipType, int itemEnchantRank, 
        bool isEquiped, ItemAbility itemMainAbility, bool _isSale = false) :
        base(_itemUID, _keycode, _itemName, _itemTpye, _itemRank, _itemDesc, _itemEach, 
            _itemValue, _itemIMG, equipType, itemEnchantRank, isEquiped, itemMainAbility, _isSale)
    {

    }

    // 드론을 생성한다. 
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

    // 드론은 룬을 장착할 수 있는 슬롯 수가 드론 별로 정해져 있다. 
    // 룬은 어떠한 룬이라도 상관 없이 요구 옵션 슬롯 수만 채운다면 드론은 효과를 발생한다 

    // 슬롯을 일정 n/n'/n'' 만큼 채우면 효과가 발동된다 (드론마다 상이함) 

    // 드론 초기화 
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
    
    // 장착한 룬 리스트 반환
    public Dictionary<int, MemoryRune> GetRuneDicionary()
    {
        return dic_RuneSlots;
    }

    // 드론에 장착된 룬 옵션 적용 
    public void ApplyRuneOption()
    {
        foreach(var pair in dic_RuneSlots)
        {
            if (pair.Value == null) continue;

            // 대상이 메모리룬 형태가 아니라면 넘긴다 
            if ((pair.Value is MemoryRune) == false) continue;

            var rune = pair.Value;
            // 룬의 형태에 따라서 extraStat에 적절히 추가한다. 
            extraStat.ApplyOptionExtraStat(rune.itemMainAbility.abilityType,
                rune.itemMainAbility.power);
        }
    }


    // 이 드론의 고유한 효과를 발현한다
    public void ApplyDroneEffect()
    {
        // 첫 번째 옵션 발동 
        if(droneFirstEffectRuneCount > 0  && dic_RuneSlots.Count >= droneFirstEffectRuneCount)
        {
            ApplyFirstDroneBonusEffect(); 
        }

        // 두 번째 옵션 발동 
        if (droneSecondEffectRuneCount > 0 && dic_RuneSlots.Count >= droneSecondEffectRuneCount)
        {
            ApplySecondDroneBonusEffect(); 
        }

        // 세 번째 옵션 발동 
        if (droneThirdEffectRuneCount > 0 && dic_RuneSlots.Count >= droneThirdEffectRuneCount)
        {
            ApplyThirdDroneBonusEffect(); 
        }
    }


    // 드론의 첫 번째 옵션을 발생한다 
    public virtual void ApplyFirstDroneBonusEffect()
    {
       
    }

    // 드론의 두 번째 옵션을 발생한다 
    public virtual void ApplySecondDroneBonusEffect()
    {
        
    }

    // 드론의 세 번째 옵션을 발생한다 
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
