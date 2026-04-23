using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("引用设置")]
    public LetterManager letterManager; // 拖入场景中的 LetterManager
    public GridManager gridManager; // 拖入场景中的 GridManager
    public GameObject canvas;
    //public LoadNextScene sceneLoader;

    [Header("场景名称配置")]
    public string sceneA = "Level_Letter";
    public string sceneB = "Level_Desk";

    [Header("前四天机制")]
    public GameObject buttonPrefab;    // 拖入你制作的按钮预制体
    public Transform buttonContainer;

    [Header("第五天机制")]
    public GameObject hoverPanel; 

    [Header("关卡数据")]
    public List<LevelData> allLevels;
    public LevelData currentLevelData; // 可以在 Inspector 拖入，也可以代码加载
    private int currentLevelIndex = 0;

    void Awake()
    {
        // 单例模式
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if (scene.name == sceneB)
        {
            if (canvas != null) canvas.SetActive(false);
            //return;
        }

        // 如果当前是 Level_Letter 场景，我们需要重新找回 UI 引用
        if (scene.name == sceneA)
        {
            RefreshReferences();
            LoadLevel(currentLevelData);
        }

    }

    void RefreshReferences()
    {
        GameObject containerObj = GameObject.FindWithTag("ButtonContainer");
        if (containerObj != null) buttonContainer = containerObj.transform;

        letterManager = FindFirstObjectByType<LetterManager>();

        //GameObject p = GameObject.FindWithTag("HoverPanel"); 
        //if (p != null) hoverPanel = p;
    }


    void Start()
    {
        RefreshReferences();
        LoadLevelByIndex(currentLevelIndex);
    }


    /*
     * 根据当前关卡索引加载关卡
     */
    void LoadLevelByIndex(int index)
    {
        if (allLevels.Count > index)
        {
            currentLevelData = allLevels[index];
            LoadLevel(currentLevelData);
        }
    }


    /*
     * 加载指定关卡数据
     */
    void LoadLevel(LevelData data)
    {
        //========== 第五天机制：设置背景图和悬浮面板 ========
        if (hoverPanel != null)
        {
            hoverPanel.SetActive(data.showHoverPanel);
        }

        //========== 前四天机制：生成按钮并传递谜底给 GridManager ========
        if (buttonContainer == null) return;

        Debug.Log($"当前是 {data.name}");

        // 将关卡的谜底传递给 GridManager
        GridManager grid = FindFirstObjectByType<GridManager>();

        if (grid != null)
        {
            grid.SetLevelData(data);
            Debug.Log($"{data.name} 的谜底是 {string.Join(", ", data.targetSequence)}");
            grid.hasCompleted = false;
        }   

        // 清空旧按钮
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // 遍历 letterButtons 列表
        foreach (LetterButtonInfo info in data.letterButtons)
        {
            // 生成按钮
            GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);

            // 获取按钮脚本
            MyLetterUI btnScript = btnObj.GetComponent<MyLetterUI>();

            if (btnScript != null)
            {
                btnScript.Init(info, data, this.letterManager);
            }
        }
    }

    /*
     * 通关逻辑
     */
    public void OnLevelComplete()
    {
        Debug.Log("关卡完成！当前 index = " + currentLevelIndex);

        // 这里可以添加过渡动画

        // 加载下一关
        currentLevelIndex++;

        Debug.Log("增加后 index = " + currentLevelIndex);

        if (currentLevelIndex < allLevels.Count)
        {
            currentLevelData = allLevels[currentLevelIndex];

            SwitchToNextScene();

            Debug.Log("准备加载第 " + (currentLevelIndex + 1) + " 天");
        }
    }

    public void SwitchToNextScene()
    {
        // 获取当前活跃场景的名字
        string currentSceneName = SceneManager.GetActiveScene().name;
        string targetScene;

        if (currentSceneName == sceneA)
        {
            targetScene = sceneB;
        }
        else
        {
            targetScene = sceneA;
        }

        SceneManager.LoadScene(targetScene);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}