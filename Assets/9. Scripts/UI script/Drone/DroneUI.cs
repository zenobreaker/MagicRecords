using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEngine;

public class DroneUI : MonoBehaviour
{
    // 드론 UI 를 그린다 

    // 드론을 장착한 캐릭터
    private readonly CharacterController selectedCharacter;

    // 캐릭터에서 꺼낸 드론
    MagicalDrone selectedDrone;

    // 선택한 슬롯 번호
    public int selctedSlotNumber = 0; 
    // 그려야할 슬롯 오브젝트 프리팹
    InvenSlot pf_Slot;

    GameObject selectDroneObject;       // 선택된 드론 오브젝트 

    public GameObject go_magicalDrone_2Slot;
    public GameObject go_magicalDrone_4Slot;
    public GameObject go_magicalDrone_6Slot; 


    // 처음에 이 UI는 장착할 드론이 선택되어져 있다. 
    // 인벤토리에 드론을 선택하고 룬 장착을 하면 이 페이지로 온다 .
    // 그냥 오면 장착을 시도한 드론이 없으므로 드론을 선택하도록 드론페이지를 펼친다 

    // 드론 오브젝트 배치
    public void InitializeDroneObject()
    {
        if (selectedDrone == null) return;

        // 모든 오브젝트들을 다 꺼놓는다 
        go_magicalDrone_2Slot.SetActive(false);
        go_magicalDrone_4Slot.SetActive(false);
        go_magicalDrone_6Slot.SetActive(false);

        switch (selectedDrone.maxSlotCount)
        {
            case 2: // 두 개의 슬롯을 가진 드론
                go_magicalDrone_2Slot.SetActive(true);
                selectDroneObject = go_magicalDrone_2Slot;
                break;
            case 4:     // 네 개의 슬롯을 가진 드론
                go_magicalDrone_4Slot.SetActive(true);
                selectDroneObject = go_magicalDrone_4Slot;
                break;
            default:    // 그외엔 6개
                go_magicalDrone_6Slot.SetActive(true);
                selectDroneObject = go_magicalDrone_6Slot;
                break;

        }


        // 슬롯을 그리기 위해 드론을 다시 그린다.  
        DrawDroneForm();

    }

    // 드론을 선택한 캐릭터에서 꺼내놓는다
    public void SetDroneToCharacter()
    {
        if (selectedCharacter == null || selectedDrone == null) return;

        //selectedDrone = selectedCharacter.GetMyDrone(); 
    }

    // 장착한 드론에 따른 슬롯들을 그린다 예를 들면 2룬 슬롯을 가진 드론이면 2슬롯이 보인다 
    public void DrawDroneForm()
    {
        if (selectedCharacter == null || selectedDrone == null) return;

        // 드론에 장착된 룬 개수를 가져와 그린다 

        var runeDic = selectedDrone.GetRuneDicionary();
        if (runeDic.Count <= 0) return;

        // 개수 만큼 정렬되서 보이는 모습으로 그려야한다
        // 그런 세팅해주는 데이터베이스값이 없으므로 그냥 하기
        if (selectDroneObject == null) return;

        var droneObject = selectDroneObject.GetComponent<DroneObject>();
        if (droneObject == null) return;

        // 드론의 슬롯을 그려준다 
        for (int i = 0; i < runeDic.Count; i++)
        {
            var slot = droneObject.slots[i];
            if (slot == null) continue;

            slot.AddItem(runeDic[i]);
        }
 
    }
    
    // 드론의 슬롯에 선택한 룬을 장착
    public void EquipTargetDroneSlotWithRune(ref MemoryRune _selectedRune)
    {
        if(_selectedRune == null || selctedSlotNumber <= 0) return;

        _selectedRune.isEquip = true;     // 대상 룬에 값을 변경 
        // 드론에 룬 장착 
        selectedDrone.EquipRune(selctedSlotNumber, ref _selectedRune);

        // 슬롯을 그리기 위해 드론을 다시 그린다.  
        DrawDroneForm(); 
    }


    // 선택한 드론의 슬롯에 룬을 해제 
    public void UnequipTargetDroneSlot()
    {
        if (selectedDrone == null || selctedSlotNumber <= 0) return;

        selectedDrone.UnequipRune(selctedSlotNumber);
        
        // 슬롯을 그리기 위해 드론을 다시 그린다.  
        DrawDroneForm();
    }

}
