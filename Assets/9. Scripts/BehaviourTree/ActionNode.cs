using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������ �ϴ� ������ ���
public class ActionNode : INode
{
    Func<INode.ENodeState> onUpdate = null;

    public ActionNode(Func<INode.ENodeState> onUpdate)
    {
        this.onUpdate = onUpdate;
    }

    public INode.ENodeState Evaluate()
    {
        // onUpdate�� ��������Ʈ�� �ִٸ� ������ �� �� ����� ��ȯ ���ٸ� ���и� ��ȯ
        return onUpdate?.Invoke() ?? INode.ENodeState.ENS_Failure;
    }
}
