using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner
{
    INode rooteNode; // 최상위 노드값

    public BehaviourTreeRunner(INode rooteNode)
    {
        this.rooteNode = rooteNode;
    }

    public void Operate()
    {
        rooteNode.Evaluate(); // 최상위 노드 평가 시작 
    }
}
