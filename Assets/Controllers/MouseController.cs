using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
public class MouseController : MonoBehaviour
{
    public GameObject _circleCursorPrefab;
    public GameObject _circleCursorParent;
    Vector3 _currFramePosition;
    Vector3 _lastFramePosition;
    Vector3 _startDragPosition;
    bool _isDragging = false;
    Tile _tileUnderMouse;
    Tile _tileLastFrame;
    List<GameObject> _dragPreviewGameObjects;

    // Use this for initialization
    void Start()
    {
        _dragPreviewGameObjects = new List<GameObject>();
        // Preload 100 circleCursors when loading the scene
        SimplePool.Preload(_circleCursorPrefab, 100, _circleCursorParent);
    }

    // Update is called once per frame
    void Update()
    {
        _currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _tileUnderMouse = WorldController.Instance.GetTileAtCoordinates(_currFramePosition);

        UpdateCameraMovement();
        // UpdateCursor();
        UpdateDrag();
        UpdateCameraZooming();

        _lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _tileLastFrame = _tileUnderMouse;
    }

    void UpdateCameraMovement()
    {
        // Handle screen dragging (camera movement)
        if (Input.GetMouseButton(2))
        {
            var cameraMovement = _lastFramePosition - _currFramePosition;
            Camera.main.transform.Translate(cameraMovement);
        }
    }

    void UpdateCameraZooming()
    {
        // Camera.main.orthographicSize -= Input.mouseScrollDelta.y;
        Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel");
    }

    void UpdateDrag()
    {
        CleanUpDraggingCursors();
        // If we are over a UI element, then skip this function
        if (EventSystem.current.IsPointerOverGameObject() && _isDragging == false)
        {
            return;
        }

        // Start Handle actions (left mouse clicks)
        if (Input.GetMouseButtonDown(0))
        {
            _startDragPosition = _currFramePosition;
            GameObject go = SimplePool.Spawn(_circleCursorPrefab, new Vector3(_tileUnderMouse.X, _tileUnderMouse.Y, 0), Quaternion.identity);
            _dragPreviewGameObjects.Add(go);
            _isDragging = true;
        }

        if (Input.GetMouseButton(0) && _tileLastFrame != _tileUnderMouse)
        {
            // Display a preview of the drag area
            Tile[] tilesToEdit = WorldController.Instance.GetTilesAtCoordinates(_startDragPosition, _currFramePosition);
            if (tilesToEdit != null)
            {
                foreach (var tile in tilesToEdit)
                {
                    GameObject go = SimplePool.Spawn(_circleCursorPrefab, new Vector3(tile.X, tile.Y, 0), Quaternion.identity);
                    _dragPreviewGameObjects.Add(go);
                }
            }
        }

        // End Handle actions (left mouse clicks)
        if (Input.GetMouseButtonUp(0))
        {
            BuildModeController buildModeController = GameObject.FindObjectOfType<BuildModeController>();
            Tile[] tilesToEdit = WorldController.Instance.GetTilesAtCoordinates(_startDragPosition, _currFramePosition);
            // We are in InstallObjects mode
            foreach (var tile in tilesToEdit)
            {
                buildModeController.DoBuild(tile);
            }
            _isDragging = false;
        }
    }


    private void CleanUpDraggingCursors()
    {
        while (_dragPreviewGameObjects.Count > 0 && (_isDragging == false || _tileLastFrame != _tileUnderMouse))
        {
            GameObject go = _dragPreviewGameObjects[0];
            _dragPreviewGameObjects.RemoveAt(0);
            SimplePool.Despawn(go);
        }
    }
}
