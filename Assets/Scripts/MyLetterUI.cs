using UnityEngine;
using UnityEngine.UI;

public class MyLetterUI : MonoBehaviour
{
    private GameObject myLetterPrefab; // 存一下这封信对应的预制体
    private LevelData currentData;
    private LetterManager letterManager;
    public Dialog dialog; // 需要在Inspector里关联这个组件
    private LetterButtonInfo myInfo;

    public AudioClip clickSound; // 点击按钮时的音效

    [Header("点击限制")]
    public int maxClickCount = 1; // 最多允许点击几次
    private int currentClickCount = 0;

    private Button button;

    public void Init(LetterButtonInfo info, LevelData data, LetterManager manager)
    {
        button = GetComponent<Button>();

        // 1. 设置按钮的视觉效果（图片）
        if (info.buttonSprite != null)
        {
            GetComponent<UnityEngine.UI.Image>().sprite = info.buttonSprite;
        }

        if (this.dialog == null)
        {
            this.dialog = Object.FindAnyObjectByType<Dialog>();
        }

        // 2. 存储数据，供点击时使用
        this.myLetterPrefab = info.prefabToSpawn;
        this.currentData = data;
        this.letterManager = manager;
        this.myInfo = info;
    }

    public void OnClick()
    {
        if (currentClickCount >= maxClickCount)
        {
            return;
        }
        currentClickCount++;

        if (clickSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.audioSource.PlayOneShot(clickSound);
        }

        letterManager.SpawnLetter(myLetterPrefab);

        if (myInfo != null && myInfo.openingDialog != null)
        {
            if (dialog != null)
            {
                dialog.UpdateDialogData(myInfo.openingDialog);
                dialog.ShowDialog();
            }
            else             {
                Debug.LogWarning($"[MyLetterUI] Dialog component not assigned for {myInfo.letterName}");
            }
        }

        if (currentClickCount >= maxClickCount && button != null)
        {
            button.interactable = false;
        }

    }
}
