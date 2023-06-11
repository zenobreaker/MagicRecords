using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HandScript : MonoBehaviour
{
    private static HandScript instance;
    public static HandScript MyInstance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<HandScript>();
            }
            return instance;
        }
    }

    //IMoveable은 Skill 에서 상속 받는다. 
    public Skill MyMoveable { get; set; }

    private Image Icon;

    [SerializeField]
    private Vector3 offset = Vector3.zero;


    GraphicRaycaster gr;
    PointerEventData ped;

    // Start is called before the first frame update
    void Start()
    {
        Icon = GetComponent<Image>();

        gr = GetComponent<GraphicRaycaster>();
        ped = new PointerEventData(null);
    }

    // Update is called once per frame
    void Update()
    {
        // 마우스를 따라 아이콘이 이동한다.
        Icon.transform.localPosition = new Vector2(Input.mousePosition.x - (Screen.width / 2),
                                                    Input.mousePosition.y - (Screen.height / 2));

        if (Input.GetMouseButtonDown(0))
        {
            
            ped.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            gr.Raycast(ped, results);

            if (results.Count != 0)
            {
                GameObject obj = results[0].gameObject;
                if (UIPageManager.instance.popupList.Count > 0)
                {
                    Debug.Log(obj.name + "," + UIPageManager.instance.popupList.Peek());
                    if (!obj.CompareTag(UIPageManager.instance.popupList.Peek().tag))
                    {
                        UIPageManager.instance.OpenClose(UIPageManager.instance.GetTopPopupList());
                        // 다른 공간을 누르면 나중에 열어놓은 UI 꺼짐 
                    }
                }
            }
        }
        /*
        if(Input.GetMouseButtonDown(0) && Icon.color.a == 0 )
        {
        
           if(EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if(Physics.Raycast(ray, out hit))
                {
                    Debug.Log(hit.collider.tag);
                }
            }
           
        }*/
    }

    public void TakeMoveable(Skill moveable)
    {
        this.MyMoveable = moveable;

        // 클릭한 스킬 아이콘 정보를  Icon에 담는다.
        Icon.sprite = moveable.MyIcon;
        Icon.color = Color.white;
        // skill.MyIcon 이 IMoveable을 구현한 것이 되었으므로 moveable.myicon은 skill.myicon을 호출하는 것과 같다. 
    }
    
    public IMoveable Put()
    {
        IMoveable tmp = MyMoveable;

        // 복사한 스킬 아이콘 정보를 null로 만든다.
        MyMoveable = null;

        // 복사된 아이콘을 투명하게 만든다.
        Icon.color = new Color(0, 0, 0, 0);
        
        // 복사한 스킬의 아이콘 정보를 전달한다.
        return tmp;
    }

    public void exitPut()
    {
        
        // 복사한 스킬 아이콘 정보를 null로 만든다.
        MyMoveable = null;

        // 복사된 아이콘을 투명하게 만든다.
        Icon.color = new Color(0, 0, 0, 0);
    }
}
