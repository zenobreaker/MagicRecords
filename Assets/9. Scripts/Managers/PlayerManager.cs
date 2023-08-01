using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public GameObject playerPrefab;

    private List<GameObject> playerableObjectList = new List<GameObject>();

    [SerializeField]
    private FollowCamera followCamera = null;
    [SerializeField]
    private ActionController actionController = null;

    private Dictionary<int ,PlayerControl> playerUnion = new Dictionary<int, PlayerControl>(); 

    public void CreateCharacter()
    {
        playerableObjectList.Clear();
        int count = 1;

        // 플레이어 정보 
        if (InfoManager.instance != null)
        {
            // 플레이어 객체 정보 및 오브젝트랑 가져와서 합치기
            foreach (var data in InfoManager.instance.GetSelectPlayerList())
            {
                if (data == null) continue;

                var playerablebject = PlayerDatabase.instance.GetPlayerableObject(data.objectID);
                if (playerablebject == null)
                {
                    // player 데이터가 생성되지 않았다면 리턴
                    Debug.Log("Create Error : 캐릭터가 생성되지 않았습니다." + "ID : " + data.MyID);
                    continue;
                }
                var player = Instantiate(playerablebject.playerObject, Vector3.zero, Quaternion.identity);

                // 캐릭터를 생성하면 플레이어 컨트롤의 값을 조정 
                if (player.TryGetComponent<PlayerControl>(out var playerControl))
                {
                    // 데이터 세팅 
                    playerControl.MyPlayer = data;
                    // 능력치 적용 
                    playerControl.MyPlayer.MyStat.ApplyOption();
                    //메인캐릭터에 hud 적용하기 
                    playerControl.MyPlayer.InitCurrentHP();
                    playerControl.MyPlayer.InitCurrentMP();
                    //targetPlayer = InfoManager.instance.GetPlayerInfo(1001);
                    playerableObjectList.Add(player);
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

                            UIManager.instance.SetHUDBySelectedCharacter(playerStat);
                            //UIManager.instance.MyHP.Initalize(playerStat.MyStat.hp);
                            //UIManager.instance.MyMP.Initalize(playerStat.MyStat.mp);
                            //UIManager.instance.MyCP.Initalize(CharStat.instance.currentCP, CharStat.instance.cp);
                            UIManager.instance.SetQuickSlot(playerControl);
                        }

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

        var playerData = InfoManager.instance.GetMyPlayerInfo(1001);
        var player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        if (player.TryGetComponent<PlayerControl>(out var playerControl))
        {
            playerControl.MyPlayer = playerData;
            playerControl.MyPlayer.MyStat.ApplyOption();
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
                //UIManager.instance.MyHP.Initalize(playerStat.MyStat.hp);
                //UIManager.instance.MyMP.Initalize(playerStat.MyStat.mp);
                //UIManager.instance.MyCP.Initalize(CharStat.instance.currentCP, CharStat.instance.cp);
                UIManager.instance.SetQuickSlot(playerControl);
            }
        }
    }

    public PlayerControl GetPlayer()
    {
        return playerableObjectList[0].GetComponent<PlayerControl>();
    }

    // 플레이어 위치 할당하기 
    public void PlayerSetPos(Vector3 _pos)
    {
        playerableObjectList[0].transform.position = _pos;
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

    // 편성한 캐릭터에게 경험치 주기
    public void SetExpToCurrentPlayer(int _exp)
    {
        //if()
    }
}
