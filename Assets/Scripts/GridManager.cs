using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int rows = 3;    
    public int cols = 5;   
    public float cellSize = 1.0f;

    private int[] currentTargetSequence;

    private BoxCollider2D areaCollider;

    public bool hasCompleted = false;

    void Awake()
    {
        areaCollider = GetComponent<BoxCollider2D>();

        if (areaCollider != null)
        {
            areaCollider.size = new Vector2(cols * cellSize, rows * cellSize);
        }
    }

    // 传入当前关卡的目标序列
    public void SetLevelData(LevelData data)
    {
        currentTargetSequence = data.targetSequence;
    }

    public void TrySnapAndCheck(ArtPiece piece)
    {
        // 获取碎片相对于 GridManager 中心点的局部位置
        Vector3 localPos = piece.transform.position - transform.position;

        // 判定是否在网格定义的矩形范围内
        float halfWidth = (cols * cellSize) / 2f;
        float halfHeight = (rows * cellSize) / 2f;

        if (localPos.x >= -halfWidth && localPos.x <= halfWidth &&
            localPos.y >= -halfHeight && localPos.y <= halfHeight)
        {
            // 计算最近的网格点坐标 (局部)
            float snappedX = Mathf.Round(localPos.x / cellSize) * cellSize;
            float snappedY = Mathf.Round(localPos.y / cellSize) * cellSize;

            // 应用吸附 (世界坐标)
            piece.transform.position = transform.position + new Vector3(snappedX, snappedY, 0);
        }

        CheckSequence();
    }

    public void CheckSequence()
    {
        if (hasCompleted) return;

        // 顺序检测：根据文档确定行数。
        ArtPiece[] allPieces = FindObjectsByType<ArtPiece>(FindObjectsSortMode.None);
        List<ArtPiece> snappedPieces = allPieces
.        Where(p => areaCollider.OverlapPoint(p.transform.position))
        .OrderByDescending(p => Math.Round(p.transform.position.y, 1))
        .ThenBy(p => Math.Round(p.transform.position.x, 1))
        .ToList();

        int[] currentSequence = snappedPieces.Select(p => p.pieceID).ToArray();
        Debug.Log("当前序列: " + string.Join(", ", currentSequence));

        if (currentTargetSequence != null && Enumerable.SequenceEqual(currentSequence, currentTargetSequence))
        {
            hasCompleted = true;

            Debug.Log("正确");

            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.OnLevelComplete();
            }
            else
            {
                Debug.LogError("场景中找不到 LevelManager");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = transform.position;

        float fullWidth = cols * cellSize;
        float fullHeight = rows * cellSize;

        // 外框
        Gizmos.DrawWireCube(center, new Vector3(fullWidth, fullHeight, 0));

        // 网格点
        Gizmos.color = new Color(0, 1, 1, 0.3f);

        float startX = -(cols - 1) * cellSize / 2f;
        float startY = -(rows - 1) * cellSize / 2f;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                Vector3 dotPos = center + new Vector3(startX + c * cellSize, startY + r * cellSize, 0);
                Gizmos.DrawSphere(dotPos, 0.05f);
            }
        }
    }
}