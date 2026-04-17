using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PieceCollage : MonoBehaviour
{
    [SerializeField] EventSystem m_EventSystem;
    PointerEventData m_PointerEventData;
    private GameObject holdingObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            m_PointerEventData = new PointerEventData(m_EventSystem);

            //Set the Pointer Event Position to that of the game object
            m_PointerEventData.position = Input.mousePosition;

            var results = new List<RaycastResult>();

            EventSystem.current.RaycastAll(m_PointerEventData, results);

            if (results.Count > 0 && results[0].gameObject.tag == "ART_PART")
            {
                //results[0].gameObject.GetComponent<OnItemChosen>().OnSelected();
                holdingObject = results[0].gameObject;
            }
            else
            {
                //GameObject[] parts = GameObject.FindGameObjectsWithTag("FACIAL_PART");
                //for (int i = 0; i < parts.Length; i++)
                //{
                //    parts[i].GetComponent<OnItemChosen>().DisableController();
                //}
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            holdingObject = null;
        }

        if (holdingObject != null)
        {
            Vector2 newVec = Input.mousePosition;
            newVec -= new Vector2(Screen.width / 2, Screen.height / 2);
            holdingObject.GetComponent<RectTransform>().anchoredPosition = newVec;
        }
    }
}
