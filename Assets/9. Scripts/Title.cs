using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public string sceneName = "GameScene";

    public static Title instance;

   // private SaveAndLoad theSaveNLoad;

    private void Awake()
    {
        //theSaveNLoad = FindObjectOfType<SaveAndLoad>(); 찾을 객체는 현재 씬에 없으므로 

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void ClickStart()
    {
        Debug.Log("로딩");
        SceneManager.LoadScene("Lobby");
        gameObject.SetActive(false);
    }

    public void ClickLoad()
    {
        Debug.Log("로드");

        StartCoroutine(LoadCoroutine());

    }

    IEnumerator LoadCoroutine()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone) // 로딩이 끝나지 않는다면? operation.process를 이용한 로딩화면 제작가능
        {
            yield return null;
        }

    //    theSaveNLoad = FindObjectOfType<SaveAndLoad>(); // 다음 씬으로 넘어와졌을 때 찾도록 한다
    //    theSaveNLoad.LoadData();
        gameObject.SetActive(false); // DontDestroy()로 인해 객체가 파괴되지 않기 때문
    }


    public void ClickExit()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }

}
