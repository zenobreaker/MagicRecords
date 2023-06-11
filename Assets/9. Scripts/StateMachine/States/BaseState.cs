using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    protected CharacterController owner;    // 이 스테이트를 가지는 대상
    public Vector3 destination; // 도착 지점 관련

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void FixedUpdateState();
    public abstract void ExitState();


    // 일정 범위까지 도착했는지 검사하는 함수 
    public bool CheckAlived(Vector3 tr, Vector3 destination, float stopDist)
    {
        var result = Vector3.Magnitude(destination - tr);
        if ( result <= stopDist)
        {
            return true;
        }


        return false;
    }
}
