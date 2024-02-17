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
        Debug.Log("�����̺�Ʈ������ ���");
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
            Debug.Log("�ش� �����ʿ��� �̺�Ʈ �����");
            response.AddListener(action);
        }
    }

    public void RaiseGameEvent()
    {
        if (gameEvent != null)
            gameEvent.Raise(); // ��ϵ� ���� �̺�Ʈ�� ���� ��������������?
    }

    public void OnEventRaised() 
    {
        if(response != null)
        {
            response.Invoke();
        }
    }

}
