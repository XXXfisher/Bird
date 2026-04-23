using UnityEngine;
using System.Collections.Generic; // 使用 List 需要引入

[System.Serializable]
public class LetterButtonInfo
{
    public string letterName;      // 按钮的名字（可选，方便调试）
    public Sprite buttonSprite;    // 这个按钮特有的图片（比如不同样式的信封）
    public GameObject prefabToSpawn; // 点击这个按钮要生成的信件预制体
}


public enum LevelType { Normal, Hover, PutDown }

[CreateAssetMenu(fileName = "NewDay", menuName = "Day/LevelData")]
public class LevelData : ScriptableObject
{
    [Header("这一天所有的信件按钮")]
    public List<LetterButtonInfo> letterButtons;

    [Header("通用配置")]
    public LevelType levelType; // 这一天的类型（普通、悬停、写信）
    public DialogData_SO dialogData;
    public bool isDialogNeeded;
    public Sprite backgroundImage; // 这一天的背景图

    [Header("正确序列")]
    public int[] targetSequence;

    [Header("第五天")]
    public bool showHoverPanel;

    [Header("第六天")]
    public bool showDay6Input;
}