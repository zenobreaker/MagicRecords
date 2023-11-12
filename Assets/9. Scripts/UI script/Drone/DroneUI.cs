using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEngine;

public class DroneUI : MonoBehaviour
{
    // ��� UI �� �׸��� 

    // ����� ������ ĳ����
    private readonly WheelerController selectedCharacter;

    // ĳ���Ϳ��� ���� ���
    MagicalDrone selectedDrone;

    // ������ ���� ��ȣ
    public int selctedSlotNumber = 0; 
    // �׷����� ���� ������Ʈ ������
    InvenSlot pf_Slot;

    GameObject selectDroneObject;       // ���õ� ��� ������Ʈ 

    public GameObject go_magicalDrone_2Slot;
    public GameObject go_magicalDrone_4Slot;
    public GameObject go_magicalDrone_6Slot; 


    // ó���� �� UI�� ������ ����� ���õǾ��� �ִ�. 
    // �κ��丮�� ����� �����ϰ� �� ������ �ϸ� �� �������� �´� .
    // �׳� ���� ������ �õ��� ����� �����Ƿ� ����� �����ϵ��� ����������� ��ģ�� 

    // ��� ������Ʈ ��ġ
    public void InitializeDroneObject()
    {
        if (selectedDrone == null) return;

        // ��� ������Ʈ���� �� �����´� 
        go_magicalDrone_2Slot.SetActive(false);
        go_magicalDrone_4Slot.SetActive(false);
        go_magicalDrone_6Slot.SetActive(false);

        switch (selectedDrone.maxSlotCount)
        {
            case 2: // �� ���� ������ ���� ���
                go_magicalDrone_2Slot.SetActive(true);
                selectDroneObject = go_magicalDrone_2Slot;
                break;
            case 4:     // �� ���� ������ ���� ���
                go_magicalDrone_4Slot.SetActive(true);
                selectDroneObject = go_magicalDrone_4Slot;
                break;
            default:    // �׿ܿ� 6��
                go_magicalDrone_6Slot.SetActive(true);
                selectDroneObject = go_magicalDrone_6Slot;
                break;

        }


        // ������ �׸��� ���� ����� �ٽ� �׸���.  
        DrawDroneForm();

    }

    // ����� ������ ĳ���Ϳ��� �������´�
    public void SetDroneToCharacter()
    {
        if (selectedCharacter == null || selectedDrone == null) return;

        //selectedDrone = selectedCharacter.GetMyDrone(); 
    }

    // ������ ��п� ���� ���Ե��� �׸��� ���� ��� 2�� ������ ���� ����̸� 2������ ���δ� 
    public void DrawDroneForm()
    {
        if (selectedCharacter == null || selectedDrone == null) return;

        // ��п� ������ �� ������ ������ �׸��� 

        var runeDic = selectedDrone.GetRuneDicionary();
        if (runeDic.Count <= 0) return;

        // ���� ��ŭ ���ĵǼ� ���̴� ������� �׷����Ѵ�
        // �׷� �������ִ� �����ͺ��̽����� �����Ƿ� �׳� �ϱ�
        if (selectDroneObject == null) return;

        var droneObject = selectDroneObject.GetComponent<DroneObject>();
        if (droneObject == null) return;

        // ����� ������ �׷��ش� 
        for (int i = 0; i < runeDic.Count; i++)
        {
            var slot = droneObject.slots[i];
            if (slot == null) continue;

            slot.AddItem(runeDic[i]);
        }
 
    }
    
    // ����� ���Կ� ������ ���� ����
    public void EquipTargetDroneSlotWithRune(ref MemoryRune _selectedRune)
    {
        if(_selectedRune == null || selctedSlotNumber <= 0) return;

        _selectedRune.isEquip = true;     // ��� �鿡 ���� ���� 
        // ��п� �� ���� 
        selectedDrone.EquipRune(selctedSlotNumber, ref _selectedRune);

        // ������ �׸��� ���� ����� �ٽ� �׸���.  
        DrawDroneForm(); 
    }


    // ������ ����� ���Կ� ���� ���� 
    public void UnequipTargetDroneSlot()
    {
        if (selectedDrone == null || selctedSlotNumber <= 0) return;

        selectedDrone.UnequipRune(selctedSlotNumber);
        
        // ������ �׸��� ���� ����� �ٽ� �׸���.  
        DrawDroneForm();
    }

}
