using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;
using static UnityEngine.Rendering.DebugUI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("引用设置")]
    public LetterManager letterManager; // 拖入场景中的 LetterManager
    public GridManager gridManager; // 拖入场景中的 GridManager
    public GameObject canvas;
    public Dialog dialog;
    //public LoadNextScene sceneLoader;

    [Header("场景名称配置")]
    public string sceneA = "Level_Letter";
    public string sceneB = "Level_Desk";

    [Header("前四天机制")]
    public GameObject buttonPrefab;    // 拖入你制作的按钮预制体
    public Transform buttonContainer;

    [Header("第五天机制")]
    public GameObject hoverPanel;

    [Header("第六天机制")]
    public GameObject InputCanvas;

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
        // 无论在哪个场景，只要有关卡数据，就执行一次基础的状态重置（例如显隐控制）
        if (currentLevelData != null)
        {
            // 显式根据数据控制显隐，防止跨场景残留

        }

        if (scene.name == sceneB)
        {
            if (canvas != null) canvas.SetActive(false);
            if (hoverPanel != null) hoverPanel.SetActive(false);
            if (InputCanvas != null) InputCanvas.SetActive(false);

        }

        if (scene.name == sceneA)
        {
            if (canvas != null) canvas.SetActive(true); // 确保切回来时 Canvas 是开的
            if (hoverPanel != null) hoverPanel.SetActive(currentLevelData.showHoverPanel);
            if (InputCanvas != null) InputCanvas.SetActive(currentLevelData.showDay6Input);
            RefreshReferences();
            LoadLevel(currentLevelData);
        }
    }

    void RefreshReferences()
    {
        GameObject containerObj = GameObject.FindWithTag("ButtonContainer");
        if (containerObj != null) buttonContainer = containerObj.transform;

        letterManager = FindFirstObjectByType<LetterManager>();

        //GameObject p = GameObject.FindWithTag("Day5");
        //if (p != null) hoverPanel = p;
        if (hoverPanel == null)
        {
            Transform t = transform.Find("HoverPanel"); // 确保名字和 Hierarchy 里一致
            if (t != null) hoverPanel = t.gameObject;
        }

        if (this.dialog == null)
        {
            this.dialog = FindAnyObjectByType<Dialog>();
        }
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

        //========== 第六天机制：设置输入界面 ========
        if (InputCanvas != null)
        {
            InputCanvas.SetActive(data.showDay6Input);

            //if (data.showDay6Input)
            //{
            //    // 如果你写了 Day6InputManager，可以在这里调用它的初始化方法
            //    // InputCanvas.GetComponent<Day6InputManager>().ResetUI();
            //}
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
        StartCoroutine(CompleteLevelRoutine());
    }

    IEnumerator CompleteLevelRoutine()
    {
        Debug.Log("关卡完成！准备播放结束对话。");

        // 1. 显示结束对话
        if (dialog != null && currentLevelData.endlevelDialogData != null)
        {
            dialog.UpdateDialogData(currentLevelData.endlevelDialogData);
            dialog.ShowDialog();

            // 等待对话框关闭
            while (dialog.dialogPanel.activeInHierarchy)
            {
                yield return null; 
            }
        }
        else
        {
            Debug.LogWarning("没有结束对话，直接进入下一关");
        }

        currentLevelIndex++;

        if (currentLevelIndex < allLevels.Count)
        {
            currentLevelData = allLevels[currentLevelIndex];

            yield return new WaitForSeconds(0.5f);

            SwitchToNextScene();
            Debug.Log("加载下一天数据完成");
        }
        else
        {
            Debug.Log("所有关卡已完成！");
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