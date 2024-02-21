using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StageField { NORMAL, GRASS, ICE, DESERT, CAVE };

[System.Serializable]
public class Stage
{
    public uint stageID;
    public string stageName;
    // todo 이벤트 컷신 정보 변수 추가 
    public GameObject mapObject;
    public GameObject[] enemyRespawns;
    public GameObject playerRespawn;
    public List<GameObject> trapObjectList = new List<GameObject>();
    public int enemyCount;
    public int wave;

    public List<int> appearMonsterIDList;
}
// 실제 게임 플레이에 배치될 Stage Map 등의 데이터를 이용하여 배치해주는 매니저
public class StageManager : MonoBehaviour
{
    [SerializeField] private RespwanManager theRM = null;
    [SerializeField] private StagePosController theSPC = null;
    [SerializeField] MonsterDatabase monsterDB;

    [Header("스테이지 오브젝트")]
    [SerializeField] private List<Stage> stageList = null;
    [SerializeField] RewardController rewardController;


    Stage selectedStage; 

    int currentStageNum = 0;

    Dictionary<uint, MonsterGrade> saveMonsterDic = new Dictionary<uint, MonsterGrade>();


    // 필요한 컴포넌트
    [Header("UI 컴포넌트")]
    [SerializeField] private StageClearUI stageClearUI;
    [SerializeField] private Text txt_CurrentScore = null;
    [SerializeField] private GameObject go_UI = null;
    [SerializeField] private Text txt_StageClearText = null;

    // 스테이 관련 클래스 초기화 
    public void InitializeStageClass()
    {

    }

    public void SavePlayerTransform(Transform p_PlayerTR)
    {
        theRM.SavePlayerTransform(p_PlayerTR);
    }
    public void SaveEnemy(MonsterGrade monsterGrade, uint id)
    {
        saveMonsterDic[id] = monsterGrade;
    }

    public void SaveEnemy(GameObject _enemy, MonsterGrade monstertGrade = MonsterGrade.NORMAL)
    {
        theRM.InitEnemies(_enemy, monstertGrade);
    }

    public void SaveEnemies(GameObject[] _enemies)
    {
        theRM.InitEnemies(_enemies);
    }
    
    

    // todo 스테이지에 나타날 맵 오브젝트 초기화 
    public void SetMapObjecct()
    {
        
    }

    public int GetCurrentStageID()
    {
        if(StageInfoManager.instance != null)
        {
            var stageInfo = StageInfoManager.instance.GetStageAppearInfoByCurrentStageNode();
            if(stageInfo != null)
            {
                return stageInfo.stageID;
            }
        }

        return 0; 
    }

    public void CreateStage()
    {
        if (StageInfoManager.instance == null) 
            return; 

        Debug.Log("스테이지 생성 ID : " );
        
        // 스테이지 이벤트 정보 가져오기 
        var stageAppearInfo = StageInfoManager.instance.GetStageAppearInfoByCurrentStageNode();
        if(stageAppearInfo == null || stageList == null)
        {
            return; 
        }
        
        // 선택한 스테이지 이벤트! 클래스에서 원하는 것을 가져온다.
        // 이벤트 클래스의 진행해야할 정보가 들어있다. 
        switch (stageAppearInfo.stageType)
        {
            case StageType.BATTLE:
          
                int selectMapID = stageAppearInfo.mapID;
                int monsterCount = stageAppearInfo.appearIDList.Count;
                int wave = stageAppearInfo.wave;
                // 가져온 정보에서 맵 찾기
                for (int i = 0; i < stageList.Count; i++)
                {
                    if (stageList[i].stageID == selectMapID)
                    {
                        selectedStage = stageList[i];
                        selectedStage.mapObject?.SetActive(true);
                        selectedStage.enemyCount = monsterCount;
                        selectedStage.wave = wave;

                        selectedStage.appearMonsterIDList.Clear();
                        selectedStage.appearMonsterIDList = stageAppearInfo.appearIDList;
                        selectedStage.enemyCount = selectedStage.appearMonsterIDList.Count;
                        break; 
                    }
                }

                break;
            case StageType.EVENT:
                break;
            case StageType.SHOP:
                break;
            case StageType.MULTY:
                break;
            case StageType.TEST:
                CreateTestStage();
                break;
        }


        if (selectedStage == null)
            return;
        
        // 해당 클래스의 맵 오브젝트 켜주기 
        selectedStage.mapObject.SetActive(true);
    }

