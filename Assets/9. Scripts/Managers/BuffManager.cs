using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public static BuffManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

 
    public void StartBuffTimer(Character character, BuffDebuff buffDebuff)
    {
        if (character == null || buffDebuff == null) return; 

        StartCoroutine(ManageBuffTimer(character, buffDebuff));
    }


    public IEnumerator ManageBuffTimer(Character character, BuffDebuff buffDebuff)
    {
        if (character == null || buffDebuff == null || buffDebuff.specialOption == null)
            yield return null;

        float timer = 0;

        while (buffDebuff.specialOption.coolTime > 0)
        {
            buffDebuff.specialOption.coolTime -= 0.1f;
            timer += 0.1f;
            // ���� ����� �����ϴ� ��� üũ 
            if(buffDebuff.buffCallFlag == true &&
                buffDebuff.buffCallTime <= timer)
            {
                timer = 0;
                // ���� ��� ����
                //buffDebuff.Excute(character);
            }

            yield return new WaitForSeconds(0.1f);
        }

       // character.RemoveBuffDebuff(buffDebuff);

    }
}
