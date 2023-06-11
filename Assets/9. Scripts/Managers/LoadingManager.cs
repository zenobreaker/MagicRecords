using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// 2022 02 27 
public class LoadingManager : MonoBehaviour
{
    #region singleton 
    // �̱��� ���� 
    public static LoadingManager instance;


    private void Awake()
    {
        // �� ������ ��ġ�� �����̹Ƿ� ���ŵ������� ������Ʈ�� �������� �ʴ´�
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

    public static string LOAD_SCENE_NAME;      // �ε��� �� �̸� ���� ���� 

    //// �ε����� �ε��� �Լ�
    //public void LoadTheLoadingScene(string name)
    //{
    //    LOAD_SCENE_NAME = name; 
    //    SceneManager.LoadScene("LoadingScene");
    //}
}
