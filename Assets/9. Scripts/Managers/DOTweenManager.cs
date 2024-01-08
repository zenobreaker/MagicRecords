using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DOTweenManager : MonoBehaviour
{
    public static DOTweenManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 여기서 DOTween을 초기화하거나 다른 DOTween 관련 기능을 추가할 수 있습니다.
    void Start()
    {
        // DOTween 초기화
        DOTween.Clear();
        DOTween.Init();
        DOTween.RewindAll();
    }
}
