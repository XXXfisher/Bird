using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class HoverItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    public string content; // 在 Inspector 面板里为每个 Image 输入不同的文字
    public TextMeshProUGUI textUI;
    public HoverManager manager;
    public Vector2 yOffset = new Vector2(0, 50);

    public int itemID;
    [Header("淡出设置")]
    public float fadeDuration = 2f;

    private RectTransform rectTransform;
    private Coroutine fadeCoroutine;

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

        // 停止正在进行的淡出
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        textUI.text = msg;
        textUI.gameObject.SetActive(true);

        // 重置透明度
        Color c = textUI.color;
        c.a = 1f;
        textUI.color = c;

        textUI.rectTransform.anchoredPosition = target.anchoredPosition + yOffset;
    }

    public void Hide()
    {
        if (textUI == null) return;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(2f);

        float timer = 0f;

        Color startColor = textUI.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);

            Color c = startColor;
            c.a = alpha;
            textUI.color = c;

            yield return null;
        }

        textUI.gameObject.SetActive(false);
    }
}