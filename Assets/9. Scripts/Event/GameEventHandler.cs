using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

public class GameEventHandler : MonoBehaviour
{
    public static GameEventHandler instance;

    public GameEventListener gameEventListener;

    public enum GameEventState
    { 
        GAME_INIT,
        GAME_READY,
        GAME_RUNNING,
        GAME_FINISHED,

        GAME_LOBBY_INIT,
        GAME_LOBBY_OUT,
    }



    void Awake()
    {
        if (instance == null)
            instance = this; 
        else
        {
            Destroy(this);
        }

        DontDestroyOnLoad(this.gameObject);
    }


    private void Start()
    {
        gameEventListener = GetComponent<GameEventListener>();
    }


    private void LateUpdate()
    {
        // 이벤트 실행
        InvokGameEvent();
    }

    // 이벤트 실행
    public void InvokGameEvent()
    {
        if (gameEventListener == null)
            return;

        gameEventListener.RaiseGameEvent();
    }

    // 이벤트 추가하기 
    public void AddEventToLobby(UnityAction action)
    {
        gameEventListener.AddEvent(action);
    }

    public void TestAddEvent()
    {
        gameEventListener.AddEvent(() =>
        {
            Debug.Log("테스트 미션 발생");
        });
    }


}
