using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class PortraitClass
{
    public int id;
    public Sprite sprite; 
}


[CreateAssetMenu(fileName = "Portriat Data", menuName = "Scriptable Object/Portriat Data", order = int.MaxValue)]
public class PortraitData : ScriptableObject
{
    [SerializeField]
    public List<PortraitClass> sprites = new List<PortraitClass>();

    public Sprite GetSprite(int id)
    {
        PortraitClass portraitClass = sprites.Find(x => x.id == id);

        if (portraitClass != null)
        {
            return portraitClass.sprite;
        }
        else
        {
            return null;
        }
    }
}

