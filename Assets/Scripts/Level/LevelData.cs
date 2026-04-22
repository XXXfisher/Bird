/*
 * 用于在同一个场景中储存不同天数时的关卡数据
 * 
 * 切换天数时，加载对应的LevelData，更新数据:UI Button的Letter预制体，UI Button的Sprite，背景图片
 */

using UnityEngine;

[CreateAssetMenu(fileName = "NewDay", menuName = "Day/LevelData")]
public class LevelData : ScriptableObject
{
    public Sprite backgroundSprite; 
    //public GameObject[] letterPrefabs;
    // 其他关卡相关数据...待补充
}