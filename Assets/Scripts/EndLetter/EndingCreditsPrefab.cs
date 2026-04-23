using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndingCreditsPrefab : MonoBehaviour
{
    [Header("引用")]
    [SerializeField] private RectTransform creditsTextRect;
    [SerializeField] private TMP_Text creditsText;
    [SerializeField] private TMP_Text anyKeyText;

    [Header("滚动参数")]
    [SerializeField] private float scrollSpeed = 28f;
    [SerializeField] private float startY = -1200f;     // 从屏幕底部外开始
    [SerializeField] private float endOffset = 80f;     // 完全滚出屏幕后额外再滚一点
    [SerializeField] private float hintDelay = 0.3f;    // 滚完后多久显示提示

    [Header("场景")]
    [SerializeField] private string titleSceneName = "Title";

    [Header("玩家输入样式")]
    [SerializeField] private string playerInputColor = "#FFD36B";
    [SerializeField] private int playerInputScale = 145;

    [Header("文本模板")]
    [TextArea(10, 40)]
    [SerializeField] private string endingTemplate;

    private bool isScrolling = false;
    private bool canReturn = false;

    private float endY;
    private float timer;

    private void OnEnable()
    {
        ResetState();
    }

    public void SetPlayerInput(string playerInput)
    {
        if (creditsText == null || creditsTextRect == null)
        {
            Debug.LogWarning("EndingCreditsPrefab 缺少文本引用！");
            return;
        }

        if (string.IsNullOrWhiteSpace(playerInput))
        {
            playerInput = "...";
        }

        // 玩家输入：加粗 + 放大 + 变色
        string styledInput =
            $"<size={playerInputScale}%><b><color={playerInputColor}>{playerInput}</color></b></size>";

        string finalText = endingTemplate.Replace("[Player Input]", styledInput);
        creditsText.text = finalText;

        // 强制刷新 TMP 排版
        Canvas.ForceUpdateCanvases();
        creditsText.ForceMeshUpdate();

        // 取“真实文字高度”
        float realTextHeight = creditsText.preferredHeight;

        // 把文本框高度缩成真实高度，避免后半段滚空白
        creditsTextRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, realTextHeight);

        // 再刷新一次，确保尺寸更新完成
        Canvas.ForceUpdateCanvases();
        creditsText.ForceMeshUpdate();

        // 起始位置：屏幕下方外
        creditsTextRect.anchoredPosition = new Vector2(0f, startY);

        // 结束位置：整段文字完全滚出 1080 高度屏幕顶部
        endY = 1080f + (realTextHeight * 0.5f) + endOffset;

        isScrolling = true;
        canReturn = false;
        timer = 0f;

        if (anyKeyText != null)
        {
            anyKeyText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isScrolling)
        {
            ScrollCredits();
        }
        else
        {
            timer += Time.deltaTime;

            if (!canReturn && timer >= hintDelay)
            {
                canReturn = true;

                if (anyKeyText != null)
                {
                    anyKeyText.gameObject.SetActive(true);
                }
            }

            if (canReturn && Input.anyKeyDown)
            {
                SceneManager.LoadScene(titleSceneName);
            }
        }
    }

    private void ScrollCredits()
    {
        Vector2 pos = creditsTextRect.anchoredPosition;
        pos.y += scrollSpeed * Time.deltaTime;
        creditsTextRect.anchoredPosition = pos;

        if (pos.y >= endY)
        {
            isScrolling = false;
            timer = 0f;
        }
    }

    private void ResetState()
    {
        isScrolling = false;
        canReturn = false;
        timer = 0f;

        if (anyKeyText != null)
        {
            anyKeyText.gameObject.SetActive(false);
        }
    }
}