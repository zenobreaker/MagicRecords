using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


// 게임 진행 스테이트 
public enum GameState
{
    NONE,
    START,
    STAND_BY_PLAY,
    PLAYING,
    END,
}

public class GameManager : MonoBehaviour
{
    public static GameManager MyInstance;

    public Animator stageAnim;
    public Text stageText;

    public PlayerControl player;
    public string stageName;
    public bool isStageIn = false;
    public bool isStageEnd = false;             // 게임 종료 여부 판단할 변수
    public bool isStageClear = false;
    bool isRoutine = false;
    public static bool isCharacterOn = false;   // 캐릭터 생성 여부 

    public int playerCount;                   // 게임 내 아군 수
    public int enemyCount;                    // 게임 내 적 수 
    public int itemCount;
    public int currentWave;
    public int maxWave;

    public bool isTest = false;
    // 게임 획득 점수 
    private int gameScore;

    GameState gameState;

    private float spwanDelayTime = 0.23f;
    private float currentTime = 0;
    private bool isSpawn = false;
    private int spawnCount = 0 ;

    public int MyGameScore
    {
        set { gameScore = value; }
        get { return gameScore; }
    }

    // 필요한 컴포넌트 
    [SerializeField] private StageManager theSM = null;
    [SerializeField] private PlayerManager thePM = null;
    [SerializeField] ComboManager theCombo = null;

    // 이번 게임에 등장하는 모든 아군 플레이어 
    List<WheelerController> team = new List<WheelerController>();

    private void Awake()
    {
        if (MyInstance == null)
            MyInstance = FindObjectOfType<GameManager>();
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // 스테이지 초기화
        StageInit();
    }

    private void Update()
    {
        if(gameState == GameState.START)
        {
            // 인게임 요소 체크 
            // 1. 캐릭터 
            // 2. 적 생성할 수 
            // 3. wave 수치 
            if(playerCount > 0 && enemyCount > 0 && currentWave >0)
            {
                gameState = GameState.STAND_BY_PLAY;
                isSpawn = true;
            }
        }
        else if(gameState == GameState.STAND_BY_PLAY)
        {
            // 게임을 진행한다. 
            // 웨이브가 0이되면 게임은 끝
            // 이 스테이트에 오면 전장에 적들을 배치한다. 
            // 스테이지 매니저에게 스폰 명령 
            if (isSpawn == true)
            {
                isSpawn = false;
                theSM.RespwanEnemy();
            }

            // 레코드매니저에게 유저가 선택한 레코드를 적용시키라고 명령한다. 
            if(RecordManager.instance != null)
            {
                RecordManager.instance.ApplyRecordToPlayers(team);
            }

            gameState = GameState.PLAYING;
        }
        else if(gameState == GameState.PLAYING)
        {
            if (currentWave <= 0)
            {
                gameState = GameState.END;
            }
        }
        else if(gameState == GameState.END)
        {

        }
    }

    public void SaveEnemyData(MonsterGrade monsterType, uint id)
    {
        theSM.SaveEnemy(monsterType, id);
    }

    public void SaveEnemyWithGM(GameObject _enemy, MonsterGrade p_monstertType = MonsterGrade.NORMAL)
    {
        theSM.SaveEnemy(_enemy, p_monstertType);
    }

    public void SaveEnemyWithGM(GameObject[] _enemies)
    {
        theSM.SaveEnemies(_enemies);
    }

    public void StageInit()
    {
        // 플레이어 적 수 초기화 
        playerCount = 0;
        enemyCount = 0;
        spawnCount = 0;
        gameScore = 0;
        currentTime = 0.0f;
        currentWave = 0;
        maxWave = 0;
        
        gameState = GameState.NONE;

        team.Clear(); 

        isStageIn = true;
        isStageClear = false;
        isStageEnd = false;
        // 게임  시작 시 타임 스케일이 0이라면 1로 변경해서 진행한다. 
        if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        Debug.Log("게임 스테이지 " + stageName);

        StartCoroutine(StartStageCoroutine());
    }

    public void TestStage()
    {
        isTest = true;

        gameScore = 0;
        isStageIn = true;
        isStageClear = false;
        isStageEnd = false;

        theCombo.ResetCombo();  // 콤보 초기화

        theSM.CreateTestStage();

    }

    public void StageEnd()
    {
        thePM.RemovePlayer();
        if (!isTest)
        {
            theSM.ShowClearUI(false);
        }
        theSM.DisableStage();
        ///LobbyManager.MyInstance.ApearLobbyUI();
    }

    // 게임 클리어 확인
    public void CheckGameClaer()
    {
        // 모든 적을 쓰러뜨렸으면 현재 웨이브 값을 내린다.
        if(enemyCount <= 0)
        {
            currentWave -= 1;
        }
        // 웨이브값을 검사해서 게임이 끝났는지 검사 
        if (currentWave <= 0)
        {
            isStageEnd = true;
            isStageClear = true;
        }
        else if(playerCount <= 0)
        {
            isStageEnd = true;
            isStageClear = false; 
        }
        // 검사했는데 웨이브가 아직 남아있으면 스폰을 한 번 더 시켜야한다.
        else if(currentWave > 0 && isSpawn == false)
        {
            gameState = GameState.STAND_BY_PLAY;
        }

        if (isStageEnd == true && isStageClear == true && isRoutine == false)
        {
            StartCoroutine(StageClearCoroutine());

            Debug.Log("스테이지 클리어!");
        }
        else if (isStageEnd == true &&  isStageClear == false)
        {
            StartCoroutine(StageFailureCoroutine());
           
            Debug.Log("스테이지 클리어 실패!");
        }
    }

