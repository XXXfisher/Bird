/*
 * 负责管理当前的Letter实例，连接UI Button。
 * 
 * MyLetterUI.cs中点击UI Button后调用SpawnLetter方法生成对应的Letter实例。
 * 
 * 连接letterController判断纸条是否已经被剪刀裁剪过，如果已经裁剪过则保留当前实例，否则删除当前实例。
 */

using UnityEngine;

public class LetterManager : MonoBehaviour
{
    public GameObject currentLetterInstance;

    public MyScissors scissors;

    public void SpawnLetter(GameObject prefab)
    {

        
        if (currentLetterInstance != null)
        {
           LetterController letterController = currentLetterInstance.GetComponent<LetterController>();

            if (letterController != null && letterController.isCut)
            {
                Debug.Log("Keep" + currentLetterInstance.name); // 如果已经裁剪当前实例，则保留，当前实例按钮全局禁用，可继续生成其他实例
            }
            else
            {
                Destroy(currentLetterInstance); // 如果未裁剪就切换，则删除此次实例
            }
            
            //currentLetterInstance = null; // 重置当前实例
        }

        // 生成新实例
        currentLetterInstance = Instantiate(prefab);

    }
}