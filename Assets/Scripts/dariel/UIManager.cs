using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private Vector3 _posToMoveUIPanelOpen, _posToMoveUIPanelClose;
    [SerializeField]
    private GameObject _uiPanel;
    [SerializeField]
    private float _uiPanelMoveTime = 0.5f;
    private bool _uiPanelOpen = false;
    [SerializeField]
    private GameObject _scrollContent;
    [SerializeField]
    private GameObject _uiStructureItem;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public void InitUI()
    {
        InitUIPanel();
        InitUIConstructions();
    }


    private void InitUIConstructions()
    {
        // COUNT Lenght = UI count structures
        for (int i = 0; i < 2; i++)
        {
            GameObject gameObj = Instantiate(_uiStructureItem, _scrollContent.transform);
            //gameObj.GetComponent<PlatformTypeClick>().SetStructures((Structures)i);
            
            switch (i)
            {
                case 0:
                    gameObj.GetComponent<PlatformTypeClick>().SetStructures(Structures.Platform);
                    gameObj.GetComponent<PlatformTypeClick>().SetContainer(GameManager.instance.GetPlatformContainer());
                    gameObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/platform");
                    break;
                case 1:
                    gameObj.GetComponent<PlatformTypeClick>().SetStructures(Structures.SimpleTower);
                    gameObj.GetComponent<PlatformTypeClick>().SetContainer(GameManager.instance.GetTowerContainer());
                    gameObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/tower");
                    break;
                case 2:
                    gameObj.GetComponent<PlatformTypeClick>().SetStructures(Structures.Construction);
                    gameObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/platform");
                    break;
                default:
                    break;
            }
            
        }
    }

    public void InitUIPanel()
    {
        _posToMoveUIPanelClose = _uiPanel.transform.localPosition;
        _posToMoveUIPanelOpen.y = _uiPanel.transform.localPosition.y + (_uiPanel.GetComponent<RectTransform>().rect.height / 2);
    }

    public void ActivateScroll(bool isActive)
    {
        _scrollContent.transform.parent.parent.GetComponent<ScrollRect>().enabled = isActive;
    }

    public void UIPanelClick()
    {
        Debug.Log("UIPanel Click");
        if (!_uiPanelOpen)
        {
             StartCoroutine(LerpPosition(_posToMoveUIPanelOpen, _uiPanel, _uiPanelMoveTime));
             _uiPanelOpen = true;
         }
         else
         {
             StartCoroutine(LerpPosition(_posToMoveUIPanelClose, _uiPanel, _uiPanelMoveTime));
             _uiPanelOpen = false;
         }

    }

    IEnumerator LerpPosition(Vector3 targetPosition, GameObject objectToMove, float duration)
    {
        float time = 0;
        Vector3 startPosition = objectToMove.transform.localPosition;

        while (time < duration)
        {
            objectToMove.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        objectToMove.transform.localPosition = targetPosition;
    }
}
