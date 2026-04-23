using UnityEngine;

/// <summary>
/// 通用拖拽纸条脚本
/// 挂在被拖拽的物体上即可
/// 功能：
/// 1. 鼠标拖拽物体
/// 2. 松手时检测当前位置下方的 Collider2D
/// 3. 如果检测到指定 Tag 的目标区域，则本物体消失
/// 4. 并在该区域内生成一个图片预制体
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class LetterDelete : MonoBehaviour
{
    [Header("拖拽设置")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float dragZDepth = 10f;

    [Header("目标检测")]
    [SerializeField] private string targetTag = "PaperTarget";

    [Header("生成设置")]
    [SerializeField] private GameObject spawnImagePrefab;
    [SerializeField] private bool randomSpawnInArea = true;
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;

    [Header("拖拽结束后如果未匹配")]
    [SerializeField] private bool returnToStartPosition = true;

    private Vector3 startPosition;
    private Vector3 mouseOffset;
    private bool isDragging = false;
    private Collider2D selfCollider;

    private void Awake()
    {
        selfCollider = GetComponent<Collider2D>();

        // 如果没有手动指定摄像机，就自动找主摄像机
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        startPosition = transform.position;
    }

    private void OnMouseDown()
    {
        if (mainCamera == null) return;

        isDragging = true;

        // 记录初始位置，方便拖拽失败后回去
        startPosition = transform.position;

        // 计算鼠标点击点和物体中心的偏移，避免拖拽时物体瞬移到鼠标中心
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        mouseOffset = transform.position - mouseWorldPos;
    }

    private void OnMouseDrag()
    {
        if (!isDragging || mainCamera == null) return;

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        transform.position = mouseWorldPos + mouseOffset;
    }

    private void OnMouseUp()
    {
        if (!isDragging) return;

        isDragging = false;

        // 检测当前拖拽物体位置下方所有碰撞体
        Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);

        Collider2D matchedTarget = null;

        for (int i = 0; i < hits.Length; i++)
        {
            // 排除自己
            if (hits[i] == selfCollider)
                continue;

            // 检测 Tag 是否匹配
            if (hits[i].CompareTag(targetTag))
            {
                matchedTarget = hits[i];
                break;
            }
        }

        // 如果找到匹配区域
        if (matchedTarget != null)
        {
            HandleSuccess(matchedTarget);
        }
        else
        {
            HandleFail();
        }
    }

    /// <summary>
    /// 获取鼠标对应的世界坐标
    /// </summary>
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;

        // 这里的 z 需要填摄像机到物体所在平面的距离
        float z = Mathf.Abs(mainCamera.transform.position.z) + dragZDepth;
        mousePos.z = z;

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = transform.position.z; // 保持物体原本的 z，不乱跑层级
        return worldPos;
    }

    /// <summary>
    /// 拖拽成功逻辑
    /// </summary>
    /// <param name="target">匹配到的目标区域</param>
    private void HandleSuccess(Collider2D target)
    {
        // 在目标区域生成图片
        if (spawnImagePrefab != null)
        {
            Vector3 spawnPos = GetSpawnPosition(target);
            Instantiate(spawnImagePrefab, spawnPos, Quaternion.identity);
        }

        // 让纸条消失
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 拖拽失败逻辑
    /// </summary>
    private void HandleFail()
    {
        if (returnToStartPosition)
        {
            transform.position = startPosition;
        }
    }

    /// <summary>
    /// 计算生成位置
    /// 可选择在区域中心生成，或者在区域范围内随机生成
    /// </summary>
    private Vector3 GetSpawnPosition(Collider2D target)
    {
        Bounds bounds = target.bounds;
        Vector3 result;

        if (randomSpawnInArea)
        {
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);
            result = new Vector3(randomX, randomY, transform.position.z);
        }
        else
        {
            result = new Vector3(bounds.center.x, bounds.center.y, transform.position.z);
        }

        result += spawnOffset;
        return result;
    }
}