    // 스테이지 시작 
    IEnumerator StartStageCoroutine()
    {
        if(StageInfoManager.instance == null)
        {
            yield return null; 
        }

        // 스테이지 생성  
        theSM.CreateStage();
        // 캐릭터 생성 
        thePM.CreateCharacter();
        // 캐릭터 위치 조정
        thePM.PlayerSetPos(theSM.GetPlayerSpawnPosition());
        // 콤보 초기화
        theCombo.ResetCombo();
        player = thePM.GetPlayer();
        team.Add(player);

        if (player != null)
            playerCount = 1; 

        // 목표 수치 설정
        enemyCount = theSM.GetEnemyCount();
        currentWave = theSM.GetSelectStageWaveCount();
        maxWave = theSM.GetSelectStageWaveCount();

        yield return new WaitForSeconds(0.5f);

        // TODO : 스테이지 UI 추가 
        stageText.gameObject.SetActive(true);
      //  stageText.text = "스테이지" + "\n" + stage.stageName;
        stageAnim.SetTrigger("Apear");

        yield return new WaitForSeconds(1.5f);


        if (stageAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            stageText.gameObject.SetActive(false);

        // 게임 진행 스테이트 변경 
        gameState = GameState.START;
    }



    //스테이지 클리어
    IEnumerator StageClearCoroutine()
    {
        isRoutine = true;
        FollowItem[] items = FindObjectsOfType<FollowItem>();

        // 필드에 남아있는 아이템들을 스테이지 클리어 조건 달성 시, 모두 회수하도록 한다
        if (items.Length > 0)
        {
            Debug.Log("아이템 등장" + items.Length);
            itemCount = items.Length;

            for (int i = 0; i < items.Length; i++)
            {
                items[i].viewDistance = Vector3.Distance(player.transform.position, items[i].gameObject.transform.position);
                items[i].followSpeed = 10f; // 유효 사거리를 캐릭터까지 있도록 하여 스피드를 올려서 빨리 먹어질 수 있도록 함
            }
        }

        yield return new WaitUntil(() => itemCount <= 0);
       
        isStageIn = false;
        isStageClear = false;
        isRoutine = false;
        isCharacterOn = false;

        gameState = GameState.END;
        theSM.ClearStage(); // 해당 스테이지 클리어함수 호출
        //yield return new WaitUntil(() => theReward.isConfirm == true);
        StageInfoManager.initJoinPlayGameModeFlag = true;
        RecordManager.CHOICED_COMPLETE_RECORD = false;
        theSM.ShowClearUI(true);

        // 캐릭터 경험치 지급 
        GrowUpMyCharacters(MyGameScore); 
    }

    IEnumerator StageFailureCoroutine()
    {
        isStageClear = false;
        isStageIn = false;
        isCharacterOn = false;
        Time.timeScale = Time.timeScale > 0 ? 0 : 1;

        theSM.ShowClearUI(isStageClear);
        
        // 게임 진행 스테이트 변경 
        gameState = GameState.END;
        yield return null;
    }

    public void IncreaseCombo()
    {
        theCombo.IncreaseCombo();
    }

    public void OpenClose(GameObject _gameObject)
    {
        if (_gameObject.activeSelf)
        {
            _gameObject.SetActive(false);

            SoundManager.instance.PlaySE("Escape_UI");
        }
        else
        {
            _gameObject.SetActive(true);

            SoundManager.instance.PlaySE("Confirm_Click");
        }
    }

    // 설정 열기 
    public void OpenSetting(GameObject _gameObject)
    {

        if (_gameObject.activeSelf)
        {
            _gameObject.SetActive(false);
            SoundManager.instance.PlaySE("Escape_UI");  // 
            Time.timeScale = 1;
        }
        else
        {
            _gameObject.SetActive(true);
            SoundManager.instance.PlaySE("Confirm_Click");
            Time.timeScale = 0;
        }
    }

    // 게임으로 돌아가기 
    public void BackToGame(GameObject _gameObject)
    {
        OpenClose(_gameObject);
        Time.timeScale = 1;
    }

    // 로비로 돌아가기 
    public void BackToTheRoom()
    {
        isStageClear = false;
        isStageIn = false;
        isStageEnd = true;
        isCharacterOn = false;
        StageEnd();
        Time.timeScale = 1;    // 이전 UI에서 시간을 멈췄으므로 모든 씬의 시간을 다시 1로 맞춤
        SoundManager.instance.PlaySE("Confirm_Click");
    }

    // 게임 점수 변경 0525
    public void ChangeGameScore(int downCount, int score)
    {
        enemyCount -= downCount;
        MyGameScore += score;

        // 게임 클리어 확인 
         CheckGameClaer();
    }

    // 게임 내 아군 수 변경하기 
    public void ChanagePlayerTeamCount(int downCount)
    {
        playerCount -= downCount;
        // 게임 클리어 확인 
        CheckGameClaer();
    }

    // 게임을 끝내고 나면 캐릭터들을 성장시켜준다. (소지한 모든 캐릭터)
    public void GrowUpMyCharacters(int _totalExp, float _directRate = 1.0f, float _indirectRate = 1.0f)
    {
        if (InfoManager.instance == null) return;


        // 전장에 나가 싸워낸 캐릭터 
        var battleCharacters = InfoManager.instance.GetSelectPlayerList();

        // 경험치 지급 
        foreach(var character in battleCharacters)
        {
            if (character == null) continue;

            int resultExp = (int)(_totalExp * _directRate);
            character.GrowUp(resultExp); 
        }


        // 내가 소지한 캐릭터 
        var myInfoCharacters = InfoManager.instance.GetUnselectCharacters();
        // 경험치 지급 
        foreach(var pair in myInfoCharacters)
        {
            if (pair.Value == null) continue;

            int resultExp = (int)(_totalExp * _indirectRate);
            pair.Value.GrowUp(resultExp);
        }
    }
}
