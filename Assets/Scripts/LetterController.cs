/*
 * 负责纸条的状态显示逻辑和碎片的位移逻辑。
 * 
 * 每个纸条都是一个预制体，选中纸条UI后生成对应实例。
 * 
 * 根物体：LetterController
 * Trigger,检测剪刀碰撞
 * 
 * 子物体：ArtPiece
 * Collider，用于拖动
 * 
 */
using UnityEngine;

public class LetterController : MonoBehaviour
{
    public GameObject[] stateGroups; // 包含不同状态的子物体组

    private FlipController flipCtrl;

    private int currentStateIndex = 0; // 当前状态索引

    private bool cut0 = false;
    private bool cut1 = false;

    // 只要剪开过任何一部分，就标记为“已动过”，Manager 据此决定是否保留实例
    public bool isCut => cut0 || cut1;

    public bool isFullyBroken => cut0 && cut1;

    void Awake()
    {
        flipCtrl = GetComponent<FlipController>();
    }

    /*
     * ID用于区分是哪个触发器触发的。
     * ID是由挂在感应区上的子物体trigger脚本传给LetterController的
     */
    public void StartCutting(int triggerID)
    {
        if (flipCtrl != null && !flipCtrl.canCut)
        {
            Debug.Log("正面状态，无法裁剪");
            return;
        }

        if (triggerID == 0) cut0 = true;
        if (triggerID == 1) cut1 = true;

        int targetIndex = 0;
        if (cut0 && cut1)
        {
            targetIndex = 3;
        }
        else if (cut0)
        {
            targetIndex = 1;
        }
        else if (cut1)
        {
            targetIndex = 2;
        }
        else
        {
            targetIndex = 0;
        }
        
        UpdateDisplay(targetIndex);
    }

    private void UpdateDisplay(int nextIndex)
    {
        if(nextIndex >= stateGroups.Length || nextIndex < 0) return;
        if(nextIndex == currentStateIndex) return;

        // 隐藏旧状态，显示新状态
        stateGroups[currentStateIndex].SetActive(false);
        stateGroups[nextIndex].SetActive(true);

        // 更新当前状态索引
        currentStateIndex = nextIndex;

        ApplyPhysicsToNewPieces(stateGroups[nextIndex]);

    }

    private void ApplyPhysicsToNewPieces(GameObject newStateGroup)
    {
        // 获取新状态组中的所有碎片
        foreach(ArtPiece p in newStateGroup.GetComponentsInChildren<ArtPiece>())
        {
            if (!p.canDrag)
            {
                p.canDrag = true;

                Rigidbody2D rb = p.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.simulated = true;

                    // 施加一个随机的力和旋转
                    Vector2 randomForce = new Vector2(Random.Range(-200f, 200f), Random.Range(100f, 300f));
                    float randomTorque = Random.Range(-200f, 200f);
                    rb.AddForce(randomForce);
                    rb.AddTorque(randomTorque);
                }
            }
            
        }
    }

}
