using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// 2022 02 27 
public class LoadingManager : MonoBehaviour
{
    #region singleton 
    // 싱글톤 변수 
    public static LoadingManager instance;


    private void Awake()
    {
        // 각 씬마다 배치할 예정이므로 제거되지않은 오브젝트로 설정하지 않는다
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion

    public static string LOAD_SCENE_NAME;      // 로딩할 씬 이름 저장 변수 

    //// 로딩씬을 로드할 함수
    //public void LoadTheLoadingScene(string name)
    //{
    //    LOAD_SCENE_NAME = name; 
    //    SceneManager.LoadScene("LoadingScene");
    //}
}
