using UnityEngine;
using UnityEngine.EventSystems;

public class UIPanelOpenClose : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button == PointerEventData.InputButton.Left) UIManager.instance.UIPanelClick();
    }
}