using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{
    [SerializeField]
    private float range = 0f; // 습득 가능한 최대 거리.

    private bool pickupActivated = false; // 습득 가능할 시 true;

    private RaycastHit hitInfo; // 충돌체 정보 저장.

    // 아이템 레이어만 반응하도록 레이어 마스크를 설정.
    [SerializeField]
    private LayerMask layerMask = 0;

    // 필요한 컴포넌트. 
    [SerializeField]
    private Text actionText = null;

    public Transform go_Transform = null;
   
    // Update is called once per frame
    void Update()
    {
        
        if (GameManager.isCharacterOn && go_Transform != null)
        {
            CheckItem();
            TryAction();
        }
      
    }

    public void SetTransform(Transform _tf)
    {
        go_Transform = _tf;
    }

    private void TryAction()
    {
        if (Input.GetKeyDown(KeyCode.E) )
        {
            CheckItem();
            CanPickUp();
        }
    }

    private void CanPickUp()
    {
        if (pickupActivated)
        {
            if(hitInfo.transform != null)
            {
                SoundManager.instance.PlaySE(hitInfo.transform.GetComponent<ItemPickUp>().item.itemSound);
                Debug.Log(hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + "를 획득했습니다.");
                //Inventory.instacne.AcquireItem(hitInfo.transform.GetComponent<ItemPickUp>().item);
                Destroy(hitInfo.transform.gameObject);
                InfoDisappear();
            }
        }
    }

    private void CheckItem()
    {
        Debug.DrawRay(go_Transform.position, go_Transform.TransformDirection(new Vector3(0,0, range)) , Color.blue);
        if (Physics.Raycast(go_Transform.position, go_Transform.TransformDirection(Vector3.forward), out hitInfo, range, layerMask))
        {
            if (hitInfo.transform.CompareTag("Item"))
            {
                ItemInfoAppear();
            }
        }
        else
            InfoDisappear();
    }

    private void ItemInfoAppear()
    {
        pickupActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.text = hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + "<color=yellow>" + " 획득..! " + "</color>";
    }

    private void InfoDisappear()
    {
        pickupActivated = false;
        actionText.gameObject.SetActive(false);
    }
}
