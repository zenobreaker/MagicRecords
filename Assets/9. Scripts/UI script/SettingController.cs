using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingController : MonoBehaviour
{
    public Slider bgmSlider; // BGM을 조절하는 Slider
    public Slider sfxSlider; // Effect Sound  조절하는 Slider

    public Image bgmImage;
    public Image sfxImage;

    public Sprite[] audioSprite; // 0 : Sound On Sprite , 1 : Sound Off Sprite


    void Start()
    {
        if (SoundManager.instance != null)
        {
            bgmSlider.value = SoundManager.instance.bgmPlayer.volume;
            sfxSlider.value = SoundManager.instance.sfxPlayer[0].volume;
        }
    }

    private void OnGUI()
    {
        CheckValue();
    }


    // 슬라이더의 값을 확인 
    void CheckValue()
    {
        if (bgmSlider.value == 0)   // 브금 슬라이더의 값이 0으로 될 경우 이미지 변경
            bgmImage.sprite = audioSprite[1];
        else if(bgmSlider.value > 0)
            bgmImage.sprite = audioSprite[0];

        if (sfxSlider.value == 0)
            sfxImage.sprite = audioSprite[1];
        else if (sfxSlider.value > 0)
            sfxImage.sprite = audioSprite[0];

    }

    public void SetBGMVolume(float _volume)
    {
        SoundManager.instance.bgmPlayer.volume = _volume;
    }

    public void SetSfxVolume(float _volume)
    {
        for (int i = 0; i < SoundManager.instance.sfxPlayer.Length; i++)
        {
            SoundManager.instance.sfxPlayer[i].volume = _volume;
        }
    }

    // 이미지 버튼을 누르면 동작 
    public void SetVolumeButton(string _type)
    {
        switch (_type)
        {
            case "BGM":
                if (bgmSlider.value > 0)    // bgm이 켜저 있는 경우
                {
                    bgmImage.sprite = audioSprite[1];
                    SoundManager.instance.bgmPlayer.volume = 0;
                }
                else if (bgmSlider.value == 0) // bgm이 꺼져 있는 경우
                {
                    bgmImage.sprite = audioSprite[0];
                    SoundManager.instance.bgmPlayer.volume = 0.3f;
                }
                bgmSlider.value = SoundManager.instance.bgmPlayer.volume;   // 슬라이더 값 최신화
                break;
            case "SFX":
                // sfx 오디오 소스들을 전부 불러온다 
                if (sfxSlider.value > 0)
                {
                    sfxImage.sprite = audioSprite[1];
                    for (int i = 0; i < SoundManager.instance.sfxPlayer.Length; i++)
                    {
                        SoundManager.instance.sfxPlayer[i].volume = 0;
                    }
                }
                else if (sfxSlider.value == 0)
                {
                    sfxImage.sprite = audioSprite[0];
                    for (int i = 0; i < SoundManager.instance.sfxPlayer.Length; i++)
                    {
                        SoundManager.instance.sfxPlayer[i].volume = 0.3f;
                    }
                }
                sfxSlider.value = SoundManager.instance.sfxPlayer[0].volume;
                break;
        }
    }
  
}
