using UnityEngine;
using UnityEngine.UI; // 使用 UI Image 制作进度条

public class WrapUp : MonoBehaviour
{
    [Header("设置")]
    public GameObject wrapUpLetter; // 最终生成的物体预制体
    public float holdDuration = 1.5f; // 需要持续按下的时间

    [Header("Spawn Reference")]
    public Collider2D spawnRegion;      
    public float checkRadius = 0.5f;    // 防止纸条重叠的检测半径
    public int maxAttempts = 15;  

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

        Vector3 spawnPos = GetRandomPointInCollider(spawnRegion);

        // 执行生成和销毁
        if (wrapUpLetter != null)
        {
            Instantiate(wrapUpLetter, spawnPos, Quaternion.identity);
        }
        Destroy(this.gameObject);
    }

    private Vector3 GetRandomPointInCollider(Collider2D collider)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomPoint = new Vector2(
                Random.Range(collider.bounds.min.x, collider.bounds.max.x),
                Random.Range(collider.bounds.min.y, collider.bounds.max.y)
            );
            // 检测该点附近是否有其他物体
            if (!Physics2D.OverlapCircle(randomPoint, checkRadius))
            {
                return randomPoint;
            }
        }
        // 如果多次尝试后仍未找到合适位置，返回中心点作为备用
        return collider.bounds.center;
    }

    // wrapup预制体在游戏全局中不被销毁
}