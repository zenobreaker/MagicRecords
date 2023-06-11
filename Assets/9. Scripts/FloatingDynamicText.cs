using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingDynamicText : MonoBehaviour
{
    public Text myDamageText;
    public float posY;
    public float lifeTime;

    public float life;

    private Vector3 wtsPos; // ������ǥ���� ��ũ����ǥ�� �� �� 
    private Vector3 poollerPos;  // ������Ʈ Ǯ���� ��ǥ�� 

    public void SetText(string text, Vector3 wts = default(Vector3))
    {
        myDamageText.text = text;
        this.gameObject.SetActive(true);
        wtsPos = wts;
        life = lifeTime; 
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.activeInHierarchy == true)
        {

            // �θ� ������Ʈ �Ʒ��� �����ϴ� ������Ʈ�̱⶧���� �����ǰ��� ���÷� ����Ѵ�.
            Vector3 upper = new Vector3(0,  posY * Time.deltaTime, 0);
            wtsPos +=  upper; 
            this.transform.position = Camera.main.WorldToScreenPoint(wtsPos);
            //transform.Translate(0, posY  * Time.deltaTime, 0, Space.World);

            life -= Time.deltaTime;
            if (life <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
