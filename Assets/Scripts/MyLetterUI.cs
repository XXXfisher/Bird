using UnityEngine;
using UnityEngine.UI;

public class MyLetterUI : MonoBehaviour
{
    private GameObject myLetterPrefab; // 存一下这封信对应的预制体
    private LevelData currentData;
    private LetterManager letterManager;
    public Dialog dialog; // 需要在Inspector里关联这个组件




    public void Init(LetterButtonInfo info, LevelData data, LetterManager manager)
    {
        // 1. 设置按钮的视觉效果（图片）
        if (info.buttonSprite != null)
        {
            GetComponent<UnityEngine.UI.Image>().sprite = info.buttonSprite;
        }

        // 2. 存储数据，供点击时使用
        this.myLetterPrefab = info.prefabToSpawn;
        this.currentData = data;
        this.letterManager = manager;
    }

    public void OnClick()
    {
        letterManager.SpawnLetter(myLetterPrefab);

        if (currentData.isDialogNeeded)
        {
            // 调用对话系统显示对话框
            if (dialog != null)
            {
                dialog.UpdateDialogData(currentData.dialogData);
                dialog.ShowDialog();
            }
        }
    }
}
