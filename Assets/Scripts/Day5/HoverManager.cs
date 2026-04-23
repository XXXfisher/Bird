using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class HoverManager : MonoBehaviour
{
    public static HoverManager Instance;

    [Header("文字UI")]
    public GameObject textObj;
    public TextMeshProUGUI textUI;

    [Header("随机摆放")]
    public RectTransform panel;
    public List<RectTransform> items;

    public int maxTry = 100;

    void Awake()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.hoverPanel = this.gameObject;

            // 顺便根据当前的关卡数据决定初始显隐状态
            bool shouldShow = LevelManager.Instance.currentLevelData.showHoverPanel;
            this.gameObject.SetActive(shouldShow);
        }
        Instance = this;
        textObj.SetActive(false);

    }

    void Start()
    {
        CollectItems();
        RandomPlaceItems();
    }

    void Update()
    {
        // 让文字跟随鼠标（可选但推荐）
        if (textObj.activeSelf)
        {
            textObj.transform.position = Input.mousePosition;
        }
    }

    // ===== Hover 控制 =====
    public void Show(string msg)
    {
        textUI.text = msg;
        textObj.SetActive(true);
    }

    public void Hide()
    {
        textObj.SetActive(false);
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
}