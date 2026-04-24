using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class DeskManager : MonoBehaviour
{
    public static DeskManager Instance;

    [Header("配置与引用")]
    public Image backgroundUI;
    public Transform spawnPoint;

    [Header("鸟动画")]
    public GameObject BirdAnim;
    public AudioClip birdClip;

    [Header("按钮与对话系统")]
    public Button nextButton;
    public Dialog dialogSystem;
    public DialogData_SO day1Dialog, day2Dialog, day3Dialog, day4Dialog;

    [Header("桌面的信件预制体")]
    public GameObject D1, D2, D3, D4;

    private GameObject currentSpawnedDesk;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

    }

    private void Start()
    {
        RefreshLevelElements();
        StartCoroutine(BirdAndLetter());
    }

    public void RefreshLevelElements()
    {
        if (LevelManager.Instance == null || LevelManager.Instance.currentLevelData == null) return;

        LevelData data = LevelManager.Instance.currentLevelData;

        // 1. 设置背景
        if (backgroundUI != null && data.backgroundImage != null)
        {
            backgroundUI.sprite = data.backgroundImage;
        }

    }

    private void SpawnDeskByData(LevelData data)
    {
        // 销毁旧物体
        if (currentSpawnedDesk != null) Destroy(currentSpawnedDesk);

        GameObject prefabToSpawn = null;

        string dataName = data.name;

        if (dataName.Contains("Day1")) prefabToSpawn = D1;
        else if (dataName.Contains("Day2")) prefabToSpawn = D2;
        else if (dataName.Contains("Day3")) prefabToSpawn = D3;
        else if (dataName.Contains("Day4")) prefabToSpawn = D4;


        if (prefabToSpawn != null)
        {
            currentSpawnedDesk = Instantiate(prefabToSpawn, spawnPoint != null ? spawnPoint.position : transform.position, Quaternion.identity);
            currentSpawnedDesk.transform.SetParent(this.transform);
        }
    }

    IEnumerator BirdAndLetter()
    {
        LevelData data = LevelManager.Instance.currentLevelData;

        string dataName = data.name;

        yield return new WaitForSeconds(2f);
        if (BirdAnim != null)
        {
            BirdAnim.SetActive(true);
            AudioManager.Instance.audioSource.PlayOneShot(birdClip);
            yield return new WaitForSeconds(1f);
            //BirdAnim.SetActive(false);
        }
        yield return new WaitForSeconds(2f);
        SpawnDeskByData(data);

        yield return new WaitForSeconds(1f);

        DialogData_SO targetDialog = null;
        if (dataName.Contains("Day1")) targetDialog = day1Dialog;
        else if (dataName.Contains("Day2")) targetDialog = day2Dialog;
        else if (dataName.Contains("Day3")) targetDialog = day3Dialog;
        else if (dataName.Contains("Day4")) targetDialog = day4Dialog;

        if (targetDialog != null && dialogSystem != null)
        {
            dialogSystem.dialogPanel.SetActive(true);
            dialogSystem.UpdateDialogData(targetDialog);
            dialogSystem.ShowDialog();

            // 只要 dialogPanel 还是激活状态，协程就会在这里“卡住”
            //while (dialogSystem.dialogPanel.activeInHierarchy)
            //{
            //    yield return null;
            //}

            // 显示下一步按钮
            if (nextButton != null)
            {
                nextButton.gameObject.SetActive(true);
            }
        }
    }



}