using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusController : MonoBehaviour
{    
    public float recoveryTime;

    // 엑스트라 스탯
    public int extraAtk;
    public int extraDef;
    public int extraSpd;

    public bool isAlive; 

    public void InitRecoveryStat(Character _player)
    {
        if (_player == null)
            return;

        if(_player.MyCurrentHP> 0)
        {
            isAlive = true;
            StartCoroutine(RecorveryMP(_player));
        }

    }

    public void EndRecovery()
    {
        StopAllCoroutines();
    }

    // 마나 자동회복 
    public IEnumerator RecorveryMP(Character _player)
    {
        if (_player == null) yield return null ; 

        while (isAlive == true)
        {
            yield return new WaitForSeconds(recoveryTime);

            if (_player.MyCurrentMP <= _player.MyStat.totalMP)
            {
                _player.MyCurrentMP += 1 + (1 * _player.MyStat.totalMPR);
            }
        }
    }

   
}
