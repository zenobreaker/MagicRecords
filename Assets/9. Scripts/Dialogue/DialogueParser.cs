using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueParser : MonoBehaviour
{
    public Dialogue[] Parse(string _CSVFileName)
    {
        List<Dialogue> dialogueList = new List<Dialogue>();     // 대사 리스트 생성
        TextAsset csvData = Resources.Load<TextAsset>(_CSVFileName);    // csv 파일 가져옴

        string[] data = csvData.text.Split(new char[] { '\n' });
      
        
        for (int i = 1; i < data.Length;)
        {
            string[] row = data[i].Split(new char[] { ',' });   // 아이디 캐릭터 이름 대사 순으로 들어간느 배열 

            Dialogue dialogue = new Dialogue();     // 대사 리스트 생성

            dialogue.name = row[1];
            
            List<string> contextList = new List<string>();


            do
            {
                contextList.Add(row[2]);
                if (++i < data.Length)
                {
                    row = data[i].Split(new char[] { ',' });
                }
                else
                {
                    break;
                }
            } while (row[0].ToString() == "");

            dialogue.contexts = contextList.ToArray();      // 배열로 재변환

            dialogueList.Add(dialogue);         // 리스트에 다이얼로그 추가 


        }

        return dialogueList.ToArray();

    }

}
