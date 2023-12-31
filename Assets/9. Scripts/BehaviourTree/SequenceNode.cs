using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �ڽ� ��带 ���ʿ������������� �����ϸ鼭 failure ���°� ���� �� ���� �����ϰ� �Ǵ� ���
// running ������ �� �� ���¸� �����ؾ��ϱ� ������ ���� �ڽ� ���� �̵��ϸ� �ȵȴ�.
// ���� ������ ���� �� �ڽĿ� ���� �򰡸� �����ؾ��Ѵ�.
public class SequenceNode : INode
{
    List<INode> children;

    public SequenceNode(List<INode> children)
    {
        this.children = children;
    }

    public INode.ENodeState Evaluate()
    {
        // �ڽĵ��� ���ٸ� ����
        if (children == null || children.Count == 0)
            return INode.ENodeState.ENS_Failure;

        foreach(var child in children)
        {
            switch(child.Evaluate())
            {
                // running �϶� running�� ��ȯ
                case INode.ENodeState.ENS_Running:
                    return INode.ENodeState.ENS_Running;
                // success��� ���� �ڽ����� �̵��Ѵ�.
                case INode.ENodeState.ENS_Success:
                    continue;
                // failure �� �� failure�� ��ȯ
                case INode.ENodeState.ENS_Failure:
                    return INode.ENodeState.ENS_Failure;
            }
        }

        // ���� ���� �´ٸ� success�� ��ȯ
        return INode.ENodeState.ENS_Success ;
    }
}
