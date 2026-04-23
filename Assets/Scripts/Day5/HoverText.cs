using UnityEngine;
using UnityEngine.EventSystems;

public class HoverItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    public string message;

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverManager.Instance.Show(message);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HoverManager.Instance.Hide();
    }
}