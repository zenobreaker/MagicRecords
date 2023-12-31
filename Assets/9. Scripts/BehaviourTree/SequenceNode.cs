using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 자식 노드를 왼쪽에서오른쪽으로 진행하면서 failure 상태가 나올 때 까지 진행하게 되는 노드
// running 상태일 때 그 상태를 유지해야하기 때문에 다음 자식 노드로 이동하면 안된다.
// 다음 프레임 때도 그 자식에 대한 평가를 진행해야한다.
public class SequenceNode : INode
{
    List<INode> children;

    public SequenceNode(List<INode> children)
    {
        this.children = children;
    }

    public INode.ENodeState Evaluate()
    {
        // 자식들이 없다면 실패
        if (children == null || children.Count == 0)
            return INode.ENodeState.ENS_Failure;

        foreach(var child in children)
        {
            switch(child.Evaluate())
            {
                // running 일때 running을 반환
                case INode.ENodeState.ENS_Running:
                    return INode.ENodeState.ENS_Running;
                // success라면 다음 자식으로 이동한다.
                case INode.ENodeState.ENS_Success:
                    continue;
                // failure 일 때 failure를 반환
                case INode.ENodeState.ENS_Failure:
                    return INode.ENodeState.ENS_Failure;
            }
        }

        // 여기 까지 온다면 success를 반환
        return INode.ENodeState.ENS_Success ;
    }
}
