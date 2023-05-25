using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

// [RequireComponent(typeof(TMP_Text))]
public class ButtonColorControl : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Color textNormalColor;
    public Color textHoverColor;
    public Color textPressedColor;
    
    public Color bgNormalColor;
    public Color bgHoverColor;
    public Color bgPressedColor;

    public TMP_Text text;
    public Image image;

    public void Start()
    {
        text ??= transform.GetComponentInChildren<TMP_Text>();
        image ??= GetComponent<Image>();
        textNormalColor = text.color;
        bgNormalColor = image.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = textHoverColor;
        image.color = bgHoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = textNormalColor;
        image.color = bgNormalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        text.color = textPressedColor;
        image.color = bgPressedColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        text.color = textNormalColor;
        image.color = bgNormalColor;
    }
}
