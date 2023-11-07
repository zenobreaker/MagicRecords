using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;


/// <summary>
/// 인게임에 사용되는 HUD 매니저 
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public Canvas GameUI;
    private GameObject[] keybindButtons;
    public ActionButton[] actionButtons = null;
    public FloatingText floatingText;

    // HUD를 보여줄 선택한 플레이어 정보
    public Character selctPlayer; 

    [Header("캐릭터 체력 및 마나")]
    [SerializeField]
    private Gauge health = null;
    [SerializeField]
    private Gauge mana = null;
    [SerializeField] Gauge currrentCP = null;

    public GameObject BuffUiBase;
    public BuffIcon buffIcon;

    public Gauge MyHP
    {
        get { return health; }
    }
    public Gauge MyMP
    {
        get { return mana; }
    }

    public Gauge MyCP
    {
        get { return currrentCP; }
    }

    [Header("캐스팅바")]
    [SerializeField] GameObject baseCastingBar = null;
    [SerializeField] Image castingBar = null;
    [SerializeField] Image skillIcon = null;
    [SerializeField] Text skillName = null;
    [SerializeField] Text castTime = null;
    
    [Header("적 체력바")]
    [SerializeField]
    private GameObject enemyHealthBar = null;
    private Gauge enemyHealthStat;

    [Header("점수 및 남은 적 확인")]
    [SerializeField] private Text scoreText = null;
    [SerializeField] private Text enemyCounterText = null;

 
    private void Awake()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<UIManager>();
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        //Tag가 keybind로 설정된 게임 오브젝트를 찾는다.
        keybindButtons = GameObject.FindGameObjectsWithTag("keybind");

        //  DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {

        if (scoreText == null)
        {
            scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        }

        enemyHealthStat = enemyHealthBar.GetComponentInChildren<Gauge>();
        UpdateGameScore();
        UpdateCounter();

        //SetQuickSlot();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // 선택한 캐릭터 hp/mp 등 스텟 관련 업데이트 
        UpdateHUDBySelecctedChar();

        UpdateGameScore();
        UpdateCounter();
        //if(CharStat.instance.currentCP == 10)
        //    UpdateChainIcon();
    }

    // HUD랑 선택된 캐릭터 정보랑 연결
    public void SetHUDBySelectedCharacter(Character _selectedPlayer)
    {
        if (_selectedPlayer == null) return;
        
        selctPlayer = _selectedPlayer;

        MyHP.Initalize(selctPlayer.MyCurrentHP, selctPlayer.MyMaxHP);
        MyMP.Initalize(selctPlayer.MyCurrentMP, selctPlayer.MyMaxMP);
        // TODO : CP 시스템 추가 
        //MyCP.Initalize(status.); 
    }

    // 선택한 플레이어의 스탯에 따른 HUD 업데이트 하기 
    public void UpdateHUDBySelecctedChar()
    {
        if (selctPlayer == null) return;
        
        MyHP.MyCurrentValue = selctPlayer.MyCurrentHP;
        MyMP.MyCurrentValue = selctPlayer.MyCurrentMP;
    }
 
    // HP와 MP는 리젠 시키는 값이 있다. 
    public void UpdateRegenValue()
    {
        // 리젠 타이머는 1초 고정 초당 회복량이 달라진다.
        //if(Time.deltaTime <= 60)
//        MyHP.MyCurrentValue 
    }


    public void OpenClose(CanvasGroup canvasGroup)
    {
        Debug.Log(canvasGroup.name);

        //투명값으로 UI를 끄거나 킨다.
        canvasGroup.alpha = canvasGroup.alpha > 0 ? 0 : 1;
        //UI가 켜져 있을 땐 레이케스트 충돌이 되도록 만들고
        //UI가 꺼져 있을 땐 레이케스트 충돌이 무시되어 다른 조작(적 선택)등을
        //할 수 있게 한다.
        canvasGroup.blocksRaycasts = (canvasGroup.blocksRaycasts) == true ? false : true;

        //   Debug.Log("타임스탑");
        //   if(canvasGroup.name != "Info")
        //       Time.timeScale = Time.timeScale > 0 ? 0 : 1;

    }

    public void UpdateKeyText(string key, KeyCode code)
    {
        //Text tmp = Array.Find(keybindButtons, x => x.name == key).GetComponentInChildren<Text>();
        //tmp.text = code.ToString();
    }

 
    // Onclick 이벤트 대신 호출 후 사용 
    public void ClickActionButton(string buttonName)
    {
        //ActionButton.Start()의 MyButton.onClick.AddListener(OnClick);로
        //등록된 함수가 호출된다.
        Debug.Log("호출된 이름" + buttonName);
        Array.Find(actionButtons, x => x.gameObject.name == buttonName).MyButton.onClick.Invoke();
    }


    public void ShowHealthBar(Character character)
    {
        enemyHealthBar.SetActive(true);
        enemyHealthStat.Initalize(character.MyCurrentHP, character.MyMaxHP);
    }


    public void ShowHealthBar(WheelerController wheeler)
    {
        if (wheeler == null) return;
        ShowHealthBar(wheeler.MyPlayer);

        int count = 0; 
        foreach (var buff in wheeler.buffDebuffs)
        {
            if (buff == null) continue;

            // 기존에 버프 오브젝트가 있다면 그녀석을 활성화
            // 버프가 오면 그 오브젝트를 생성한다.
            // 오브젝트가 있는지 검사 
            if (BuffUiBase.transform.childCount >= wheeler.buffDebuffs.Count)
            {
                var buffObject = BuffUiBase.transform.GetChild(count);
                if (buffObject != null)
                {
                    // 오브젝트가 있다면 오브젝트에 버프 정보 전달 
                    if (buffObject.TryGetComponent<BuffIcon>(out var icon))
                    {
                        icon.Init(buff);
                        count++;
                    }
                }
            }
            else
            {
                // 없으면 새로 생성
                var buffIconIns = Instantiate(buffIcon, BuffUiBase.transform);
                buffIconIns.Init(buff);
            }
        
        }

    }

    public void HideHealthBar()
    {
        if (enemyHealthStat.fillAmo <= 1)
            enemyHealthBar.SetActive(false);
    }

    // 스킬 퀵슬롯에 등록시키기 
    public void SetQuickSlot(PlayerControl p_targetPlayer)
    {
        for (int i = 0; i < actionButtons.Length; i++)
        {
            actionButtons[i].playerControl = p_targetPlayer;
            actionButtons[i].SetSkill(i);
            
            if (p_targetPlayer.MyPlayer.skills[(SkillSlotNumber)i] != null 
                && p_targetPlayer.MyPlayer.skills[(SkillSlotNumber)i].isChain)
            {
                 actionButtons[i].ActiveChainIcon();
            }
            //  if(p_targetPlayer.MyPlayer.MySkills[i].IsChain)

        }
        
    }

    void SetChainSlot(int idx)
    {
        actionButtons[idx].isChain = true;
        
    }

    // 점수 갱신
    public void UpdateGameScore()
    {
        if (scoreText == null)
        {
            return;
        }

        scoreText.text = "점수 : " + GameManager.MyInstance.MyGameScore.ToString();
    }

    public void UpdateCounter()
    {
        if(enemyCounterText == null)
        {
            return; 
        }

        enemyCounterText.text = 
            "Wave : " + GameManager.MyInstance.currentWave.ToString() + "/" +
            GameManager.MyInstance.maxWave.ToString();
    }

    public void ActionButtonReset()
    {
        for (int i = 0; i < actionButtons.Length; i++)
        {
            actionButtons[i].ResetSkillCoolTime();
        }
    }
   
    public void CreateFloatingText(Vector3 _pos, string _dmg, bool isCrit = false)
    {
        var clone = DamageObjectPooler.instance.GetObject(_pos);
        //var clone = DamageObjectPooler.instance.GetObject();
        if (clone.TryGetComponent(out FloatingDynamicText floatingText))
        {
            Debug.Log("대미지 " + _dmg);
            floatingText.SetText(_dmg, _pos, isCrit);
        }
    }

    public void CastSkill(ActiveSkill _skill)
    {
        castingBar.fillAmount = 0;
        skillName.text = _skill.CallSkillName;
        skillIcon.sprite = _skill.MyIcon;
        
        StartCoroutine(Progress(_skill.MyCastTime));
    } 


    IEnumerator Progress(float _castTime)
    {
        float timePassed = Time.deltaTime;
        float rate = 1.0f / _castTime;
        float progress = 0f;
        
        baseCastingBar.SetActive(true);
        
        while(progress <= 1.0f)
        {
            castingBar.fillAmount = Mathf.Lerp(0, 1, progress);
            progress += rate * Time.deltaTime;
            timePassed += Time.deltaTime;
            castTime.text = (_castTime - timePassed).ToString("F2");

            if (_castTime - timePassed < 0)
                castTime.text = "0";

            yield return null;
        }
        baseCastingBar.SetActive(false);
    }


    

    // 스킬 발사가 완료되거나, 취소시 호출
    public void StopCasting()
    {
        StopAllCoroutines();
    }
}
