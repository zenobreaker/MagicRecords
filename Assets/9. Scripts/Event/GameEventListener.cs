using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    public GameEvent gameEvent;
    public UnityEvent response;


    private void OnEnable()
    {
        if (gameEvent == null)
            return;
        Debug.Log("게임이벤트리스너 등록");
        gameEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        if (gameEvent == null)
            return;

        gameEvent.UnregisterListener(this);
    }


    public void AddEvent(UnityAction action)
    {
        if(action != null)
        {
            Debug.Log("해당 리스너에게 이벤트 등록함");
            response.AddListener(action);
        }
    }

    public void RaiseGameEvent()
    {
        if (gameEvent != null)
            gameEvent.Raise(); // 등록된 게임 이벤트를 전부 실행하지않을까?
    }

    public void OnEventRaised() 
    {
        if(response != null)
        {
            response.Invoke();
        }
    }

}
