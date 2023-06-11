using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Jostick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] RectTransform rect_Background = null;
    [SerializeField] RectTransform rect_Joystick = null;


    //PlayerControl thePlayer = null;

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

 
     
       // PlayerControl.MyInstance.InputJoyStick(value);
     
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isTouch = true;
        // PlayerControl.MyInstance.isTouch = isTouch;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isTouch = false;
        rect_Joystick.localPosition = Vector3.zero;
        //PlayerControl.MyInstance.isTouch = isTouch;
        //PlayerControl.MyInstance.InputJoyStick(Vector2.zero);
    }

    public void PushAttackButton()
    {
        //PlayerControl.MyInstance.TryAttack();
    }
}
