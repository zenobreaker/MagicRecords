using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Drawing;

public class Slot : MonoBehaviour, IPointerClickHandler
{

    private Vector3 originPos;

    protected Item item = null;
    public Item MyItem
    {
        get
        {
            return item;
        }
    }

    public string itemID;
    public string itemName;
    public int itemCount; //.획득한 아이템의 개수
    public Image itemImage; // 아이템의 이미지
    public bool isClick;

    // 필요한 컴포넌트 
    [SerializeField]
    protected Text text_Count = null;
    [SerializeField]
    protected GameObject go_CountImage = null;
    [SerializeField] protected Image itemBGImage = null;
    [SerializeField] protected RectTransform rt_parent = null;
    [SerializeField] protected Image fadeImage = null;

    protected SlotTooltip theSlotTooltip;

    protected GameObject selectedSlot;


    void Start()
    {
        //theSlotTooltip = FindObjectOfType<SlotTooltip>();
        originPos = transform.position;
    }

    // 이미지의 투명도 조절
    protected void SetColor(float _alpha)
    {
        UnityEngine.Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
        itemBGImage.color = color;
    }

    public void SetSize(Vector2 p_size)
    {
        // 슬롯 크기에 따른 width와 height 값을 가져와 대입
        itemImage.rectTransform.sizeDelta = p_size;
        itemBGImage.rectTransform.sizeDelta = p_size;
    }

    public void SetSize()
    {
        Debug.Log("사이즈!" + rt_parent.sizeDelta);
        // 슬롯 크기에 따른 width와 height 값을 가져와 대입
        itemImage.rectTransform.sizeDelta = rt_parent.sizeDelta;
        itemBGImage.rectTransform.sizeDelta = rt_parent.sizeDelta;
    }


    // 아이템 등급 설정에 따른 배경색 결정 
    protected void SetBackGround(Item _item)
    {
        itemBGImage.sprite = ItemEffectDatabase.instance.GetItemRankSprite(_item);
    }

    // 아이템 획득
    public virtual void AddItem(Item _item, int _count = 1)
    {
        if (_item == null)
            return;

        item = _item;
        itemID = _item.itemImgID;
        itemName = _item.itemName;
        itemCount = _count;
        itemImage.sprite = _item.itemImage;
        //Debug.Log("아이템 들어옴" + _item.itemName + this.name + _item.itemRank);
        SetBackGround(_item);
        

        if (_item.itemType != ItemType.Equipment)
        {
            go_CountImage.SetActive(true);
            text_Count.text = itemCount.ToString();
        }
        else
        {
            text_Count.text = "0";
            go_CountImage.SetActive(false);
        }
        
        SetColor(1);
    }

    public Item GetItem()
    {
        return this.item;
    }

    public void SetItemSale()
    {
        if (fadeImage == null)
            return;

        if (this.item.isSale)
        {
            isClick = false;
            fadeImage.gameObject.SetActive(true);
        }
        else
        {
            isClick = true;
            fadeImage.gameObject.SetActive(false);
        }
    }

    // 아이템 개수 조정 
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        if (itemCount <= 0)
            ClearSlot();
    }

    // 슬롯 초기화 
    public virtual void ClearSlot()
    {
        itemID = "";
        itemName = "";
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);

        text_Count.text = "0";
        go_CountImage.SetActive(false);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
       if(eventData.button == PointerEventData.InputButton.Left)
        {
            selectedSlot = this.gameObject;
        }
    }

    

}
