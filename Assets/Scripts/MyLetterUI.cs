using UnityEngine;

public class MyLetterUI : MonoBehaviour
{
    public GameObject letterPrefab;

    public LetterManager letterManager;

    public Dialog dialog;

    public bool isDialogNeeded;

    public DialogData_SO dialogData;

    public void OnLetterUIButtonClicked()
    {
        letterManager.SpawnLetter(letterPrefab);

        if (isDialogNeeded)
        {
            // 调用对话系统显示对话框
            if (dialog != null)
            {
                dialog.UpdateDialogData(dialogData);
                dialog.ShowDialog();
            }
        }
    }
}
