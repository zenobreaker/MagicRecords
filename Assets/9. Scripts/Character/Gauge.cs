using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gauge : MonoBehaviour
{
    private Image content;

    public Image afterHpbar;


    [SerializeField]
    private Text statText = null;

    [SerializeField]
    private float lerpSpeed = 0f;

    private float currentFill;
    public float MyMaxValue { get; set; }
    private float currentValue;

    public float fillAmo;

    public bool backHpHit = false;

    public float MyCurrentValue
    {
        get
        {
            return currentValue;
        }
        set
        {
            if (value > MyMaxValue) currentValue = MyMaxValue;
            else if (value < 0) currentValue = 0;
            else currentValue = value;

            currentFill = currentValue / MyMaxValue;
            if(statText != null)
                statText.text = currentValue + "/" + MyMaxValue;
        }
    }

    
    // Start is called before the first frame update
    void Start()
    {
        content = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentFill != content.fillAmount)
        {
            //content.fillAmount = Mathf.Lerp(content.fillAmount, currentFill, Time.deltaTime * lerpSpeed);
            content.fillAmount = currentFill;
            fillAmo = content.fillAmount;
        }
        Invoke("AfterHpDown", 0.5f);
    }

    void AfterHpDown()
    {
        if (afterHpbar != null)
        {
            afterHpbar.fillAmount = Mathf.Lerp(afterHpbar.fillAmount, currentFill, Time.deltaTime * lerpSpeed);
        }
    }

    public void Initalize(float value)
    {
        MyMaxValue = value;
        MyCurrentValue = value;
    }

    public void Initalize(float currentValue, float maxValue)
    {
        MyMaxValue = maxValue;
        MyCurrentValue = currentValue;
    }
}
