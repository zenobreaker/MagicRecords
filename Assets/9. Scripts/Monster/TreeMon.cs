using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeMon : AttackMonster
{
    public GameObject go_TreeAttack;


    protected override void RandomPattern()
    {
        base.RandomPattern();

        switch (currentPattern)
        {
            case 0:
                baseAttackRange = 3.5f * addRange;
                delayTime = 3f;
                break;
            case 1:
                baseAttackRange = 5f * addRange;
                delayTime = 5f;
                break;
            case 2:
            case 3:
                baseAttackRange = 7f * addRange;
                delayTime = 7f;
                break;
            case 4:
                // 보스타입이 아니라면 해당 패턴은 넘긴다.
                if (player.MyStat.myGrade != MonsterGrade.BOSS)
                {
                    RandomPattern();
                    break;
                }
                baseAttackRange = 7f * addRange;
                delayTime = 7f;

                break;
            default:
                baseAttackRange = 3.5f * addRange;
                delayTime = 7.5f;
                break;
        }
    }

    protected override void SetAction(int p_currentPattern)
    {
        base.SetAction(p_currentPattern);
        switch (p_currentPattern)
        {
            case 0:
                anim.SetTrigger("Attack");
                break;
            case 1:
                anim.SetTrigger("Attack2");
                StartCoroutine(SummonTree());
                break;
            case 2:
                anim.SetTrigger("Attack2");
                StartCoroutine(TreeWave());
                break;
            case 3:
                anim.SetTrigger("Attack2");
                StartCoroutine(TreeTriWave());
                break;
            case 4:
                anim.SetTrigger("Attack2");
                StartCoroutine(TreeAroundWave());
                break;
        }
    }

    public override void AttackEableObject(bool isOn)
    {
        if (attackRanges.Length > 0)
        {
            // AttackMonster에서 호출하니 여긴 주석
            //attackRange.GetComponent<AttackArea>().power = status.MyAttack;
            if (attackRanges[currentPattern] != null)
            {
                attackRanges[currentPattern].SetAttackInfo(player, transform);
                attackRanges[currentPattern].SetOnEnableCollider();
            }
        }
        StartCoroutine(NormalAttack());
    }

    IEnumerator NormalAttack()
    {
        yield return new WaitForSeconds(1.2f);

        Vector3 t_Pos = transform.localPosition + (transform.forward * 1.5f + transform.up * 1.5f);
        var clone = Instantiate(attackEffect[0], t_Pos, transform.localRotation);
        clone.Play();
        yield return new WaitForSeconds(0.8f);
        //attackRange.GetComponent<BoxCollider>().enabled = false;
        AttackEableObject(false);
        Destroy(clone.gameObject);
        yield return null;
    }

    void CreateTree()
    {

    }
    IEnumerator SummonTree()
    {
        Vector3 t_Pos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        //DangerMarkerShoot(1, 1f, 1f);
        yield return new WaitForSeconds(1f);

        var tree = Instantiate(go_TreeAttack, t_Pos + transform.forward*3f, Quaternion.identity);
        //tree.GetComponentInChildren<AttackArea>().power = Mathf.RoundToInt(player.MyTotalAttack* 1.5f);
        tree.GetComponentInChildren<AttackArea>().SetAttackInfo(player, transform, 1.5f);
        //tree.GetComponent<AttackArea>().PlayAnim();
        Destroy(tree, 1f);
        
    }

    IEnumerator TreeWave()
    {
        Vector3 trForward = transform.forward * 3f;
        Vector3 t_Pos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        DangerMarkerShoot(0, t_Pos, t_Pos + trForward);
        yield return new WaitForSeconds(1.5f);
       
        Vector3 treePos = t_Pos;
        int count = 0;

        while (count < 5)
        {
            var tree = Instantiate(go_TreeAttack, treePos + trForward, Quaternion.identity);
            //tree.GetComponentInChildren<AttackArea>().power = Mathf.RoundToInt(player.MyTotalAttack * 1.7f);
            tree.GetComponentInChildren<AttackArea>().SetAttackInfo(player, transform, 1.7f);
            //tree.GetComponent<AttackArea>().PlayAnim();
            treePos = tree.transform.localPosition;
            count++;
            Destroy(tree, 4.5f);
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(0.5f);

        //yield return null;
    }

    IEnumerator TreeTriWave()
    {
        //Mathf.Cos(Mathf.PI * 2 * curPatternCount / maxPatternCount[patternIndex])
        //              원주율(둘레값이 클 수록 파형을 많이그림)   *  0~1까지의 수(패턴개수)
        //#. 3방향으로 보낼 경고선 계산 
        Vector3 trForward = transform.forward * 3f;
        Quaternion qAngle1 = Quaternion.Euler(0, 30, 0); // 중앙 기술로 부터 멀어질 각도 (우)
        Quaternion qAngle2 = Quaternion.Euler(0, -30, 0); // 중앙 기술로 부터 멀어질 각도 (좌)

        Vector3 myPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);   // 자기 위치
        Vector3 treePos1, treePos2, treePos3;

        treePos1 = myPos + trForward;
        treePos2 = qAngle1 * trForward;  // 쿼터니언 값에 벡터값을 곱하면 각도만큼 회전한 값 
        treePos3 = qAngle2 * trForward;

        DangerMarkerShoot(0, myPos, treePos1);
        DangerMarkerShoot(0, myPos, myPos + treePos2);
        DangerMarkerShoot(0, myPos, myPos + treePos3);
        
        treePos2 += myPos;  // 원래 위치를 더해줘서 원하는 위치로 이동 
        treePos3 += myPos;

        yield return new WaitForSeconds(1.5f);

        int count = 0;
      
        while (count < 5)
        {
            
           // Vector3 t_pos2 = new Vector3(treePos2.x + Mathf.Cos(Mathf.PI * 15), 0, treePos2.z + Mathf.Cos(Mathf.PI * 15));
            var tree1 = Instantiate(go_TreeAttack, treePos1, Quaternion.identity);
            //tree1.GetComponentInChildren<AttackArea>().power = player.MyTotalAttack + Mathf.RoundToInt(player.MyTotalAttack * 0.7f);
            tree1.GetComponentInChildren<AttackArea>().SetAttackInfo(player, transform, 1.7f);
            var tree2 = Instantiate(go_TreeAttack, treePos2, Quaternion.identity);
            //tree2.GetComponentInChildren<AttackArea>().power = player.MyTotalAttack + Mathf.RoundToInt(player.MyTotalAttack * 0.7f);
            tree2.GetComponentInChildren<AttackArea>().SetAttackInfo(player, transform, 1.7f);
            var tree3 = Instantiate(go_TreeAttack, treePos3, Quaternion.identity);
            //tree3.GetComponentInChildren<AttackArea>().power = player.MyTotalAttack + Mathf.RoundToInt(player.MyTotalAttack * 0.7f);
            tree3.GetComponentInChildren<AttackArea>().SetAttackInfo(player, transform, 1.7f);

            // 이전에 소환한 나무 위치 + 차이 만큼 다음 위치 값 세팅 
            treePos1 = tree1.transform.localPosition + trForward;
            // 중앙 기준으로 다시 목표 방향 세팅 
            Vector3 tVec = treePos1 - myPos;
            treePos2 = qAngle1 * tVec;
            treePos3 = qAngle2 * tVec;
            treePos2 += myPos;
            treePos3 += myPos;

            count++;
            Destroy(tree1, 4.5f);
            Destroy(tree2, 4.5f);
            Destroy(tree3, 4.5f);
            yield return new WaitForSeconds(0.5f);
        }

       // yield return new WaitForSeconds(2f);

    }

    IEnumerator TreeAroundWave()
    {
        int roopCount = 5;
        float distance = 3.5f;              // 나무 생성간 거리 
        int count = 15;                     // 발사체 개수
        float intervalAngle = 360 / count;  // 발사체 사이의 각도
        float x,z;
        x = z = 0;

        // #. 전 방향으로 경고선 방출 
        Vector3 t_Pos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);   // 자기 위치

        for (int i = 0; i < count; i++)
        {

            float angle = intervalAngle * i;
            x = Mathf.Cos(angle * Mathf.PI / 180);
            z = Mathf.Sin(angle * Mathf.PI / 180);

            t_Pos.x = distance * x + t_Pos.x;
            t_Pos.z = distance * z + t_Pos.z;

            Vector3 endPos = new Vector3(x, 0, z * 15);
            DangerMarkerShoot(0, t_Pos, t_Pos + endPos);
            //dangerLine[0].GetComponent<DangerLine>().TempDangerMarkerShoot(transform.localPosition, new Vector3(x,0,z), Quaternion.identity, 18f);

        }
               
        yield return new WaitForSeconds(1.5f);

        // #. 전 방향으로 나무 생성
        while (roopCount > 0)
        {
            for (int i = 0; i < count; i++)
            {

                float angle = intervalAngle * i;
                x = Mathf.Cos(angle * Mathf.PI / 180);
                z = Mathf.Sin(angle * Mathf.PI / 180);

                t_Pos.x = distance * x + transform.localPosition.x;
                t_Pos.z = distance * z + transform.localPosition.z;

                var tree = Instantiate(go_TreeAttack, t_Pos, Quaternion.identity);
                //tree.GetComponentInChildren<AttackArea>().power = player.MyTotalAttack + Mathf.RoundToInt(player.MyTotalAttack * 0.7f);
                tree.GetComponentInChildren<AttackArea>().SetAttackInfo(player, transform, 1.7f);
                Destroy(tree, 3.5f);
            }
            distance += 3;
            roopCount -= 1;
            yield return new WaitForSeconds(0.7f);
        }

        yield return null;
    }


}
