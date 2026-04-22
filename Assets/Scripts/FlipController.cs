using UnityEngine;

public class FlipController : MonoBehaviour
{
    // 负责翻转纸条的显示逻辑,按下tab键翻转纸条，共有正面和背面两种状态。
    // 只有纸条处于背面状态时才允许剪刀裁剪，Manager据此决定是否保留实例。

    public GameObject front; // 正面
    public GameObject back; // 背面

    private bool isBack = false; // 默认打开时是正面，用于判断当前是否处于背面状态

    public bool canCut => isBack; // 只有背面状态才允许剪刀裁剪

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isBack = !isBack; // 切换状态
            front.SetActive(!isBack); // 正面显示取反
            back.SetActive(isBack); // 背面显示与状态一致

        }
    }

}
