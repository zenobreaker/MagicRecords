using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectSlot : MonoBehaviour
{
    public Image go_StageImage;
    public Button go_StageButton;
    public Text go_StageNameText;

    public void SetSlot(Image _image, string _stageName, UnityEngine.Events.UnityAction callback)
    {
        go_StageImage = _image;
        go_StageNameText.text = _stageName; 
        go_StageButton.onClick.AddListener(callback);
    }

    public void SetSpriteImage(Sprite _sprite)
    {
        go_StageImage.sprite = _sprite;
    }


    public void SetSlotText(string _stageName)
    {
        go_StageNameText.text = _stageName; 
    }

    //btn_MonsterSlots
    public void SetButtonInteractive(bool _interactive)
    {
        go_StageButton.interactable = _interactive;
    }


    public int GetButtonEventCount()
    {
        return go_StageButton.onClick.GetPersistentEventCount();
    }

    /// <summary>
    /// 버튼에 이벤트를 할당 이전 이벤트는 지운다 
    /// </summary>
    /// <param name="_callback"></param>
    public void SetButtonOnlyOneEventLisetener(UnityEngine.Events.UnityAction _callback)
    {
        go_StageButton.onClick.RemoveAllListeners();
        go_StageButton.onClick.AddListener(_callback);
    }
}
