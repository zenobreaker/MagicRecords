using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionController : MonoBehaviour
{
    // 로비로 돌아가기
    public void BackToLobby()
    {
        //씬 변경
        LoadingSceneController.LoadScene("Lobby");
    }

    // 게임 계속 하기 
    public void ContinueTheGame()
    {
        // 심플하게 그냥 UI 종료 
        this.gameObject.SetActive(false);
    }
}
