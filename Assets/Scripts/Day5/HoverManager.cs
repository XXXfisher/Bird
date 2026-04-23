using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class HoverManager : MonoBehaviour
{
    //public static HoverManager Instance;

    [Header("随机摆放")]
    public RectTransform panel;
    public List<RectTransform> items;

    [Header("统计设置")]
    public int targetCount = 4; // 目标碰撞次数
    private HashSet<int> touchedIDs = new HashSet<int>(); // 存储已碰撞物体的唯一ID
    private bool isTaskCompleted = false; // 防止重复触发Debug


    public int maxTry = 100;


    void Start()
    {
        CollectItems();
        RandomPlaceItems();
    }



    // ===== 随机摆放（不重叠）=====
    void RandomPlaceItems()
    {
        List<Rect> placedRects = new List<Rect>();

        foreach (RectTransform item in items)
        {
            bool placed = false;

            for (int i = 0; i < maxTry; i++)
            {
                Vector2 pos = GetRandomPos(item);

                Vector2 size = item.sizeDelta;
                Rect rect = new Rect(pos - size * 0.5f, size);

                bool overlap = false;

                foreach (Rect r in placedRects)
                {
                    if (r.Overlaps(rect))
                    {
                        overlap = true;
                        break;
                    }
                }

                if (!overlap)
                {
                    item.anchoredPosition = pos;
                    placedRects.Add(rect);
                    placed = true;
                    break;
                }
            }

            if (!placed)
            {
                Debug.LogWarning(item.name + " 放不下（区域太小或物体太多）");
            }
        }
    }

    Vector2 GetRandomPos(RectTransform item)
    {
        Vector2 size = panel.rect.size;
        Vector2 itemSize = item.sizeDelta;

        float x = Random.Range(-size.x / 2 + itemSize.x / 2,
                                size.x / 2 - itemSize.x / 2);

        float y = Random.Range(-size.y / 2 + itemSize.y / 2,
                                size.y / 2 - itemSize.y / 2);

        return new Vector2(x, y);
    }

    void CollectItems()
    {
        items = new List<RectTransform>();

        foreach (Transform child in panel)
        {
            RectTransform rt = child as RectTransform;
            if (rt != null)
            {
                items.Add(rt);
            }
        }
    }

    public void ReportTouch(int id)
    {
        if (isTaskCompleted) return;

        // 如果这个 ID 还没被记录过
        if (!touchedIDs.Contains(id))
        {
            touchedIDs.Add(id);
            Debug.Log($"<color=cyan>进度更新：</color> 碰到了新 Collider (ID: {id})，当前进度: {touchedIDs.Count}/{targetCount}");

            if (touchedIDs.Count >= targetCount)
            {
                isTaskCompleted = true;
                OnAllTargetsTouched();
            }
        }
    }

    private void OnAllTargetsTouched()
    {
        Debug.Log("<color=green>任务完成！</color>");

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelComplete();
        }
    }
}