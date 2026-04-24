/*
 * 控制对话系统UI显示和对话SO数据的更新。
 * 
 * 设置多个dialog data，包含对话对象、头像和文本。
 * 
 * 在需要显示对话时，调用ShowDialog方法。
 */

using UnityEngine;
using UnityEngine.UI;


public class Dialog : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject dialogPanel;
    public Button nextButton;
    public TMPro.TextMeshProUGUI mainText;

    [Header("头像 UI")]
    public Image portraitImage; // 对话头像

    [Header("Data")]
    public DialogData_SO currentData;
    int currentLineIndex = 0;

   
    private void Start()
    {
        HideDialog();
    }

    //控制对话
    public void ShowDialog()
    {
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(true); // 显示对话框
        }

        if (currentData == null || currentData.dialogPieces == null || currentData.dialogPieces.Count == 0)
        {
            // 没有数据则直接隐藏
            HideDialog();
            return;
        }

        UpdateDialogData(currentData);
        Debug.Log($"[Dialog] ShowDialog -> CurrentIndex: {currentLineIndex}, TotalPiece: {currentData.dialogPieces.Count}");
        
        UpdateMainDialog(currentData.dialogPieces[currentLineIndex]);
    }

    public void HideDialog()
    {
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false); // 隐藏对话框
        }
    }

    //根据传入的对话信息进行更新的函数
    public void UpdateDialogData(DialogData_SO data)
    {
        currentData = data;
        currentLineIndex = 0;
    }

    public void UpdateMainDialog(DialogPiece piece)
    {
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(true);
        }

        mainText.text = piece != null ? piece.text : "";

        // 更新头像
        if (portraitImage != null)
        {
            if (piece != null && piece.image != null)
            {
                portraitImage.sprite = piece.image;
                portraitImage.gameObject.SetActive(true);
            }
            else
            {
                portraitImage.gameObject.SetActive(false);
            }
        }


        // 如果还有下一句就显示 next，否则隐藏
        //bool hasNext = currentData != null &&
        //               currentData.dialogPieces != null &&
        //               currentLineIndex < currentData.dialogPieces.Count - 1;
        bool hasNext = currentData != null &&
               currentData.dialogPieces != null &&
               currentLineIndex < currentData.dialogPieces.Count;

        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(hasNext);
        }
    }

    // 快进下一句
    public void ContinueNextDialog()
    {
        Debug.Log("[Dialog] continue invoked");
        if (currentData == null || currentData.dialogPieces == null)
        {
            HideDialog();
            return;
        }

        if (currentLineIndex < currentData.dialogPieces.Count - 1)
        {
            currentLineIndex++;
            // 传入 DialogPiece 给显示方法
            UpdateMainDialog(currentData.dialogPieces[currentLineIndex]);
        }
        else
        {
            HideDialog();
        }
    }

}