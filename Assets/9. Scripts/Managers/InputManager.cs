using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Button[] btns = null;

    // Update is called once per frame
    void Update()
    {
        keyInput();
    }

    private void keyInput()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("타임스케일 " + Time.timeScale);
        }
    }
}
