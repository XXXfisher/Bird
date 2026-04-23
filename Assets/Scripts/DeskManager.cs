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
    public GameObject BirdAnim;

    [Header("所有的桌子预制体库")]
    public GameObject D1, D2, D3, D4;

    private GameObject currentSpawnedDesk;

    void Awake()
    {
        //BirdAnim.SetActive(false);
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

        yield return new WaitForSeconds(2f);
        if (BirdAnim != null)
        {
            BirdAnim.SetActive(true);
            yield return new WaitForSeconds(1f); 
            //BirdAnim.SetActive(false);
        }
        yield return new WaitForSeconds(2f); 
        SpawnDeskByData(data);
    }

}