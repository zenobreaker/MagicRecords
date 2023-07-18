using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class LoadingSceneController : MonoBehaviour
{
   

    //public static LoadingSceneController Instance
    //{
    //    get
    //    {
    //        if (instance == null) {
    //            var obj = FindObjectOfType<LoadingSceneController>();
    //            if (obj != null)
    //                instance = obj;
    //            else
    //                instance = Create();
    //         }

    //        return instance;
    //    }
    //}

    //private static LoadingSceneController Create()
    //{
    //    return Instantiate(Resources.Load<LoadingSceneController>("LoadingUI"));
    //}


 

    [SerializeField]
    private CanvasGroup canvasGroup = null;

    [SerializeField]
    private Image progressBar = null;

    [SerializeField]
    private Slider slider = null;

    private static string loadSceneName;

    public static void LoadScene(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    { 
        //gameObject.SetActive(true);
        //SceneManager.sceneLoaded += OnSceneLoaded;
        loadSceneName = sceneName;
        SceneManager.LoadScene("LoadingScene", loadSceneMode);
    }

    private void Start()
    {
        StartCoroutine(LoadSceneProcess());
    }

    private IEnumerator LoadSceneProcess()
    {
        slider.value = 0f;
        // 코루틴 안에서 yield return으로 코루틴을 호출하면 해당 코루틴이 끝날 때까지 대기
        //yield return StartCoroutine(Fade(true)); 

        if (string.IsNullOrEmpty(loadSceneName))
            loadSceneName = "GameScene";

        AsyncOperation op = SceneManager.LoadSceneAsync(loadSceneName);
        op.allowSceneActivation = false;

        float timer = 0f;
        while (!op.isDone)
        {
            yield return null;
            if(op.progress < 0.9f)
            {
                slider.value = op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                slider.value = Mathf.Lerp(0.9f, 1f, timer);
                if(slider.value >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if(arg0.name == loadSceneName)
        {
            StartCoroutine(Fade(false));
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private IEnumerator Fade(bool isFadeIn)
    {
        float timer = 0f;
        while(timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 3f;
            canvasGroup.alpha = isFadeIn ? Mathf.Lerp(0f, 1f, timer) : Mathf.Lerp(1f, 0f, timer);
        }

        if (!isFadeIn)
        {
            gameObject.SetActive(false);
        }
    }
}
