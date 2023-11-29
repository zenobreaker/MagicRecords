using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ToastMessage : MonoBehaviour
{
    // 토스트 메세지 클래스

    [SerializeField]
    TextMeshProUGUI mainText;

    public float fadeDuration;      // 사라지는 시간     


    // 토스트 메세지를 만들어 주는 함수 
    public void CreateToastMessage(string context)
    {
        // 1. 텍스트 내용을 적어준다 
        DrawText(context);

        // 2. 게임 오브젝트를 활성화한다.
        this.gameObject.SetActive(true);
    }

    // 토스트 등장 시 토스트에 포함된 텍스트 UI에 매개변수로 오는 내용을 작성한다.
    public void DrawText(string context)
    {
        if (mainText == null) return; 

        mainText.text = context;
    }

    // 토스트를 등장 시키는 매소드
    public void AppearMessage()
    {
        if (mainText == null)
            return;

        // 사라지는 메소드로 텍스트의 알파가 0으로 내려갔으므로 올려주기 
        mainText.DOFade(1.0f, 0.0f);

        // todo 사운드가 있다면 등장 사운드 추가하기 
    }

    // 토스트가 사라질 때 호출된다. 
    // 토스트 메세지를 사라지게 한다. 
    public void DisappearMessage()
    {
        if (mainText == null)
            return;
        
        mainText.DOFade(0.0f, fadeDuration);
    }

    // 토스트 오브젝트를 비활성화 시켜준다. 
    public void EndToastMessage()
    {
        // 토스트 메세지 오브젝트가 이벤트가 끝나면 끝나는 이벤트로 추가한다.
        this.gameObject.SetActive(false);
    }
    
}
