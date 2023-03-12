using UnityEngine;
using UnityEngine.EventSystems;

public class UIItemPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private Color _wrongInstallColor;

    private Vector3 _beforeDrugPos;

    //[SerializeField]
    //private GameObject _transperentItem;
    private RectTransform _rectTransform;

    private void Start()
    {
        //_transperentItem.SetActive(false);
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("OnCollisionEnter2D-----");
        GetComponent<SpriteRenderer>().material.color = _wrongInstallColor;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("OnCollisionExit2D====");
        GetComponent<SpriteRenderer>().material.color = Color.white;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("UIPlatform DOWN Click");
        //UIManager.instance.UIPanelClick();
        _beforeDrugPos = transform.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("UIPlatform UP Relise");
    }
}