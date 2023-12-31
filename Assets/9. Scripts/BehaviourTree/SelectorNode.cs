using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �ڽ� ��� �߿��� ó������ success�� running ���¸� ���� ��尡 �߻��ϸ� �� ������ �����ϰ� ����
public class SelectorNode : INode
{
    List<INode> children; // �ڽ� ������ ��� ����

    public SelectorNode(List<INode> children)
    {
        this.children = children;
    }

    public INode.ENodeState Evaluate()
    {
        // �ڽĵ��� ���ٸ� ���� 
        if (children == null)
            return INode.ENodeState.ENS_Failure;

        foreach (var child in children)
        {
            // �ڽĵ��� �򰡽��� �� �� ������� ���� ��ȯ ó��
            switch (child.Evaluate())
            {
                // �ڽ� ���°� running �϶� running ��ȯ
                case INode.ENodeState.ENS_Running:
                    return INode.ENodeState.ENS_Running;
                //�ڽ� ���°� success �� �� success ��ȯ
                case INode.ENodeState.ENS_Success:
                    return INode.ENodeState.ENS_Success;
                // �ڽ� ���°� failure �϶� ���� ���·� �̵�
            }
        }

        return INode.ENodeState.ENS_Failure;
    }
}
