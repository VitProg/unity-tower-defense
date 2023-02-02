using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlatformTypeClick : MonoBehaviour ,IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private GameObject _movePlatformType;
    private GameObject _container;
    private GameObject _platform;
    private Structures _structures;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        UIManager.instance.UIPanelClick();
        Vector3 pos = transform.InverseTransformPoint(eventData.position);
        _platform = Instantiate(_movePlatformType, new Vector3(pos.x, pos.y, 10),//_movePlatformType.transform.position.z), 
            Quaternion.identity, GameManager.instance.transform);
//_platform.transform.localScale = _platform.transform.localScale * 3;
        _platform.GetComponent<SpriteRenderer>().sprite = GetComponent<Image>().sprite;
        _platform.transform.SetAsFirstSibling();
        _platform.GetComponent<TransperentItemDrug>().structures = _structures;
        UIManager.instance.ActivateScroll(false);
        _platform.GetComponent<TransperentItemDrug>().StartCheckPlace();
        Debug.Log("OnPointerDown PlatformTypeClick Click");
    }

    public void SetStructures(Structures structures)
    {
        _structures = structures;
    }

    public void SetContainer(GameObject obj)
    {
        _container = obj;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _platform.GetComponent<TransperentItemDrug>().OnPointerUpCustom();
    }
}
