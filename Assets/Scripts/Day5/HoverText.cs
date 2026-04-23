using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    public string content; // 在 Inspector 面板里为每个 Image 输入不同的文字
    public TextMeshProUGUI textUI;
    public HoverManager manager;
    public Vector2 yOffset = new Vector2(0, 50);

    public int itemID;


    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        Show(content, rectTransform);
        if (manager != null)
        {
            manager.ReportTouch(itemID);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Hide();
    }

    public void Show(string msg, RectTransform target)
    {
        if (textUI == null || target == null) return;

        textUI.text = msg;
        textUI.gameObject.SetActive(true);

        // Text 和 Image 在同一个父级（或同等级 Canvas）下
        textUI.rectTransform.anchoredPosition = target.anchoredPosition + yOffset;
    }

    public void Hide()
    {
        if (textUI != null) textUI.gameObject.SetActive(false);
    }
}