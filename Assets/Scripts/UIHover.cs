using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject myImage;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        myImage.SetActive(true);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        myImage.SetActive(false);
    }
}
