using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public Slider slider;
  
    AsyncOperation asyncOperation;

    // Start is called before the first frame update
    void Start()
    {
        // SkillBookManager.instance.GetSkillBook();
        slider.value = 0;
        StartCoroutine(StartLoad(StageChannel.stageCategory)); // 로딩신이 등장하자마자 다음 씬으로 이동 준비 
    }

    public IEnumerator StartLoad(string startSceneName)
    {
        asyncOperation = SceneManager.LoadSceneAsync(startSceneName);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            Debug.Log("시간" + Time.timeScale);
            if (slider.value < 0.9f)
            {
                slider.value = Mathf.MoveTowards(slider.value, 0.9f,Time.deltaTime);
            }
            else if (asyncOperation.progress >= 0.9f)
            {
                slider.value = Mathf.MoveTowards(slider.value, 1f, Time.deltaTime);
            }

            if (slider.value >= 1f && asyncOperation.progress >= 0.9f)
                asyncOperation.allowSceneActivation = true;

            yield return null;
        }
    }

}
