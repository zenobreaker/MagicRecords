using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    [SerializeField] GameObject BackGround = null;
    [SerializeField] Image[] img_Char = null; 
    [SerializeField] GameObject go_DialogueBar = null;
    [SerializeField] GameObject go_DialogueNameBar = null;

    [SerializeField] Text txt_Dialogue = null;
    [SerializeField] Text txt_Name = null;


    Dialogue[] dialogues;

    bool isDialouge = false;
    bool isNext = false; //특정 키 입력 대기.

    [Header("텍스트 출력 딜레이")]
    [SerializeField] float textDelay = 0.0f;

    int lineCount = 0;  // 대화 카운트 
    int contextCount = 0;   // 대사 카운트 

 
    private void Update()
    {
        if (isDialouge)
        {
            if (isNext)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButton(0))
                {
                    isNext = false;
                    txt_Dialogue.text = "";

                    if (++contextCount < dialogues[lineCount].contexts.Length)
                    {
                        StartCoroutine(TypeWriter());
                    }
                    else
                    {
                        contextCount = 0;
                        if (++lineCount < dialogues.Length)
                        {
                            StartCoroutine(TypeWriter());
                        }
                        else
                        {
                            EndDialogue();
                        }
                    }
                }
            }
        }
    }


    public void ShowDialogue(Dialogue[] p_dialogues)
    {
        isDialouge = true;
        txt_Dialogue.text = "";
        txt_Name.text = "";
     
        dialogues = p_dialogues;

        StartCoroutine(TypeWriter());
    }

    void EndDialogue()
    {
        isDialouge = false;
        contextCount = 0;
        lineCount = 0;
        dialogues = null;
        isNext = false;
        SettingUI(false);
      
    }

    IEnumerator TypeWriter()
    {
        SettingUI(true);

        string t_ReplaceText = dialogues[lineCount].contexts[contextCount];
        t_ReplaceText = t_ReplaceText.Replace("'", ",");

        txt_Name.text = dialogues[lineCount].name;

        for (int i = 0; i < t_ReplaceText.Length; i++)
        {
            txt_Dialogue.text += t_ReplaceText[i];
            yield return new WaitForSeconds(textDelay);
        }

        isNext = true;

    }

    void SettingUI(bool p_flag)
    {
        BackGround.SetActive(p_flag);
        go_DialogueBar.SetActive(p_flag);
        go_DialogueNameBar.SetActive(p_flag);
        for (int i = 0; i < img_Char.Length; i++)
        {
            img_Char[i].gameObject.SetActive(p_flag);
        }
    }

}
