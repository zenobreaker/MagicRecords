using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageObjectPooler : MonoBehaviour
{
    public static DamageObjectPooler instance;

    List<FloatingDynamicText> damageTextList = new List<FloatingDynamicText>();
    public int poolLength;
    public FloatingDynamicText myDamageText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        GeneratePool();
    }

    void GeneratePool()
    {
        for (int i = 0; i < poolLength; i++)
        {
            var temp = Instantiate(myDamageText, this.gameObject.transform);
            temp.gameObject.SetActive(false);
            damageTextList.Add(temp);
        }
    }

    public void DisablePool()
    {
        foreach (var text in damageTextList)
        {
            text.gameObject.SetActive(false);
        }
    }

    public FloatingDynamicText GetObject(Vector3 _pos)
    {
        FloatingDynamicText temp = null;

        foreach (var text in damageTextList)
        {
            // 하이라키에 텍스트 활성화 오브젝트가 있다면 해당 오브젝트를 선택 
            if (!text.gameObject.activeInHierarchy)
            {
                temp = text;
                temp.transform.position = Camera.main.WorldToScreenPoint(_pos);
                break;
            }
        }

        if (temp != null)
            return temp;
        else
            return AddNewObject();
    }

    FloatingDynamicText AddNewObject()
    {
        var temp = Instantiate(myDamageText, gameObject.transform);
        temp.gameObject.SetActive(true);
        damageTextList.Add(temp);
        return temp;
    }
}
