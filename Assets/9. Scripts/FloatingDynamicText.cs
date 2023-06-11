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

    private Vector3 wtsPos; // 월드좌표에서 스크린좌표로 온 값 
    private Vector3 poollerPos;  // 오브젝트 풀러의 좌표값 

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

            // 부모 오브젝트 아래에 존재하는 오브젝트이기때문에 포지션값을 로컬로 계산한다.
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
