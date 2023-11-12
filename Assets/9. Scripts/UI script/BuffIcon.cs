using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour
{
    public Image buffImage; 
    public Image coolTimeImage; 

    public SpecialOption specialOption;

    public void Init(BuffDebuff buffDebuff)
    {
        if (buffDebuff == null || buffDebuff.specialOption == null) return;

        if(specialOption != null && specialOption.effectID != buffDebuff.specialOption.effectID)
            coolTimeImage.fillAmount = 0;   

        buffImage.sprite = buffDebuff.sprite;
        specialOption = buffDebuff.specialOption;
        gameObject.SetActive(true);
    }

    void FixedUpdate()
    {
        if (specialOption == null || coolTimeImage == null) return;

        coolTimeImage.fillAmount = 1 - (specialOption.coolTime / specialOption.duration);
        if(specialOption.coolTime <= 0)
        {
            this.gameObject.SetActive(false);
            specialOption = null;
        }
    }
}

