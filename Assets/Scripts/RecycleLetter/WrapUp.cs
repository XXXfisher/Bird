using UnityEngine;
using UnityEngine.UI; // 使用 UI Image 制作进度条

public class WrapUp : MonoBehaviour
{
    [Header("设置")]
    //public GameObject wrapUpLetter; // 最终生成的物体预制体
    public float holdDuration = 1.5f; // 需要持续按下的时间

    [Header("UI 反馈")]
    public Image progressCircle;

    private float currentHoldTime = 0f;
    private bool isDone = false;

    void Start()
    {
        // 初始隐藏进度条
        if (progressCircle != null)
        {
            progressCircle.fillAmount = 0;
            progressCircle.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isDone) return;

        if (Input.GetMouseButton(0))// 左键
        {
            UpdateProgress();
        }
        else
        {
            ResetProgress();
        }
    }

    private void UpdateProgress()
    {
        currentHoldTime += Time.deltaTime;

        // 显示并更新进度条
        if (progressCircle != null)
        {
            progressCircle.gameObject.SetActive(true);
            progressCircle.fillAmount = Mathf.Clamp01(currentHoldTime / holdDuration);
        }

        // 达到目标时间
        if (currentHoldTime >= holdDuration)
        {
            ExecuteWrapUp();
        }
    }

    private void ResetProgress()
    {
        if (currentHoldTime > 0)
        {
            currentHoldTime = 0f;
            if (progressCircle != null)
            {
                progressCircle.fillAmount = 0;
                progressCircle.gameObject.SetActive(false);
            }
        }
    }

    private void ExecuteWrapUp()
    {
        isDone = true;

        Destroy(this.gameObject);
    }

}