using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ���⸸ �þ�� �ϴ� ĸ�� �ݶ��̴� ��� �ð��� �ӵ��� ���� ���̰� �þ�� �ð� ���̰� �ִ�.
/// </summary>
public class LaserCollider : MonoBehaviour
{
    public int skillDamage;
    public int hitCount;

    public bool isContiued; // ���Ӱ����ΰ�?

    public float recoveryTime;
    private bool isAction = false;

    public float maxLength;
    public float speed; 

    public CapsuleCollider capsuleCollider;
    public ParticleSystem ps_effctTarget;

    Coroutine coroutine;

    RaycastHit[] hits;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (capsuleCollider != null && ps_effctTarget != null)
        {
            var main = ps_effctTarget.main;
            // �ӵ��� ���
            speed = maxLength / ps_effctTarget.sizeOverLifetime.zMultiplier;
            // ĸ�� �ݶ��̴� ��ġ �� ���� �� �ʱ�ȭ 
            capsuleCollider.center = Vector3.zero;
            capsuleCollider.height = 0;
            
            coroutine = StartCoroutine(CoIncreaseColliderIncrease(maxLength, ps_effctTarget.sizeOverLifetime.zMultiplier));
        }
    }

    IEnumerator CoIncreaseColliderIncrease(float _length, float lifeTime = 0)
    {
        Debug.Log("����Ʈ ������Ÿ�� "+lifeTime);
        yield return new WaitForSeconds(lifeTime);
        //float length = 0;
        
        while (capsuleCollider.height < _length)
        {
          
            // �ð��� �ӵ� ��ŭ ���̸� �ø���.
            capsuleCollider.height += Time.deltaTime * speed;

            // �ݶ��̴� ��ġ�� �� ������ ���� ��
            if (capsuleCollider.center.z <= maxLength / 2)
            {
                capsuleCollider.center = new Vector3(0, 0, capsuleCollider.height * 0.5f);
            }

            yield return null;
        }
    }
}
