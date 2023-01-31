using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TransperentItemDrug : MonoBehaviour, IDragHandler
{
    private Transform _transform;
    [SerializeField]
    private Color _wrongInstallColor;
    [SerializeField]
    private Material _wrongMat;
    [SerializeField]
    private Material _DefalultMat;
    private bool _isPlaceable = true;
    private Coroutine _checkPlaceColorC;
    private LayerMask _mask;
    private BoxCollider2D _boxCollider2D;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _mask = LayerMask.GetMask("PlatformsAndStaff");
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        //transform.position =  new Vector3(transform.position.x+ (eventData.delta.x * 0.01f), transform.position.y + (eventData.delta.y * 0.01f), transform.position.z);
        
        
        //Vector3 snap;
        //float gridSize = 0.5f;
        //snap.x = Mathf.RoundToInt(transform.position.x / gridSize) * gridSize;
        //snap.y = Mathf.RoundToInt(transform.position.y / gridSize) * gridSize;
        //transform.position = new Vector3(snap.x, snap.y, 0);
    }
    
    public void StartCheckPlace()
    {
        Debug.Log("StartCheckPlace");
        _mask = LayerMask.GetMask("PlatformsAndStaff");
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _checkPlaceColorC = StartCoroutine(CheckPlaceColor());

    }

    IEnumerator CheckPlaceColor()
    {
        if (GetComponent<BoxCollider2D>() != null)
        {
            if (GetComponent<BoxCollider2D>().IsTouchingLayers(_mask))
            {
                Debug.Log("Wrong Place");
                GetComponent<SpriteRenderer>().material = _wrongMat;
                //if (GetComponent<SpriteRenderer>().material != _wrongMat)
                //{
                //    GetComponent<SpriteRenderer>().material = _wrongMat;
                //}
            }
            else 
            {
                GetComponent<SpriteRenderer>().material = _DefalultMat;
            }
                    
        }
        yield return new WaitForSeconds(0.2f);
    }


    public void OnPointerUpCustom()
    {
        Debug.Log("OnPointerUp TransperentItemDrug");
        UIManager.instance.ActivateScroll(true);
        
        if (!_boxCollider2D.IsTouchingLayers(_mask))
        {
            Debug.Log("IsTouchingLayers@@@ ==== true");
            GameManager.instance.PlacePlatform(transform.position);
        }
        /*
        if (_isPlaceable)
        {
            Debug.Log("_isPlaceable ==== true");
            GameManager.instance.PlacePlatform(transform.position);
        }
        */
        Destroy(gameObject);//,0.2f);
    }
  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter2D---FALSE");
        GetComponent<SpriteRenderer>().material = _wrongMat;
        _isPlaceable = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("OnTriggerExit2D===TRUE");
        //GetComponent<SpriteRenderer>().material.color = Color.white;
        GetComponent<SpriteRenderer>().material = _DefalultMat;
        _isPlaceable = true;
    }
  
    void Update()
    {
        Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        newPos.x = Mathf.RoundToInt(newPos.x / GameManager.instance.gridSize) * GameManager.instance.gridSize;
        newPos.y = Mathf.RoundToInt(newPos.y / GameManager.instance.gridSize) * GameManager.instance.gridSize;
        //transform.position = new Vector3(snap.x, snap.y, 0);
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
    }
}
