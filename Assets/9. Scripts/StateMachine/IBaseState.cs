using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBaseState
{
    public void EnterState();
    public void UpdateState();
    public void FixedUpdateState();
    public void ExitState();
}
