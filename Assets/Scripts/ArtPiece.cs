using UnityEngine;

public class ArtPiece : MonoBehaviour
{
    public int pieceID; // 策划可以在 Inspector 中定义每个碎片的编码
    private GridManager gridManager;
    private bool isDragging;
    public bool canDrag = false;

    Collider2D targetCol;

    Vector2 startPos;

    void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();

        targetCol = GetComponent<Collider2D>();

        startPos = this.transform.position;

    }

    void Update()
    {
        DragAndDrop();
    }


    void DragAndDrop()
    {
        if (!canDrag) return;//没裁剪开前不能拖动

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (targetCol.OverlapPoint(mousePos))
            {
                isDragging = true;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;

            gridManager.TrySnapAndCheck(this);

        }
        if (isDragging)
        {
            this.transform.position = mousePos;
        }
    }
}