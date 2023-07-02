using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StageField { NORMAL, GRASS, ICE, DESERT, CAVE };

[System.Serializable]
public class Stage
{
    public uint stageId;
    public string stageName;
  
    public GameObject go_Stage;
    public GameObject[] go_EnemyRespawns;
    public GameObject go_PlayerRespawn;
    public int enemyCount;
    public int wave;
}

public class StageManager : MonoBehaviour
{
    [SerializeField] private RespwanManager theRM = null;
    [SerializeField] private StagePosController theSPC = null;
    [SerializeField] MonsterDatabase monsterDB;

    [Header("스테이지 오브젝트")]
    [SerializeField] private List<Stage> stageList = null;

    Stage selectedStage; 

    int currentStageNum = 0;

    Dictionary<uint, MonsterType> saveMonsterDic = new Dictionary<uint, MonsterType>();
  

    // 필요한 컴포넌트
    [Header("UI 컴포넌트")]
    [SerializeField] private Text txt_CurrentScore = null;
    [SerializeField] private GameObject go_UI = null;
    [SerializeField] private Text txt_StageClearText = null;

    public void SavePlayerTransform(Transform p_PlayerTR)
    {
        theRM.SavePlayerTransform(p_PlayerTR);
    }
    public void SaveEnemy(MonsterType monsterType, uint id)
    {
        saveMonsterDic[id] = monsterType;
    }

    public void SaveEnemy(GameObject _enemy, MonsterType p_monstertType = MonsterType.NORMAL)
    {
        theRM.InitEnemies(_enemy, p_monstertType);
    }

    public void SaveEnemies(GameObject[] _enemies)
    {
        theRM.InitEnemies(_enemies);
    }

    public void SetStage()
    {
        for (int i = 1; i < stageList.Count; i++)
        {
            if (theSPC.GetStageName() == stageList[i].stageName)
            {
                selectedStage = stageList[i];
                break;
            }

        }
    }

    public void CreateStage(uint _stageid)
    {
        //SetStage();
        Debug.Log("스테이지 생성 ID : " + _stageid);

        // 스테이지 정보 가져오기 
        var stageInfo = StageInfoManager.instance.GetStageInfo();
        if(stageInfo == null)
        {
            return; 
        }


        //switch (stageInfo.mapId)
        //{
        //    case 1000:
        //        selectedStage = stageList[1];
        //        break;
        //    case 2000:
        //        selectedStage = stageList[2];
        //        break;
        //    default:    // TEST STAGE 
        //        selectedStage = stageList[0];
        //        stageList[0].go_Stage.SetActive(true);
        //        theRM.RespawnTestMon(stageList[0].go_EnemyRespawns[0].transform);
        //        return;
        //}

        
        selectedStage.go_Stage.SetActive(true);
     
    
        //var monterType = stageInfo.monsterType;
        //theRM.RespawnMonster(selectedStage.go_EnemyRespawns, monterType);
    }

    public void CreateTestStage()
    {
        selectedStage = stageList[0];
        stageList[0].go_Stage.SetActive(true);
        theRM.RespawnTestMon(stageList[0].go_EnemyRespawns[0].transform);
    }
    

    // 스테이지의 캐릭터 생성 위치 값 가져오기 
    public Vector3 GetPlayerSpawnPosition()
    {
        return selectedStage.go_PlayerRespawn.transform.position;
    }
   
    // 챕터 설정 (RL모드 한정)
    public void SetChapterWithRL()
    {
        if (RLModeController.isRLMode)
        {
            theSPC.cur_MainChpaterNum = 1;
        }
    }
    
    public void DisableStage()
    {
        //currentStageNum = -1;
        selectedStage.go_Stage.SetActive(false);
        go_UI.SetActive(false);
        theRM.DeleteAllMonster();
    }

    public int GetEnemyCount()
    {
        return stageList[currentStageNum].enemyCount;
    }

    // 결과창 출력
    public void ShowClearUI(bool _isClear)
    {
        go_UI.SetActive(true);
        if (_isClear)
        {
            txt_StageClearText.text = "Stage Clear!!!";
            // 스테이지 클리어 설정 
            StageInfoManager.instance.GetStageInfo().isCleared = true;
            ClearStage();
        }
        else
            txt_StageClearText.text = "Stage Fail...";
        txt_CurrentScore.text = string.Format("{0:000,000}", GameManager.MyInstance.MyGameScore);
    }

    public void PrevStageBtn()
    {
        // 게임 씬 닫는 로직 작동 (로딩화면)
        Time.timeScale = 1;
        Debug.Log("타임 : " + Time.timeScale);
        //씬 변경
        LoadingSceneController.LoadScene("Lobby");
        //DisableStage();
    }

    public void NextStageBtn()
    {
        if(currentStageNum == -1)
            return;
        
        Time.timeScale = Time.timeScale > 0 ? 0 : 1;
        // 로딩화면 출현 

        //다음 스테이지 버튼을 눌렀을 경우 호출
        if (currentStageNum < stageList.Count - 1)
        {
            //게임이 멈춰있는 상태라면 다시 재생시킨다. 
            Time.timeScale = 1;
            stageList[currentStageNum++].go_Stage.SetActive(false);
            stageList[currentStageNum].go_Stage.SetActive(true);
            StageChannel.stageName = stageList[currentStageNum].stageName;  

            go_UI.SetActive(false);
            Debug.Log("다음 스테이지 활성화");
            GameManager.MyInstance.StageInit();
        }
        else
        {
            Debug.Log("모든 스테이지 클리어!");
        }
    }

    // Spwan매니저와의 징검다리
    public void Respwan()
    {
       // Debug.Log("현재스테이지 " + StageChannel.stageName + "등록된 몬스터 "+ StageChannel.cur_MonsterData.monsterName);
        //MonsterData[] tempMD = monsterDB.GetRandomNormalMData().ToArray();
        //theRM.Respawn(stages[currentStageNum].go_EnemyRespawns, stages[currentStageNum].isBossStage);

        theRM.RespawnMonster(stageList[currentStageNum].go_EnemyRespawns);
    }
    
    public GameObject[] GetEnemyRespwans()
    {
        return stageList[currentStageNum].go_EnemyRespawns;
    }

    public GameObject GetPlayerRespawn()
    {
        return stageList[currentStageNum].go_PlayerRespawn;
    }


    //public IEnumerator SpawnWaves()
    //{
    //    for (int i = 0; i < stages[currentStageNum].wave; i++)
    //    {
    //        if (i == stages[currentStageNum].wave - 1)
    //        {
    //            stages[currentStageNum].isBossStage = true;
    //        }
    //        Respwan();
    //        yield return new WaitForSeconds(5f);
    //    }
    //}

    // 보스까지 한 챕터를 클리어한 경우 
    public void ClearStage()
    {
        // TODO
        // 씬을 나눴으니 해당 함수는 게임 씬에선 호출 못하고 데이터를 저장하고 씬을 새로 그릴 때
        // 반영하도록 해야한다. 
        var curStage = StageInfoManager.instance.GetStageInfo();
        if (curStage == null) return;

        curStage.isCleared = true;
    }

  
    
  

   
}
