using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoyStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] RectTransform rect_Background = null;
    [SerializeField] RectTransform rect_Joystick = null;
    [SerializeField] Image img_Background = null;
    [SerializeField] Image img_JoyStick = null;

    //PlayerControl thePlayer = null;

    private float radius;

    public bool isTouch = false;

    void Start()
    {
        radius = rect_Background.rect.width * 0.5f;
        StartCoroutine(FadeOut());
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 value = eventData.position - (Vector2)rect_Background.position;

        value = Vector2.ClampMagnitude(value, radius);

        rect_Joystick.localPosition = value;

        value = value.normalized;

      
        //PlayerControl.MyInstance.InputJoyStick(value);

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isTouch = true;
        //PlayerControl.MyInstance.isTouch = isTouch;
        StartCoroutine(FadeIn());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isTouch = false;
        rect_Joystick.localPosition = Vector3.zero;
       // PlayerControl.MyInstance.isTouch = isTouch;
       // PlayerControl.MyInstance.InputJoyStick(Vector2.zero);
        StartCoroutine(FadeOut());
    }

    public void PushAttackButton()
    {
       // PlayerControl.MyInstance.TryAttack();
    }

    public void PushDashButton()
    {
       // PlayerControl.MyInstance.isDash = true;
       // PlayerControl.MyInstance.DashMove();
    }

    IEnumerator FadeOut()
    {
        Color t_color = img_Background.color;
        Color t_joyColor = img_JoyStick.color;

        while (t_color.a > 0)
        {
            t_color.a -= 0.1f;
            t_joyColor.a -= 0.1f;
            img_Background.color = t_color;
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator FadeIn()
    {
        Color t_color = img_Background.color;
        Color t_joyColor = img_JoyStick.color;

        while (t_color.a < 1)
        {
            t_color.a += 0.1f;
            img_Background.color = t_color;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
