using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance;

    [SerializeField] string csv_FileName;

    Dictionary<int, Dialogue> dialogueDic = new Dictionary<int, Dialogue>();

    public static bool isFinish = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DialogueParser theParser = GetComponent<DialogueParser>();
            if(theParser != null)
            {
                Dialogue[] dialogues = theParser.Parse(csv_FileName);
                Debug.Log("가져옴 어디에서?" + dialogues.Length + dialogues[0].name);
                for (int i = 0; i < dialogues.Length; i++)
                {
                    dialogueDic.Add(i + 1, dialogues[i]);
                }
                isFinish = true;
            }
        }    
    }

    public Dialogue[] GetDialogue(int _startNum, int _endNum)
    {
        List<Dialogue> dialogueList = new List<Dialogue>();
   
        //  1과 3의 값이라면 0 1 2의 개수 3개를 받을 수있음
        for (int i = 0; i <= _endNum - _startNum; i++)
        {
            dialogueList.Add(dialogueDic[_startNum + i]);
        }

        return dialogueList.ToArray();
    }

    // 임시 Character 클래스 반환 
    public Character GetTempCharacter()
    {
        Character tempPlayer = new Character();
        tempPlayer.MyID = 1000;
        tempPlayer.MyStat = new CharStat(1, 10, 10, 20, 100, 100, 10);
        
        return tempPlayer;
    }
}
