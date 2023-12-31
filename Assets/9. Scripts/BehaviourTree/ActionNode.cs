using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 수행을 하는 최하위 노드
public class ActionNode : INode
{
    Func<INode.ENodeState> onUpdate = null;

    public ActionNode(Func<INode.ENodeState> onUpdate)
    {
        this.onUpdate = onUpdate;
    }

    public INode.ENodeState Evaluate()
    {
        // onUpdate에 델리게이트가 있다면 실행한 후 그 결과를 반환 없다면 실패를 반환
        return onUpdate?.Invoke() ?? INode.ENodeState.ENS_Failure;
    }
}
