using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions.Must;
using static UnityEditor.Progress;

public class EquipManager : MonoBehaviour
{
    public static EquipManager instance;

    public Character targetPlayer;     // 장착할 대상의 능력치

    // 장비 장착 기능을 이용해서 장착 장비 세팅 후 인벤토리 장착 아이템 갱신시키느 기능 추가하기
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        // DontDestroyOnLoad(this.gameObject);
    }


    // 아이템 장착실행 
    public void RunEquipItem(Character target, EquipType slotType, Item item)
    {
        if (target == null || slotType == EquipType.NONE) return;

        //// 대상에 장착하려는 슬롯에 자리를 비워놓는다.
        //var prevItem = _target.GetEquipItemSlot(_slot);

        //if (prevItem != null && (prevItem is EquipItem) == true)
        //{
        //    (prevItem as EquipItem).isEquip = false;
        //    // 인벤토리에서 해당 아이템의 데이터를 수정
        //    InventoryManager.instance.RefreshItemInfo(ref prevItem);
        //}

        // 대상에게 아이템을 장착시킨다. 
        if (item != null)
        {
            var equipment = item as EquipItem;
            if (item != null && (item is EquipItem) == true)
            {
                // 캐릭터에게 장착 
                target.EquipItem(equipment);
                // 장착한 아이템 능력치 적용 
                // 해당 아이템 정보 갱신 
                InventoryManager.instance.RefreshItemInfo(ref item);
            }
        }
        // 아이템이 없다면 
        else
        {
            target.RemoveEquipment(slotType);
        }

    }


    public void SetTargetStat(CharStat _target)
    {
        targetPlayer.MyStat = _target;
    }

    public ExtraStat CalcStatToPercent(Character p_TargetPlayer, ExtraStat p_Extra)
    {
        if (p_TargetPlayer == null || p_Extra == null) return null;

        CharStat t_stat = new CharStat(p_TargetPlayer.MyStat);

        p_Extra.extraAttack = Mathf.FloorToInt((float)t_stat.attack * ((float)p_Extra.extraAttack / 100));
        p_Extra.extraAttackSpeed = Mathf.Floor((float)t_stat.attackSpeed * ((float)p_Extra.extraAttackSpeed / 100));
        p_Extra.extraDefense = Mathf.FloorToInt(t_stat.defense * (p_Extra.extraDefense / 100));
        p_Extra.extraHP = Mathf.FloorToInt((float)t_stat.hp * ((float)p_Extra.extraHP / 100));
        p_Extra.extraMP = Mathf.FloorToInt((float)t_stat.mp * ((float)p_Extra.extraMP / 100));
        p_Extra.extraHPR = Mathf.FloorToInt((float)t_stat.hpRegen * ((float)p_Extra.extraHPR / 100));
        p_Extra.extraMPR = Mathf.FloorToInt((float)t_stat.mpRegen * ((float)p_Extra.extraMPR / 100));


        return p_Extra;

        //InfoManual.MyInstance.GetSelectedPlayer().MyStat.extraStat.SetStatus(p_Extra);
    }

}
