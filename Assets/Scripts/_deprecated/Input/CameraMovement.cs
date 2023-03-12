using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Tilemaps;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private PixelPerfectCamera pixelCam;
    [SerializeField] private Tilemap tilemap;

    [SerializeField] private float zoomStep, minCamSize, maxCamSize;


    private Vector3 dragOrigin;
    private bool isDragging;

    private float mapMinX, mapMaxX, mapMinY, mapMaxY;


    // Start is called before the first frame update
    private void Awake()
    {
        tilemap.CompressBounds();

        mapMinX = tilemap.transform.position.x - tilemap.localBounds.size.x / 2;
        mapMaxX = tilemap.transform.position.x + tilemap.localBounds.size.x / 2;

        mapMinY = tilemap.transform.position.y - tilemap.localBounds.size.y;
        mapMaxY = tilemap.transform.position.y + tilemap.localBounds.size.y / 2;
    }

    // Update is called once per frame
    private void Update()
    {
        PanCamera();
        Zoom();
    }

    private void PanCamera()
    {
        if (Input.GetMouseButtonUp(1)) isDragging = false;

        //if (InputManager.IsPointerOverUIObject()) return;


        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
            isDragging = true;
        }

        if (Input.GetMouseButton(1) && isDragging)
        {
            var diff = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);

            cam.transform.position = ClampCamera(cam.transform.position + diff);
        }
    }

    private void Zoom()
    {
        if (Input.mouseScrollDelta.y > 0f) // Scroll up
        {
            var newSize = pixelCam.assetsPPU + zoomStep;

            pixelCam.assetsPPU = (int)Mathf.Clamp(newSize, minCamSize, maxCamSize);
        }

        if (Input.mouseScrollDelta.y < 0f) // Scroll down
        {
            var newSize = pixelCam.assetsPPU - zoomStep;

            pixelCam.assetsPPU = (int)Mathf.Clamp(newSize, minCamSize, maxCamSize);
        }

        cam.transform.position = ClampCamera(cam.transform.position);
    }

    private Vector3 ClampCamera(Vector3 targetPosition)
    {
        var camHeight = cam.orthographicSize;
        var camWidth = cam.orthographicSize * cam.aspect;

        var minX = mapMinX + camWidth;
        var maxX = mapMaxX - camWidth;

        var minY = mapMinY + camHeight;
        var maxY = mapMaxY - camHeight;

        var newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        var newY = Mathf.Clamp(targetPosition.y, minY, maxY);

        return new Vector3(newX, newY, targetPosition.z);
    }
}