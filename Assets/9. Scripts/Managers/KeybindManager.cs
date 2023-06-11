using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class KeybindManager : MonoBehaviour
{
    //싱글톤
    private static KeybindManager instance;
    public static KeybindManager MyInstance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<KeybindManager>();
            }
            return instance;
        }
        set
        {
            instance = value;
        }
    }

    public Dictionary<string, KeyCode> Keybinds { get; set; }
    public Dictionary<string, KeyCode> ActionBinds { get; set; }

    private string bindName;

    private void Start()
    {
        Keybinds = new Dictionary<string, KeyCode>();
        ActionBinds = new Dictionary<string, KeyCode>();

        //처음에 키를 세팅한다. 
        BindKey("UP", KeyCode.UpArrow);
        BindKey("LEFT", KeyCode.LeftArrow);
        BindKey("DOWN", KeyCode.DownArrow);
        BindKey("RIGHT", KeyCode.RightArrow);
      //  BindKey("ATTACK", KeyCode.Space);
        
        BindKey("ACTION1", KeyCode.A);
        BindKey("ACTION2", KeyCode.S);
        BindKey("ACTION3", KeyCode.D);
        BindKey("ACTION4", KeyCode.F);
        

    }

    public void BindKey(string key, KeyCode keyBind)
    {
        Dictionary<string, KeyCode> currentDictionary = Keybinds;

        //Key에 ACTION문자가 포함되어 있다면
        if (key.Contains("ACTION"))
        {
            // currentDictionary는 액션키를 담당하는
            //ActionBinds로 변경
            currentDictionary = ActionBinds;
        }
        //currentDictionary의 키에 key가 없다면 
        if (!currentDictionary.ContainsKey(key))
        {
            //버튼을 추가한다.
            currentDictionary.Add(key, keyBind);

            //버튼이 이름을 keybind menu UI에 표시한다.
            UIManager.instance.UpdateKeyText(key, keyBind);
        }
        else if (currentDictionary.ContainsValue(keyBind))
        {
            //Dictionary의 배열에서 값으로 키를 찾는 방법이다.
            //같은 값을 지닌 키는 배열 순서가 제일 앞인 녀석이 반환된다.
            string myKey = currentDictionary.FirstOrDefault(x => x.Value == keyBind).Key;

            //변경하려는 키가 이미 사용중이라면 
            //이전에 사용 중이던 키를 없앤다. 
            currentDictionary[myKey] = KeyCode.None;
            UIManager.instance.UpdateKeyText(key, KeyCode.None);
        }
        //키를 등록시킨다.
        currentDictionary[key] = keyBind;
        UIManager.instance.UpdateKeyText(key, keyBind);
        bindName = string.Empty;
    }

    public void KeyBindOnClick(string bindName)
    {
        this.bindName = bindName;
    }

    //Upadate처럼 매프레임마다 호출된다.
    private void OnGUI()
    {
        if(bindName != string.Empty)
        {
            // UnityGUI에서 발생한 이벤트 
            Event e = Event.current;

            //발생한 이벤트가 키 입력이라면.
            if (e.isKey)
            {
                BindKey(bindName, e.keyCode);
            }
        }
    }
}
