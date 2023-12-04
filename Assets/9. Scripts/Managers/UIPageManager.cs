using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.ReloadAttribute;

// UI�� ������������ �������ִ� �Ŵ��� 

public class UIPageManager : MonoBehaviour
{
    public static UIPageManager instance;
    [SerializeField] ChoiceAlert choiceAlert = null; // ĳ���� ���� 
    [SerializeField] GameObject btn_Back = null; // �ڷΰ��� ��ư 
    [SerializeField] SlotTooltip toolTip = null;
    [SerializeField] SkillManual skillManual = null;

    // �˾� ������ ���� ���� 
    int prevStackCount;
    // �˾� â�� ������ ���� 
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

    // ���� ������ �����ΰ�?
    private void LateUpdate()
    {
        // ���� ������ ���� ������ �ٸ��ٸ� ���� �Լ��� ȣ�� ��Ű�� �Ѵ�. 
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

    // UI���� ���������� stack�� �־ �����ϵ��� �Ѵ�.

   
    public void OpenClose(GameObject _gameObject)
    {
        if (_gameObject == null)
        { 
            return; 
        }

        if (_gameObject.activeSelf)
        {

            //Debug.Log("���� ������ oc : " + popupList.Peek().name + popupList.Count);
            // 1. ���� ����Ʈ�� ��� ���� 
            //if (popupList.Count > 0)
            //    popupList.Pop();

            // 2. ��� ����Ʈ�� �Ŀ� ���� �׷��� ������ �ش� ������Ʈ�� disable �Լ��� ���� ȣ��Ǿ� 
            // ���⸦ Ż �� �ִ�. (�� �Լ��� ȣ���� �ߴٸ�) ^^
            //_gameObject.SetActive(false);
            BackPage();
            //SoundManager.instance.PlaySE("Escape_UI");
        }
        else
        {
            popupList.Push(_gameObject);
            _gameObject.SetActive(true);
            Debug.Log("���� ������ oc: " + popupList.Peek().name + popupList.Count);
            SoundManager.instance.PlaySE("Confirm_Click");
            if (btn_Back != null)
            {
                btn_Back.SetActive(true);
            }

        }

        // todo �ѽ������� uibase�� refreshui �Լ��� ȣ���ϵ��� �Ѵ�.
        if (popupList.Count > 0)
        {
            var ui = popupList.Peek();
            if(ui.TryGetComponent<UiBase>(out var uiBase))
            {
                uiBase.RefreshUI();
            }
        }
    }

    // �� �Լ����� �ݹ��� �ִٸ� �����ϴ� �Լ� 
    public void OpenClose(GameObject _gameObject, Callback callback = null)
    {
        OpenClose(_gameObject);
        // �ݹ��� �ִٸ� ����
        callback?.Invoke();
    }

    public void BackPage()
    {
        if (popupList.Count > 0)
        {
            Debug.Log("���� ������ : " + popupList.Peek().name + popupList.Count);
            GameObject temp = popupList.Peek();
            popupList.Pop();
            temp.SetActive(false);

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

    // ����â ���� 
    public void OpenToolTip(Slot _invenSlot)
    {
        if (toolTip == null || _invenSlot == null) return;

        // ������ �����ش�
        OpenClose(toolTip.gameObject);
        toolTip.ShowToolTip(_invenSlot);
    }

    // ĳ���� ���� Ŭ���� ��ȯ 
    public ChoiceAlert GetChoiceAlert()
    {
        return choiceAlert;
    }

    // ĳ���� ����â ����
    public void OpenSelectCharacter(Callback<Character> _callback = null)
    {
        if (choiceAlert == null) return;

        // 캐릭터 선택 UI 활성화
        choiceAlert.ActiveAlert(true);
        choiceAlert.ConfirmSelect(player => _callback(player));

    }

    // ��ųâ ���� - �� �Լ��� ���������� ��ų�Ŵ����� �� �� ȣ���Ѵ�.
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
