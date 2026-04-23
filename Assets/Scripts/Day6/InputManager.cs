using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class InputManager : MonoBehaviour
{
    [Header("UI 引用")]
    public GameObject poster;    
    public GameObject inputGroup;   
    public TMP_InputField inputField; 
    public Button confirmButton;

    [Header("海报显示时间")]
    public float waitTime = 3f;


    void Start()
    {
        // 初始状态显示海报，隐藏输入框
        if (poster != null) poster.SetActive(true);
        if (inputGroup != null) inputGroup.SetActive(false);

        // 绑定按钮点击事件
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(HandleCompleteInput);
        }

        StartCoroutine(ShowInputAfterDelay()); // 3秒后显示输入框
    }

    public void ShowInputField()
    {
        if (poster != null) poster.SetActive(false);
        if (inputGroup != null) inputGroup.SetActive(true);

        inputField.ActivateInputField();
    }

    IEnumerator ShowInputAfterDelay()
    {
        yield return new WaitForSeconds(waitTime);
        ShowInputField();
    }


    // 按钮点击事件处理
    public void HandleCompleteInput()
    {
        string result = inputField.text;

        if (string.IsNullOrEmpty(result))
        {
            Debug.Log("输入内容为空！");
            return;
        }

        Debug.Log("玩家输入完成: " + result);

        // 结束逻辑？
    }
}