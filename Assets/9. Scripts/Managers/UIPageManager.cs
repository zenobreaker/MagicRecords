using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UI를 스택형식으로 관리해주는 매니저 

public class UIPageManager : MonoBehaviour
{
    public static UIPageManager instance;
    [SerializeField] ChoiceAlert choiceAlert = null; // 캐릭터 선택 
    [SerializeField] GameObject btn_Back = null; // 뒤로가기 버튼 
    [SerializeField] SlotTooltip toolTip = null;
    [SerializeField] SkillManual skillManual = null;

    // 팝업 스택의 지난 개수 
    int prevStackCount;
    // 팝업 창을 관리할 스택 
    public Stack<GameObject> popupList = new Stack<GameObject>();


    public delegate void Callback<T>(T t);
    public delegate void Callback();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

    }

    private void Start()
    {
        if(choiceAlert == null)
        {
            choiceAlert = FindObjectOfType<ChoiceAlert>(); 
        }

        if (toolTip == null)
        {
            toolTip = FindObjectOfType<SlotTooltip>();
        }

    }

    // 나름 옵저버 패턴인것?
    private void LateUpdate()
    {
        // 지난 개수와 현재 개수가 다르다면 갱신 함수를 호출 시키게 한다. 
        if(prevStackCount != popupList.Count && popupList.Count >= 1)
        {
            var currentPopup = popupList.Peek();
            var uiBase = currentPopup.GetComponent<UiBase>(); 
            if (uiBase != null)
            {
                uiBase.RefreshUI();
            }
            prevStackCount = popupList.Count; 
        }
    }
    public GameObject GetTopPopupList()
    {
        return popupList.Pop();
    }

    // UI들을 전반적으로 stack에 넣어서 관리하도록 한다.

   
    public void OpenClose(GameObject _gameObject)
    {
        if (_gameObject == null)
        { 
            return; 
        }

        if (_gameObject.activeSelf)
        {

            Debug.Log("닫을 페이지 oc : " + popupList.Peek().name + popupList.Count);
            // 1. 먼저 리스트에 대상 제거 
            if (popupList.Count > 0)
                popupList.Pop();

            // 2. 대상 리스트를 후에 끈다 그렇지 않으면 해당 오브젝트의 disable 함수가 먼저 호출되어 
            // 여기를 탈 수 있다. (이 함수를 호출을 했다면) ^^
            _gameObject.SetActive(false);

            SoundManager.instance.PlaySE("Escape_UI");
        }
        else
        {
            popupList.Push(_gameObject);
            _gameObject.SetActive(true);
            Debug.Log("열린 페이지 oc: " + popupList.Peek().name + popupList.Count);
            SoundManager.instance.PlaySE("Confirm_Click");
            if (btn_Back != null)
            {
                btn_Back.SetActive(true);
            }

        }

        // todo 한시적으로 uibase의 refreshui 함수를 호출하도록 한다.
        if (popupList.Count > 0)
        {
            var ui = popupList.Peek();
            if(ui.TryGetComponent<UiBase>(out var uiBase))
            {
                uiBase.RefreshUI();
            }
        }
    }

    // 위 함수에서 콜백이 있다면 실행하는 함수 
    public void OpenClose(GameObject _gameObject, Callback callback = null)
    {
        OpenClose(_gameObject);
        // 콜백이 있다면 실행
        callback?.Invoke();
    }

    public void BackPage()
    {
        if (popupList.Count > 0)
        {
            Debug.Log("닫을 페이지 : " + popupList.Peek().name + popupList.Count);
            GameObject temp = popupList.Peek();
            temp.SetActive(false);
            popupList.Pop();

            if (popupList.Count == 0)
            {
                btn_Back.SetActive(false);
            }

        }
        SoundManager.instance.PlaySE("Escape_UI");
    }

    public void ChangeButtonAtoB(Button btn_A, Button btn_B)
    {
        btn_A.gameObject.SetActive(false);
        btn_B.gameObject.SetActive(true);
    }

    public void Cancel(GameObject p_gameObject)
    {
        if (p_gameObject != null)
        {
            p_gameObject.SetActive(false);
            if (popupList.Contains(p_gameObject))
            { 
                popupList.Pop();
            }
        }
    }

    // 툴팁창 열기 
    public void OpenToolTip(Slot _invenSlot)
    {
        if (toolTip == null || _invenSlot == null) return;

        // 툴팁을 열어준다
        OpenClose(toolTip.gameObject);
        toolTip.ShowToolTip(_invenSlot);
    }

    // 캐릭터 선택 클랙스 반환 
    public ChoiceAlert GetChoiceAlert()
    {
        return choiceAlert;
    }

    // 캐릭터 선택창 열기
    public void OpenSelectCharacter(Callback<Character> _callback = null)
    {
        if (choiceAlert == null) return;

        // 선택 UI를 연다 - 여기서 true를 주면 저쪽에서 동작을 할 때 알아서 끈다 
        choiceAlert.ActiveAlert(true);
        //OpenClose(choiceAlert.gameObject);
        // 확인 버튼 세팅 
        // 확인을 눌렀을 때 선택한 캐릭터가 있으면 행위를 진행한다. 
        choiceAlert.ConfirmSelect(player => _callback(player));

    }

    // 스킬창 열기 - 이 함수는 간접적으로 스킬매뉴얼을 열 때 호출한다.
    public void OpenSkillManual(Character _character)
    {
        if (_character == null || skillManual == null) return; 
        
        if (SkillManual.instance != null)
        {
            SkillManual.instance.OpenBaseUI(_character);
        }
        else
        {
            skillManual.OpenBaseUI(_character);
        }
    }
}
