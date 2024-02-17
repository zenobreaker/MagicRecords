using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 아군 유닛들의 배치를 신경 써주는 컨트롤러 
public class PositionController : MonoBehaviour
{

    List<PlayerControl> playerList = new List<PlayerControl>();

    // 파티별 포지션 
    public enum PartyFormation 
    { 
        BASE_TRIANGLE, // 기본 정삼각형 
        REVRESE_TRIANGLE,// 역삼각형
        CENTER_ONE_COLUMN, // 리더를 중앙으로한 1열 
    }

    public PartyFormation myFormation = PartyFormation.BASE_TRIANGLE;

    // 오브젝트의 위치를 중심으로 하는 정삼각형 꼭지점들의 위치 반환
    public Vector3[] GetTrianglePoints(PlayerControl centerObject, float triangleSize)
    {
        Vector3 center = centerObject.transform.position;

        // 정삼각형의 한 변의 길이를 계산
        float sideLength = triangleSize / Mathf.Sqrt(3f);

        // 정삼각형 꼭지점들의 위치 계산
        Vector3[] points = new Vector3[3];
        points[0] = center + Quaternion.Euler(0, 0, 0) * (Vector3.forward * sideLength);
        points[1] = center + Quaternion.Euler(0, 120, 0) * (Vector3.forward * sideLength);
        points[2] = center + Quaternion.Euler(0, -120, 0) * (Vector3.forward * sideLength);

        return points;
    }

    // 오브젝트의 위치를 중심으로 하는 역삼각형 꼭지점들의 위치 반환
    public Vector3[] GetInvertedTrianglePoints(PlayerControl centerObject, float triangleSize)
    {
        Vector3 center = centerObject.transform.position;

        // 정삼각형의 한 변의 길이를 계산
        float sideLength = triangleSize / Mathf.Sqrt(3f);

        // 역삼각형 꼭지점들의 위치 계산
        Vector3[] points = new Vector3[3];
        points[0] = center + Quaternion.Euler(0, 180, 0) * Vector3.forward * sideLength;
        points[1] = center + Quaternion.Euler(0, 300, 0) * Vector3.forward * sideLength;
        points[2] = center + Quaternion.Euler(0, 60, 60) * Vector3.forward * sideLength;

        return points;
    }

    // 1열로 오브젝트를 배치하는 함수
    public Vector3[] PlaceObjectsInRow(PlayerControl centerObject, int objectCount, float spacing)
    {
        Vector3[] positions = new Vector3[objectCount]; // 오브젝트 위치를 저장할 배열

        Vector3 center = centerObject.transform.position; // 중심 오브젝트의 위치

        for (int i = 0; i < objectCount; i++)
        {
            positions[i] = center + Vector3.forward * (i * spacing); // 각 오브젝트의 새로운 위치 계산
        }

        return positions; // 오브젝트의 위치 배열 반환
    }

    // 포지션과 넘버링에 따른 위치 값 반환
    Vector3[] GetPositionByIndexAndFormation(PlayerControl main, PartyFormation partyFormation)
    {
        // 특정 위치에서 포메이션에 따라 위치를 정한다. 
        Vector3[] positions = new Vector3[3];
        // 정삼각형일 때 기준을 생각한다. 
        // 두 번째 녀석은 중앙에서 왼쪽 아래로 
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

    // 배치되기전에 배치되는 공간인지 검사 
    bool CheckPossibleLocate(PlayerControl main, PlayerControl target, Vector3 pos)
    {
        if(main == null || target == null)
        {
            return false; 
        }

        // 타겟이 몇 번째인지 알아내기 
        if (main == target) return false;

        int targetIndex = playerList.FindIndex(p => p == main);
        if (targetIndex == -1) return false;

        // 타겟이 두 번째 일 경우 
        Vector3 seondTargetVec;
        if(targetIndex == 1)
        {
            // 메인 위치에서 
            //seondTargetVec = 
        }

        // 메인 캐릭터를 기준으로 주변을 검사 
        Vector3 mainPos = main.transform.position;

        Collider[] colliders = Physics.OverlapSphere(mainPos, 1.0f);

        int exceptLayer = 1 << LayerMask.NameToLayer("Player") | 
             1 << LayerMask.NameToLayer("Land") | 
             1 << LayerMask.NameToLayer("Enemy") ;
        

        for(int i = 0; i < colliders.Length; i++)
        {
            // 특정 오브젝트가 오면 거른다.
        }
        

        return false; 
    }

    public void SetPlayerList(List<PlayerControl> playerControls)
    {
        playerList = playerControls;
    }


    // 포지션을 지정한다. 
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
