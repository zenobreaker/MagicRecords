using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Jostick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] RectTransform rect_Background = null;
    [SerializeField] RectTransform rect_Joystick = null;


    
    private float radius;

    public bool isTouch = false;

    void Start()
    {
        radius = rect_Background.rect.width * 0.5f;
    }

   
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 value = eventData.position - (Vector2)rect_Background.position;
      

        value = Vector2.ClampMagnitude(value, radius);

        rect_Joystick.localPosition = value;

        value = value.normalized;


    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isTouch = true;
        
      
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isTouch = false;
        rect_Joystick.localPosition = Vector3.zero;
     
        //PlayerControl.MyInstance.InputJoyStick(Vector2.zero);
    }

    public void PushAttackButton()
    {
       

        //PlayerControl.MyInstance.TryAttack();
    }

    // 특수 버튼을 누른다. 특수버튼은 대쉬나 방어 등의 기술을 사용한다. 
    public void PushSpecialButton()
    {
        //todo 현재 기능으론 대쉬만 한다. 
       
    }
}
