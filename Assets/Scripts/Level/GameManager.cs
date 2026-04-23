using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("引用设置")]
    public LetterManager letterManager; // 拖入场景中的 LetterManager
    public GameObject buttonPrefab;    // 拖入你制作的按钮预制体
    public Transform buttonContainer; // 拖入 UI 里的 Content 节点 (带 LayoutGroup 的那个)

    [Header("当前关卡数据")]
    public LevelData currentLevelData; // 可以在 Inspector 拖入，也可以代码加载

    void Start()
    {
        // 游戏开始或切换天数时调用
        if (currentLevelData != null)
        {
            LoadLevel(currentLevelData);
        }
    }

    void LoadLevel(LevelData data)
    {
        // 1. 清空旧按钮
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. 修正后的循环：遍历的是 letterButtons 列表，而不是 letterPrefabs 数组
        foreach (LetterButtonInfo info in data.letterButtons)
        {
            // 生成按钮
            GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);

            // 获取按钮脚本
            MyLetterUI btnScript = btnObj.GetComponent<MyLetterUI>();

            if (btnScript != null)
            {
                // 注意：现在传递的是整个 info 对象，包含了图片和预制体
                btnScript.Init(info, data, this.letterManager);
            }
        }
    }
}