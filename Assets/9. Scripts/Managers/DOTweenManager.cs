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

    // ���⼭ DOTween�� �ʱ�ȭ�ϰų� �ٸ� DOTween ���� ����� �߰��� �� �ֽ��ϴ�.
    void Start()
    {
        // DOTween �ʱ�ȭ
        DOTween.Clear();
        DOTween.Init();
        DOTween.RewindAll();
    }
}
