using UnityEditor;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    bool isDragging = false;
    Collider2D targetCol;

    Vector2 startPos;

    void Start()
    {
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

            this.transform.position = startPos;

        }
        if (isDragging)
        {
            this.transform.position = mousePos;
        }
    }
}
