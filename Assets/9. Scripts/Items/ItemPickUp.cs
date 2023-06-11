using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public Item item;

    [SerializeField] float m_force = 0f; // 튕겨나갈 힘
    [SerializeField] Vector3 m_offset = Vector3.zero;

    public void ItemPick()
    {
        SoundManager.instance.PlaySE(item.itemSound);

        if (item.itemType == ItemType.Coin)
        {
            Debug.Log(item.itemValue);
            LobbyManager.coin += item.itemValue;    // 코인 획득 시, 획득 코인 증가 
        }
        else
        {
            //InventoryManager.instance.AddItem(item);
        }
        
        GameManager.MyInstance.itemCount--;
        Destroy(this.gameObject);
    }

    public void OnEnable()
    {
        Rigidbody rigidbody = this.gameObject.GetComponentInChildren<Rigidbody>();
        rigidbody.AddExplosionForce(m_force, transform.position + m_offset, 10f);
    }
}
