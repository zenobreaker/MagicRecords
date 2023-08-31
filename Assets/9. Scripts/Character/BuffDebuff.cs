using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BuffType
{ 
    BUFF =1,
    DEBUFF=2,
}


public class BuffDebuff : MonoBehaviour
{
    public BuffType buffType;
    public string buffName;
    public SpecialOption specialOption;
    public Image icon;

    WaitForSeconds seconds = new WaitForSeconds(0.1f);

    private void Awake()
    {
        icon = GetComponent<Image>();  
    }

    public void Init(BuffType buffType, string name, SpecialOption option)
    {
        this.buffType = buffType;
        this.buffName = name;
        specialOption = option;
        if(specialOption != null)
        {
            specialOption.SetCoolTime();
        }
    }

    public void Execute(Character character)
    {
        StartCoroutine(Activation(character));
    }

    IEnumerator Activation(Character character)
    {
        if(specialOption != null && character != null)
        {

            character.MyStat.extraStat.ApplyOptionExtraStat(specialOption.abilityType,
                specialOption.value, specialOption.isPercentage);
            character.MyStat.ApplyOption(); 

            while (specialOption.coolTime > 0)
            {
                specialOption.coolTime -= 0.1f;
                yield return seconds;
            }

            specialOption.coolTime = 0;
            Deactivation(character);
        }
    }

    public void Deactivation(Character character)
    {
        if (character == null) return;


        character.MyStat.extraStat.ApplyOptionExtraStat(specialOption.abilityType,
            -specialOption.value, specialOption.isPercentage);
        character.MyStat.ApplyOption();

        character.RemoveBuffDebuff(this);
    }

}
