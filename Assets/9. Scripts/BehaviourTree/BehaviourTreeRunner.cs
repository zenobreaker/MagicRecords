using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner
{
    INode rooteNode; // �ֻ��� ��尪

    public BehaviourTreeRunner(INode rooteNode)
    {
        this.rooteNode = rooteNode;
    }

    public void Operate()
    {
        rooteNode.Evaluate(); // �ֻ��� ��� �� ���� 
    }
}
