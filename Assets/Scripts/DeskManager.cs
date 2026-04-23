using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DeskManager : MonoBehaviour
{
    public static DeskManager Instance;

    [Header("配置与引用")]
    public Image backgroundUI;
    public Transform spawnPoint; // 生成物体的父级或位置

    [Header("所有的桌子预制体库")]
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

        // 2. 根据 LevelData 的内容生成桌子
        SpawnDeskByData(data);
    }

    private void SpawnDeskByData(LevelData data)
    {
        // 销毁旧物体
        if (currentSpawnedDesk != null) Destroy(currentSpawnedDesk);

        GameObject prefabToSpawn = null;

        // 方法 A：根据 LevelData 的名字判断（如果你的 Asset 名字叫 "Day1", "Day2"）
        string dataName = data.name;
        if (dataName.Contains("Day1")) prefabToSpawn = D1;
        else if (dataName.Contains("Day2")) prefabToSpawn = D2;
        else if (dataName.Contains("Day3")) prefabToSpawn = D3;
        else if (dataName.Contains("Day4")) prefabToSpawn = D4;

        // 方法 B (更推荐)：在 LevelData 中直接添加一个字段（见下方补充）

        if (prefabToSpawn != null)
        {
            currentSpawnedDesk = Instantiate(prefabToSpawn, spawnPoint != null ? spawnPoint.position : transform.position, Quaternion.identity);
            currentSpawnedDesk.transform.SetParent(this.transform);
        }
    }
}