using UnityEngine;

public class ArtPiece : MonoBehaviour
{
    public int pieceID; // 策划可以在 Inspector 中定义每个碎片的编码
    private GridManager gridManager;
    private bool isDragging;
    private Vector3 offset;

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