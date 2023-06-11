using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboManager : MonoBehaviour
{
    [SerializeField] Text txt_Combo = null;

    int currentCombo = 0;
    int maxCombo;   // 게임 내에 최대로 한 콤보 수 

    float timer = 5.0f;

    void Start()
    {
        if(txt_Combo == null)
        {
            txt_Combo = GameObject.Find("ComboUi/Text").GetComponent<Text>();
        }

        if (txt_Combo != null)
        {
            txt_Combo.gameObject.SetActive(false);
        }
    }

    public void IncreaseCombo(int p_num = 1)
    {
        currentCombo += p_num;
        txt_Combo.text = string.Format("{0:##0} Combo", currentCombo);

        StopCoroutine(ResettingCombo());
        StartCoroutine(ResettingCombo());

        if (maxCombo < currentCombo)
            maxCombo = currentCombo;

        if(currentCombo > 1)
        {
            txt_Combo.gameObject.SetActive(true);

            //
        }
    }

    public int GetCurrentCombo()
    {
        return currentCombo;
    }

    public void ResetCombo()
    {
        currentCombo = 0;
        txt_Combo.text = "0";
        txt_Combo.gameObject.SetActive(false);
    }

    public int GetMaxCombo()
    {
        return maxCombo;
    }

    IEnumerator ResettingCombo()
    {
        float t_time = timer; 

        while(currentCombo > 0)
        {
            t_time -= Time.deltaTime;

            if(t_time <= 0)
            {
                
                ResetCombo();
                t_time = timer;
                break; 
            }
            yield return null;
        }
    }
}
