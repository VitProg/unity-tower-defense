using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TransperentItemDrug : MonoBehaviour
{
    private Transform _transform;
    [SerializeField]
    private Color _wrongInstallColor;
    [SerializeField]
    private Material _wrongMat;
    [SerializeField]
    private Material _DefalultMat;
    private bool _isPlaceable = true;
    //private Coroutine _checkPlaceColorC;
    private LayerMask _mask, _maskRoad;
    private BoxCollider2D _boxCollider2D;
    private SpriteRenderer _spriteRenderer;
    public Structures structures = default;

    public void StartCheckPlace()
    {
        Debug.Log("StartCheckPlace");
        //_mask = LayerMask.GetMask("PlatformsAndStaff");
        //_maskRoad = LayerMask.GetMask("BackObjectLayer");
        //_boxCollider2D = GetComponent<BoxCollider2D>();
        //_spriteRenderer = GetComponent<SpriteRenderer>();
        //_checkPlaceColorC = StartCoroutine(CheckPlaceColor());
    }

    IEnumerator CheckPlaceColor()
    {
        if (GetComponent<BoxCollider2D>() != null)
        {
            if (GetComponent<BoxCollider2D>().IsTouchingLayers(_mask) | GetComponent<BoxCollider2D>().IsTouchingLayers(_maskRoad))
            {
                Debug.Log("Wrong Place");
                GetComponent<SpriteRenderer>().material = _wrongMat;
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
        Debug.Log("OnPointerUpCustom TransperentItemDrug");
        UIManager.instance.ActivateScroll(true);
        /*
        if (GetComponent<BoxCollider2D>().IsTouchingLayers(_mask) | GetComponent<BoxCollider2D>().IsTouchingLayers(_maskRoad))
        {
            Debug.Log("Destroy(gameObject)@ ");
        }
        else
        {
            Debug.Log("IsTouchingLayers@@@ ==== Faaaalse");
            GameManager.instance.PlacePlatform(transform.position);
        }
        Destroy(gameObject);
        */
        
        if (_isPlaceable)
        {
            Debug.Log("_isPlaceable ==== true");
            GameManager.instance.PlaceStructures(transform.position, structures);
        }
        Destroy(gameObject);
    }
  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter2D---FALSE");
        GetComponent<SpriteRenderer>().material = _wrongMat;
        _isPlaceable = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        GetComponent<SpriteRenderer>().material = _wrongMat;
        _isPlaceable = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("OnTriggerExit2D===TRUE");
        GetComponent<SpriteRenderer>().material = _DefalultMat;
        _isPlaceable = true;
    }
  
    void Update()
    {
        Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newPos.x = Mathf.RoundToInt(newPos.x / GameManager.instance.gridSize) * GameManager.instance.gridSize;
        newPos.y = Mathf.RoundToInt(newPos.y / GameManager.instance.gridSize) * GameManager.instance.gridSize;
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
    }
}
