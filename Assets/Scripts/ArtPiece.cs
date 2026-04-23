using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class ArtPiece : MonoBehaviour
{
    [Tooltip("定义每个碎片的编码，最小为1不能是0，每个碎片编码不能相同")]
    public int pieceID; 
    private GridManager gridManager;
    private bool isDragging;
    public bool canDrag = false;
    private SpriteRenderer sr;


    Collider2D targetCol;

    Vector2 startPos;

    void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();

        targetCol = GetComponent<Collider2D>();

        startPos = this.transform.position;

        // 设置渲染顺序，确保碎片按照pieceID的顺序渲染，较大的pieceID在上面
        sr = GetComponent<SpriteRenderer>();
        sr.sortingOrder = pieceID;

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
            Collider2D[] hits = Physics2D.OverlapPointAll(mousePos);

            if (hits.Length > 0)
            {
                Collider2D topCol = hits[0];
                int topOrder = topCol.GetComponent<SpriteRenderer>().sortingOrder;

                foreach (var col in hits)
                {
                    SpriteRenderer sr = col.GetComponent<SpriteRenderer>();
                    if (sr != null && sr.sortingOrder > topOrder)
                    {
                        topOrder = sr.sortingOrder;
                        topCol = col;
                    }
                }

                if (topCol == targetCol)
                {
                    isDragging = true;
                }
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