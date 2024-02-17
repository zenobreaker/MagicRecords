using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// �Ʊ� ���ֵ��� ��ġ�� �Ű� ���ִ� ��Ʈ�ѷ� 
public class PositionController : MonoBehaviour
{

    List<PlayerControl> playerList = new List<PlayerControl>();

    // ��Ƽ�� ������ 
    public enum PartyFormation 
    { 
        BASE_TRIANGLE, // �⺻ ���ﰢ�� 
        REVRESE_TRIANGLE,// ���ﰢ��
        CENTER_ONE_COLUMN, // ������ �߾������� 1�� 
    }

    public PartyFormation myFormation = PartyFormation.BASE_TRIANGLE;

    // ������Ʈ�� ��ġ�� �߽����� �ϴ� ���ﰢ�� ���������� ��ġ ��ȯ
    public Vector3[] GetTrianglePoints(PlayerControl centerObject, float triangleSize)
    {
        Vector3 center = centerObject.transform.position;

        // ���ﰢ���� �� ���� ���̸� ���
        float sideLength = triangleSize / Mathf.Sqrt(3f);

        // ���ﰢ�� ���������� ��ġ ���
        Vector3[] points = new Vector3[3];
        points[0] = center + Quaternion.Euler(0, 0, 0) * (Vector3.forward * sideLength);
        points[1] = center + Quaternion.Euler(0, 120, 0) * (Vector3.forward * sideLength);
        points[2] = center + Quaternion.Euler(0, -120, 0) * (Vector3.forward * sideLength);

        return points;
    }

    // ������Ʈ�� ��ġ�� �߽����� �ϴ� ���ﰢ�� ���������� ��ġ ��ȯ
    public Vector3[] GetInvertedTrianglePoints(PlayerControl centerObject, float triangleSize)
    {
        Vector3 center = centerObject.transform.position;

        // ���ﰢ���� �� ���� ���̸� ���
        float sideLength = triangleSize / Mathf.Sqrt(3f);

        // ���ﰢ�� ���������� ��ġ ���
        Vector3[] points = new Vector3[3];
        points[0] = center + Quaternion.Euler(0, 180, 0) * Vector3.forward * sideLength;
        points[1] = center + Quaternion.Euler(0, 300, 0) * Vector3.forward * sideLength;
        points[2] = center + Quaternion.Euler(0, 60, 60) * Vector3.forward * sideLength;

        return points;
    }

    // 1���� ������Ʈ�� ��ġ�ϴ� �Լ�
    public Vector3[] PlaceObjectsInRow(PlayerControl centerObject, int objectCount, float spacing)
    {
        Vector3[] positions = new Vector3[objectCount]; // ������Ʈ ��ġ�� ������ �迭

        Vector3 center = centerObject.transform.position; // �߽� ������Ʈ�� ��ġ

        for (int i = 0; i < objectCount; i++)
        {
            positions[i] = center + Vector3.forward * (i * spacing); // �� ������Ʈ�� ���ο� ��ġ ���
        }

        return positions; // ������Ʈ�� ��ġ �迭 ��ȯ
    }

    // �����ǰ� �ѹ����� ���� ��ġ �� ��ȯ
    Vector3[] GetPositionByIndexAndFormation(PlayerControl main, PartyFormation partyFormation)
    {
        // Ư�� ��ġ���� �����̼ǿ� ���� ��ġ�� ���Ѵ�. 
        Vector3[] positions = new Vector3[3];
        // ���ﰢ���� �� ������ �����Ѵ�. 
        // �� ��° �༮�� �߾ӿ��� ���� �Ʒ��� 
        switch (partyFormation)
        {
            case PartyFormation.CENTER_ONE_COLUMN:
                positions = PlaceObjectsInRow(main, 3, 2);
                break;
            case PartyFormation.REVRESE_TRIANGLE:
                positions = GetInvertedTrianglePoints(main, 5);
                break;
            default:
                positions = GetTrianglePoints(main, 5);
                break;

        }
        

        return positions;
    }

    // ��ġ�Ǳ����� ��ġ�Ǵ� �������� �˻� 
    bool CheckPossibleLocate(PlayerControl main, PlayerControl target, Vector3 pos)
    {
        if(main == null || target == null)
        {
            return false; 
        }

        // Ÿ���� �� ��°���� �˾Ƴ��� 
        if (main == target) return false;

        int targetIndex = playerList.FindIndex(p => p == main);
        if (targetIndex == -1) return false;

        // Ÿ���� �� ��° �� ��� 
        Vector3 seondTargetVec;
        if(targetIndex == 1)
        {
            // ���� ��ġ���� 
            //seondTargetVec = 
        }

        // ���� ĳ���͸� �������� �ֺ��� �˻� 
        Vector3 mainPos = main.transform.position;

        Collider[] colliders = Physics.OverlapSphere(mainPos, 1.0f);

        int exceptLayer = 1 << LayerMask.NameToLayer("Player") | 
             1 << LayerMask.NameToLayer("Land") | 
             1 << LayerMask.NameToLayer("Enemy") ;
        

        for(int i = 0; i < colliders.Length; i++)
        {
            // Ư�� ������Ʈ�� ���� �Ÿ���.
        }
        

        return false; 
    }

    public void SetPlayerList(List<PlayerControl> playerControls)
    {
        playerList = playerControls;
    }


    // �������� �����Ѵ�. 
    public void SetPartFormation()
    {
        if (playerList == null || playerList.Count <= 0) return;


        Vector3[] positions = new Vector3[3];

        int count = 1; 
        foreach (var player in playerList)
        {
            if (player == null) continue; 

            if (player.isLeader == true)
            {
                positions = GetPositionByIndexAndFormation(player, myFormation);
                continue;
            }

            if (positions.Length == 0 || count >= positions.Length)
                continue;

            player.SetDestinationPosition(positions[count]);
            count++;
        }

    }
}
