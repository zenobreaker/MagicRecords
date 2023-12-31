using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 자식 노드 중에서 처음으로 success나 running 상태를 가진 노드가 발생하면 그 노드까지 진행하고 멈춤
public class SelectorNode : INode
{
    List<INode> children; // 자식 노드들을 담는 변수

    public SelectorNode(List<INode> children)
    {
        this.children = children;
    }

    public INode.ENodeState Evaluate()
    {
        // 자식들이 없다면 실패 
        if (children == null)
            return INode.ENodeState.ENS_Failure;

        foreach (var child in children)
        {
            // 자식들의 평가실행 후 그 결과값에 따른 반환 처리
            switch (child.Evaluate())
            {
                // 자식 상태가 running 일때 running 반환
                case INode.ENodeState.ENS_Running:
                    return INode.ENodeState.ENS_Running;
                //자식 상태가 success 일 때 success 반환
                case INode.ENodeState.ENS_Success:
                    return INode.ENodeState.ENS_Success;
                // 자식 상태가 failure 일때 다음 상태로 이동
            }
        }

        return INode.ENodeState.ENS_Failure;
    }
}