    public void CreateTestStage()
    {
        selectedStage = stageList[0];
        stageList[0].mapObject.SetActive(true);
        //theRM.RespawnTestMon(stageList[0].enemyRespawns[0].transform);
    }
    

    // 스테이지의 캐릭터 생성 위치 값 가져오기 
    public Vector3 GetPlayerSpawnPosition()
    {
        if(selectedStage == null || selectedStage.playerRespawn == null)
            return Vector3.zero;

        return selectedStage.playerRespawn.transform.position;
    }
   
    // 적 생성하기 

    // 챕터 설정 (RL모드 한정)
    public void SetChapterWithRL()
    {
        if (RLModeController.isRLMode)
        {
            theSPC.curMainChpaterNum = 1;
        }
    }
    
    public void DisableStage()
    {
        //currentStageNum = -1;
        selectedStage.mapObject.SetActive(false);
        go_UI.SetActive(false);
        theRM.DeleteAllMonster();
    }

    //  적 수 반환하기 
    public int GetEnemyCount()
    {
        if (selectedStage == null)
            return 0;
        
        return selectedStage.enemyCount;
    }

    // 게임 진행 wave 반환
    public int GetSelectStageWaveCount()
    {
        if (selectedStage == null)
            return 0;

        return selectedStage.wave;
    }

    // 결과창 출력
    public void ShowClearUI(bool isClear)
    {
        if(stageClearUI != null)
        {
            stageClearUI.gameObject.SetActive(true);
            stageClearUI.SetClearFlag(isClear);
            stageClearUI.ShowStageClearUI();
        }

        //go_UI.SetActive(true);
        //if (_isClear)
        //{
        //    txt_StageClearText.text = "Stage Clear!!!";
        //}
        //else
        //    txt_StageClearText.text = "Stage Fail...";
        //txt_CurrentScore.text = string.Format("{0:000,000}", GameManager.MyInstance.MyGameScore);
    }

    public void PrevStageBtn()
    {
        // 게임 씬 닫는 로직 작동 (로딩화면)
        Time.timeScale = 1;
        Debug.Log("타임 : " + Time.timeScale);
        //씬 변경
        LoadingSceneController.LoadScene("Lobby");
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
            stageList[currentStageNum++].mapObject.SetActive(false);
            stageList[currentStageNum].mapObject.SetActive(true);
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

    // 적 생성하는 함수 RespwanManager기능을 사용한다.
    public void RespwanEnemy(List<WheelerController> wheelers)
    {
        // 선택한 스테이지 정보 
        if (selectedStage == null || theRM == null)
            return;

        if (selectedStage.appearMonsterIDList == null)
            return;

        wheelers.Clear(); 
        foreach (var id in selectedStage.appearMonsterIDList)
        {
            theRM.RespwanMonsterFormID(selectedStage.enemyRespawns, id, TeamTag.ENEMY);
        }

        foreach(var enemy in theRM.curMonsters)
        {
            wheelers.Add(enemy);
        }
    }

    public void RespwanEnemyByCharacterData(MonsterData data, bool isTest = false)
    {
        if (data == null) return;

        theRM.RespwanMonsterFormID(selectedStage.enemyRespawns, (int)data.monsterID,
            TeamTag.ENEMY, isTest);
    }

    public List<WheelerController> GetStageEnemyList()
    {
        return theRM?.curMonsters;
    }



    // 해당 스테이지 클리어한 경우 
    public void ClearStage(bool isClear)
    {
        // 스테이지인포매니저의 함수를 호출해준다. 
        StageInfoManager.instance.RefreshCurrentStageInfo(true);

        //TODO : 대상에게 해당 관련 보상을 건내준다.
        if (rewardController != null)
        {
            rewardController.GainReward(GetCurrentStageID());
        }

        ShowClearUI(isClear);
    }

    // 스테이지에 있는 대상 전부 제거 명령
    public void RequestClearEnemy()
    {
        if (theRM == null) return;

        theRM.DeleteAllMonster();
    }

    public GameObject GetCurrentMapObject()
    {
        if (selectedStage == null) return null;

        return selectedStage.mapObject;
    }

    public List<GameObject> GetCurrentMapTraPOpbjectList()
    {
        if(selectedStage == null) return null;

        return selectedStage.trapObjectList;
    }
}
