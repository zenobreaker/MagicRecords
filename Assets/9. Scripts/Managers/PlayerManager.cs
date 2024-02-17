using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using UnityEditor.Localization.Platform.Android;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab;

    private List<GameObject> playerableObjectList = new List<GameObject>();

    [SerializeField]
    private FollowCamera followCamera = null;

    public PositionController positionController;

    public JoyStick theJoySitck; 

    private Dictionary<int ,PlayerControl> playerUnion = new Dictionary<int, PlayerControl>(); 

    public void InitMyTeam()
    {
        playerUnion.Clear();
        playerUnion.Add(1, null);
        playerUnion.Add(2, null);
        playerUnion.Add(3, null);
    }

    public void CreateCharacter()
    {
        InitMyTeam();

        int count = 1;

        // 플레이어 정보 
        if (InfoManager.instance != null)
        {
            // 플레이어 객체 정보 및 오브젝트랑 가져와서 합치기
            var infoList = InfoManager.instance.GetSelectPlayerList();
            foreach (var data in infoList)
            {
                if (data == null) continue;

                var playerablebject = PlayerDatabase.instance.GetCharacterData(data.MyID);
                if (playerablebject == null)
                {
                    // player 데이터가 생성되지 않았다면 리턴
                    Debug.Log("Create Error : 캐릭터가 생성되지 않았습니다." + "ID : " + data.MyID);
                    continue;
                }
                var player = Instantiate(playerablebject.prefab, Vector3.zero, Quaternion.identity);

                // 캐릭터를 생성하면 플레이어 컨트롤의 값을 조정 
                if (player.TryGetComponent<PlayerControl>(out var playerControl))
                {
                    // 데이터 세팅 
                    playerControl.MyPlayType = PlayType.Playerable;
                    playerControl.teamTag = TeamTag.TEAM;
                  
                    playerControl.MyPlayer = data;
                    // 능력치 적용 
                    playerControl.MyPlayer.MyStat.ApplyOption();
                    //메인캐릭터에 hud 적용하기 
                    playerControl.MyPlayer.MyCurrentHP = data.MyCurrentHP;
                    playerControl.MyPlayer.MyCurrentMP = data.MyCurrentMP;

                    playerUnion[count]= playerControl;
                    if (count == 1)
                    {
                        GameManager.MyInstance.playerCount += count;
                        // UI 정보 세팅
                        if (UIManager.instance != null)
                        {
                            Debug.Log("캐릭터 UI 세팅");
                            var playerStat = playerControl.MyPlayer;

                            // 캐릭터 카메라 세팅 
                            followCamera.setOffset(player.transform);
                            
                            // 조이스틱 세팅
                            if(theJoySitck != null)
                            {
                                theJoySitck.SetPlayer(playerControl);
                            }
                            playerControl.isLeader = true;
                            UIManager.instance.SetHUDBySelectedCharacter(playerStat);
                            UIManager.instance.SetQuickSlot(playerControl);
                        }

                    }
                    else
                    {
                        playerControl.isAutoFlag = true;
                        playerControl.MyPlayType = PlayType.None;
                    }
                }

                count++;
            }


        }


        if (count == 1)
        {
            // 테스트 임시 플레이어 객체 생성 
            CreateTestPlayer();
        }

        if (count > 1)
        {
            GameManager.isCharacterOn = true;
        }
        else
        {
            GameManager.isCharacterOn = false;
        }
    }

    // 테스트용 임시 객체 플레이어 생성 
    public void CreateTestPlayer()
    {

        var playerData = InfoManager.instance.GetMyPlayerInfo(1);
        var player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        if (player.TryGetComponent<PlayerControl>(out var playerControl))
        {
            playerControl.MyPlayer = playerData;
            playerControl.MyPlayer.MyStat.ApplyOption();
            //메인캐릭터에 hud 적용하기 
            playerControl.MyPlayer.InitCurrentHP();
            playerControl.MyPlayer.InitCurrentMP();
            playerControl.isLeader = true;
            //targetPlayer = InfoManager.instance.GetPlayerInfo(1001);
            playerableObjectList.Add(player);
             // UI 정보 세팅
            if (UIManager.instance != null)
            {
                Debug.Log("캐릭터 UI 세팅");
                var playerStat = playerControl.MyPlayer;

                // 캐릭터 카메라 세팅 
                followCamera.setOffset(player.transform);

                UIManager.instance.SetHUDBySelectedCharacter(playerStat);
                UIManager.instance.SetQuickSlot(playerControl);
            }
        }
    }


    // 캐릭터의 체력을 복구 시킨다.
    public void RecoveryHPForMyTeam()
    {
        // 훈련장에서 0이 되었다면 캐릭터 체력을 다시 복구시켜준다. 
        foreach (var wheeler in playerUnion)
        {
            if (wheeler.Value == null || wheeler.Value.MyPlayer == null)
                continue;

            wheeler.Value.MyPlayer.InitCurrentHP();
        }
    }

    public PlayerControl GetPlayer()
    {
        return playerUnion[1];
    }

    // 플레이어 위치 할당하기 
    public void PlayerSetPos(Vector3 _pos)
    {
        foreach(var player in playerUnion)
        {
            if (player.Value == null)
                continue;

            if (player.Value.isLeader == false)
                continue;
            player.Value.transform.position = _pos;
            break; 
        }

        // todo 여기에 캐릭터 설정하기.
        if(positionController != null)
        {
            positionController.SetPlayerList(playerUnion.Values.ToList());
            positionController.SetPartFormation(); 

            foreach(var player in playerUnion)
            {
                if (player.Value == null)
                    continue;

                if (player.Value.isLeader == true)
                    continue;

                player.Value.transform.position = player.Value.GetDestinationPosition();
            }
        }
    }

    public void RemovePlayer()
    {
        if (playerableObjectList.Count != 0)
        {
            foreach(var obj in playerableObjectList)
            {
                Destroy(obj.gameObject);
            }
            playerableObjectList.Clear();
        }


    }

    public List<WheelerController> GetMyTeam()
    {
        List<WheelerController> list = new List<WheelerController>();

        foreach(var wheeler in playerUnion)
        {
            list.Add(wheeler.Value);
        }

        return list;
    }

    public int GetMyTeamCount()
    {
        int count = 0; 
        foreach (var wheeler in playerUnion)
        {
            if (wheeler.Value != null)
                count++;
        }

        return count; 
    }

    // 대상 플레이어를 조작할 수 있는 플레이어로 변경한다.
    public void SetControlTheCurrentPlayer(Character target, bool isAutoFlag = false)
    {
        if (target == null)
            return;

        Debug.Log("캐릭터 UI 세팅");

        UIManager.instance.SetHUDBySelectedCharacter(target);

        Dictionary<int, PlayerControl> copyDict = new Dictionary<int, PlayerControl>(playerUnion);

        int count = 2; 
        foreach(var playerControl in copyDict)
        {
            if (playerControl.Value == null) 
                continue;

            if (playerControl.Value.MyPlayer.MyID == target.MyID)
            {
                UIManager.instance.SetQuickSlot(playerControl.Value);
                // 캐릭터 카메라 세팅 
                followCamera.setOffset(playerControl.Value.transform);
                // 조이스틱 세팅
                if(theJoySitck != null)
                    theJoySitck.SetPlayer(playerControl.Value);

                // 리더로 승격
                playerControl.Value.isLeader = true;

                // 조작 관련 값을 수정한다.
                SetControlledPlayer(playerControl.Value, true, isAutoFlag);
                playerUnion[1] = playerControl.Value;
            }
            else
            {
                playerControl.Value.isLeader = false;
                SetControlledPlayer(playerControl.Value, false, true);
                playerUnion[count] = playerControl.Value;
                count++; 
            }
        }

    }

    // 플레이어 초기화를 담당하는 별도의 함수를 추가하여 중복 코드를 방지한다.
    private void SetControlledPlayer(PlayerControl playerControl, bool isControlled, bool isAutoFlag)
    {
        if (playerControl.isDead == true)
        {
            return;
        }
        // NavMeshAgent 초기화 및 상태 설정
        playerControl.MyAgent.ResetPath();
        playerControl.MyAgent.isStopped = isControlled; // 수동 제어 시 멈춤 여부
        /*playerControl.MyAgent.updateRotation = isControlled;
        playerControl.MyAgent.updatePosition = isControlled;*/

        // 속도 및 상태 설정
        playerControl.MyAgent.velocity = Vector3.zero;
        playerControl.MyAgent.speed = isControlled ? 0 : playerControl.speed; // 수동 제어 시 속도 감소
        //playerControl.isLeader = isControlled;
        playerControl.isAutoFlag = isAutoFlag;
        playerControl.MyPlayType = isControlled ? PlayType.Playerable : PlayType.None;

    }


    // 선택한 캐릭터들 결과 반영
    public void RefreshCharStatus()
    {
        foreach (var own in playerUnion)
        {
            if (own.Value == null) continue; 

            int id = own.Value.MyPlayer.MyID;

            int hp = own.Value.MyPlayer.MyCurrentHP;
            Debug.Log("캐릭터 ID : " + id + " Hp : " + hp);

            InfoManager.instance.SetSelectMyPlayerApplyData(id, own.Value.MyPlayer);
        }
    }


    // 플레이어 캐릭터의 오토모드 전환
    public void ChangeAutoModeForMyTeam(bool isAutoMode)
    {
        foreach (var wheeler in playerUnion)
        {
            if (wheeler.Value == null) continue;

            if(wheeler.Value.isLeader == true)
            {
                SetControlledPlayer(wheeler.Value, !isAutoMode, isAutoMode);
            }
        }

    }
}
