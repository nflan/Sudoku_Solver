using UnityEngine;
using UnityEngine.EventSystems;

public class TriggerChildren : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        this.transform.GetChild(0).gameObject.SetActive(true);
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
    }
}
