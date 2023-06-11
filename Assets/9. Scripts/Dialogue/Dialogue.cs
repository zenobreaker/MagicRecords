using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [Tooltip("대사치는 캐릭터 이름")]
    public string name;

    [Tooltip("대사 내용")]
    public string[] contexts;
}

[System.Serializable]
public class DialougeEvent
{
    public string name;     // 다이얼로그 이벤트 네임 

    public Vector2 line; 
    public Dialogue[] dialogues;
}