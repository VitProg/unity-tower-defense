using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlatformTypeClick : MonoBehaviour ,IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private GameObject _movePlatformType;
    [SerializeField]
    private GameObject _platformContainer;
    private GameObject _platform;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        UIManager.instance.UIPanelClick();
        //Vector3 pos = Camera.current.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y,0));
        Vector3 pos = transform.InverseTransformPoint(eventData.position);
        _platform = Instantiate(_movePlatformType, new Vector3(pos.x,pos.y, _movePlatformType.transform.position.z), 
            Quaternion.identity, _platformContainer.transform);
        _platform.GetComponent<TransperentItemDrug>().StartCheckPlace();
        UIManager.instance.ActivateScroll(false);
        Debug.Log("OnPointerDown PlatformTypeClick Click");
        //_platform.GetComponent<TransperentItemDrug>().OnPointerDown(eventData);
        //_platform.GetComponent<TransperentItemDrug>().OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _platform.GetComponent<TransperentItemDrug>().OnPointerUpCustom();
    }
}
