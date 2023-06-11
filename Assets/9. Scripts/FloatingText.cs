using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public TextMeshProUGUI proUGUI;
    public TextMeshPro textMesh; 
    public float destroyTime;
    public float moveTime;

    Camera theCam;
    //public Animation anim;

    private void OnEnable()
    {
        theCam = Camera.main;
        Debug.Log("카메라 각도" + theCam.gameObject.name + " " 
            + theCam.gameObject.transform.localRotation.x);
        this.gameObject.transform.position = new Vector3();
        this.gameObject.transform.rotation  = 
                                Quaternion.Euler(68.0f, 0, 0);
        Invoke("EndText", destroyTime);
        StartCoroutine(SlideUp());
    }

    private void OnDisable()
    {   
        ObjectPooler.ReturnToPool(gameObject);
    }

    public void SetText(string _dmg, Vector3 _createPos)
    {
        var finalPosX = (theCam.transform.position.x + _createPos.x) / 2 * 0.7f;
        var finalPosZ = (theCam.transform.position.z + _createPos.z) / 2 * 0.7f;
        var finalPosY = _createPos.y + 5;

        gameObject.transform.position = new Vector3(finalPosX, finalPosY, finalPosZ);
        gameObject.SetActive(true);
        if (textMesh != null)
        {
            textMesh.text = _dmg;
        }

       // proUGUI.text = _dmg; 

    }

    private void EndText()
    {
        this.gameObject.SetActive(false);
    }

    private IEnumerator SlideUp()
    {
        float currentTime = moveTime;
        while (currentTime > 0)
        {
            this.gameObject.transform.Translate(0, 3 * Time.deltaTime, 0);
            currentTime -= Time.deltaTime;
            yield return null;
        }
    }
}
