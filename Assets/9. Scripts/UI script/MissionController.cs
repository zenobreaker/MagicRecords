using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionController : MonoBehaviour
{
    // �κ�� ���ư���
    public void BackToLobby()
    {
        //�� ����
        LoadingSceneController.LoadScene("Lobby");
    }

    // ���� ��� �ϱ� 
    public void ContinueTheGame()
    {
        // �����ϰ� �׳� UI ���� 
        this.gameObject.SetActive(false);
    }
}
