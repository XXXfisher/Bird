/*
 * 负责纸条的裁剪逻辑和碎片的位移逻辑。
 * 
 * 每个纸条都是一个预制体，包含一个根物体和一个子物体。选中纸条UI后生成对应实例。
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
    public Transform[] pieces;

    public bool isCut = false;

    void Awake()
    {
        pieces = GetComponentsInChildren<Transform>();
    }

    public void StartCutting()
    {
        if (isCut) return; // 标记已裁剪后该实例在切换纸条时，不会被销毁
        isCut = true;

        // 纸条被裁剪的位移逻辑

        foreach (Transform piece in pieces)
        {
            if (piece != transform) // 排除根物体
            {
                Rigidbody2D rb = piece.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    // 随机一个小的位移方向和力度
                    Vector2 randomDir = Random.insideUnitCircle.normalized;
                    float randomForce = Random.Range(50f, 150f);
                    rb.AddForce(randomDir * randomForce);
                }
            }

        }

        // 开启pieces拖动开关
        foreach (ArtPiece p in GetComponentsInChildren<ArtPiece>())
        {
            p.canDrag = true;
        }

    }
}
