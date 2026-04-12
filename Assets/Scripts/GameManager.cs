using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject holdingObject;
    private Vector3 offset;
    private GridManager gridManager;

    void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryGrabObject();
        }

        if (Input.GetMouseButton(0) && holdingObject != null)
        {
            DragObject();
        }

        if (Input.GetMouseButtonUp(0) && holdingObject != null)
        {
            ReleaseObject();
        }
    }

    void TryGrabObject()
    {
        PointerEventData m_PointerEventData = new PointerEventData(EventSystem.current);
        m_PointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(m_PointerEventData, results);

        // 找到标签为 ART_PART 的物体
        foreach (var result in results)
        {
            if (result.gameObject.CompareTag("ART_PART"))
            {
                holdingObject = result.gameObject;
                offset = holdingObject.transform.position - GetMouseWorldPos();
                break;
            }
        }
    }

    void DragObject()
    {
        holdingObject.transform.position = GetMouseWorldPos() + offset;
    }

    void ReleaseObject()
    {
        var pieceScript = holdingObject.GetComponent<ArtPiece>();
        if (pieceScript != null)
        {
            gridManager.TrySnapAndCheck(pieceScript);
        }
        holdingObject = null;
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        return pos;
    }
}