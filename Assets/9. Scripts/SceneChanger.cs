using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }
        Debug.Log(SceneManager.GetActiveScene().name);
    }

    public int currentScene;

    public GameObject m_MyGameObject;

    IEnumerator LoadYourAsyncScene()
    {
        Scene curScene = SceneManager.GetActiveScene();

        // 현재씬에 특정한 씬 추가
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        // 지정된 씬에 객체 전달
        SceneManager.MoveGameObjectToScene(m_MyGameObject, SceneManager.GetSceneByName("GameScene"));
        Debug.Log("hi "+m_MyGameObject.name);
        // 현재씬 언로드 
        SceneManager.UnloadSceneAsync(curScene);
    }
    public string nextScene;
    
    public int getSceneNumber()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
        
        return currentScene;
    }

    public void GoToScene(string p_NextScene)
    {
        
        if (Time.timeScale != 1)
            Time.timeScale = 1;
        //  SceneManager.MoveGameObjectToScene(m_MyGameObject, SceneManager.GetSceneByName(p_NextScene));

         SceneManager.LoadScene(p_NextScene);
      //  StartCoroutine(LoadYourAsyncScene());
    }
}
