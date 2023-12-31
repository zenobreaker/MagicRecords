using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INode
{
    public enum ENodeState
    {
        ENS_Running,
        ENS_Success,
        ENS_Failure,
    }

    public ENodeState Evaluate();
}
