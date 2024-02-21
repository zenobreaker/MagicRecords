using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ItemEffect
{
    public string itemName; //아이템의 이름 (키값)
    public string[] part; // 부위
    public int[] num; // 수치
}

public class ItemEffectDatabase : MonoBehaviour
{
    public static ItemEffectDatabase instance; 

    [SerializeField]
    private ItemEffect[] itemEffects = null;

    public Sprite[] itemBGSprites; 

    //[SerializeField] Equipment[] equipments = null;

    private const string HP = "HP", MP = "MP", Weapon = "Weapon",Armor = "Armor";

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    // 아이템 데이터베이스에 기록된 아이템 효과들을 가져와 전달한다. 
    public string GetItemEffect(Item p_item)
    {
        string t_Str = "";

        for (int x = 0; x < itemEffects.Length; x++)
        {
            if (itemEffects[x].itemName.Contains(p_item.itemImgPath))
            {
                for (int y = 0; y < itemEffects[x].part.Length; y++)
                {
                    switch (itemEffects[x].part[y])
                    {
                        case HP:
                            // 체력 회복 구간
                            t_Str += "HP + " + itemEffects[x].num[y].ToString();
                                break;
                        case MP:
                            t_Str += "MP + " + itemEffects[x].num[y].ToString();
                            break;
                        case Weapon:
                            t_Str += "공격력 + " + itemEffects[x].num[y].ToString();
                            break;
                        case Armor:
                            t_Str += "방어력 + " + itemEffects[x].num[y].ToString();
                            break;
                        default:
                            Debug.Log("적절한 상태를 찾을 수 없습니다.");
                            break;
                    }
                    t_Str += "\n";
                }
                return t_Str;
            }
        }


        return "";
    }

    public string GetAccesoryItemEffect(Item p_item)
    {
        string t_Str = "";

        for (int x = 0; x < itemEffects.Length; x++)
        {
            // todo : 이 조건은 개념이 잘못 되었다 수정이 필요하다 
            if (itemEffects[x].itemName == p_item.itemImgPath)
            {
                for (int y = 0; y < itemEffects[x].part.Length; y++)
                {
                    switch (itemEffects[x].part[y])
                    {
                        case "ATK":
                            t_Str += "공격력 + " + itemEffects[x].num[y].ToString() + "\n";
                            break;
                        case "ATKSPD":
                            t_Str += "공격속도 + " + itemEffects[x].num[y].ToString() + "\n";
                            break;
                        case "DEF":
                            t_Str += "방어력 + " + itemEffects[x].num[y].ToString() + "\n";
                            break;
                        case "SPD":
                            t_Str += "이동속도 + " + itemEffects[x].num[y].ToString() +"\n";
                            break;
                        case HP:
                            t_Str += "체력 + " + itemEffects[x].num[y].ToString() + "\n";
                            break;
                        case "HPRecovery":
                            t_Str += "초당 체력 회복량 + " + itemEffects[x].num[y].ToString() + "\n";
                            break;
                        case MP:
                            t_Str += "마나 + " + itemEffects[x].num[y].ToString() + "\n";
                            break;
                        case "MPRecovery":
                            t_Str += "초당 마나 회복량 + " + itemEffects[x].num[y].ToString() + "\n";
                            break;

                        default:
                            Debug.Log("적절한 상태를 찾을 수 없습니다.");
                            break;
                    }
                }
                return t_Str;
            }
        }
        return "";
    }

    public int[] GetItemEffects(Item p_item)
    {
        string[] t_Effects = new string[itemEffects.Length];

        for (int x = 0; x < itemEffects.Length; x++)
        {
            if (itemEffects[x].itemName == p_item.itemName)
            {
                return itemEffects[x].num;
            }
        }

        return null;
    }

    public int GetItemEffectValue(Item p_item)
    {

        for (int x = 0; x < itemEffects.Length; x++)
        {
            if (itemEffects[x].itemName == p_item.itemName)
            {
                for (int y = 0; y < itemEffects[x].part.Length; y++)
                {
                    switch (itemEffects[x].part[y])
                    {
                        case HP:
                            // 체력 회복 구간
                            return itemEffects[x].num[y];
                        case MP:
                            return itemEffects[x].num[y];
                        case Weapon:
                            return itemEffects[x].num[y];
                        case Armor:
                            return itemEffects[x].num[y];
                        default:
                            Debug.Log("적절한 상태를 찾을 수 없습니다.");
                            break;
                    }
                }
                return 0;
            }
        }
        Debug.Log("일치하는 아이템이 없습니다.");
        return 0;
    }


    // 아이템 등급 확인 후 백그라운드 색상 전달 
    public Sprite GetItemRankSprite(Item _item)
    {
        switch (_item.itemRank)
        {
            case ItemRank.Common:
                return itemBGSprites[0];
            case  ItemRank.Magic:
                return itemBGSprites[1];
            case  ItemRank.Rare:
                return itemBGSprites[2];
            case  ItemRank.Unique:
                return itemBGSprites[3];
            case  ItemRank.Legendary:
                return itemBGSprites[4];
            default:
                return itemBGSprites[5];
        }
    }

    public void UseItem(Item _item)
    {
        Debug.Log("호출됨!");
        if (_item.itemType == ItemType.Used)
        {
            for (int x = 0; x < itemEffects.Length; x++)
            {
                if (itemEffects[x].itemName == _item.itemName)
                {
                    for (int y = 0; y < itemEffects[x].part.Length; y++)
                    {
                        switch (itemEffects[x].part[y])
                        {
                            case HP:
                                // 체력 회복 구간
                              //  CharStat.instance.IncreaseHp(itemEffects[x].num[y]);
                                Debug.Log("최대 체력 증가!" + itemEffects[x].num[y]);
                                break;
                            case MP:
                                break;
                            case Weapon:
                                break;
                            default:
                                Debug.Log("적절한 상태를 찾을 수 없습니다.");
                                break;
                        }
                    }
                    return;
                }
            }
            Debug.Log("일치하는 아이템이 없습니다.");
        }
    }

   
}
