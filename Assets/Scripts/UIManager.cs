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
    private ScrollRect _scrollRect;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        InitUIPanel();
        
    }

    public void InitUIPanel()
    {
        _posToMoveUIPanelClose = _uiPanel.transform.localPosition;
        _posToMoveUIPanelOpen.y = (_posToMoveUIPanelClose.y - _uiPanel.transform.position.y) + (_uiPanel.GetComponent<RectTransform>().rect.height / 2);
    }

    public void ActivateScroll(bool isActive)
    {
        _scrollRect.enabled = isActive;
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
