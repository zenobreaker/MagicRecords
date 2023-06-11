using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionController : MonoBehaviour
{

    [SerializeField] Button btn_Dialogue = null; 

    [SerializeField] InteractionEvent theIE = null;
    [SerializeField] DialogueManager theDM = null;
    
    public void ShowDialogue()
    {
        theDM.ShowDialogue(theIE.GetDialogue());
    }

    public void SettingIcon(bool p_flag)
    {
        btn_Dialogue.gameObject.SetActive(p_flag);
    }
}
