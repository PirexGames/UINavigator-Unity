using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using Cysharp.Threading.Tasks;

public class UIDragItem : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Drag UI")]
    [SerializeField] private RectTransform dragObj;
    [SerializeField] private bool isEffectUIDrag;
    [SerializeField] private DragType dragType;
    [SerializeField] private DirHVType dirType;
    [SerializeField] private float ratio = 1f;

    [Header("Disable ClickButton When Drag")]
    [SerializeField] private Button buttonDisableWhenDrag;

    [Header("Resize UI")]
    [SerializeField] private float minSize;
    [SerializeField] private float maxSize;
    [SerializeField] private bool isContrain;

    [Header("Limit On Canvas Screen")]
    [SerializeField] private bool limitOnCanvas;
    [SerializeField] private RectTransform canvas;
    [SerializeField] private Vector2 offset;


    [Header("Camera")]
    [SerializeField] private string cameraName;

    private Action<PointerEventData> callbacks;
    public RectTransform DragRect => dragObj;
    public Vector2 Scale => new Vector2(dragObj.rect.width, dragObj.rect.height);
    private Vector2 _minCanvasPosition;
    private Vector2 _maxCanvasPosition;
    private Vector3 _prevDir;
    private Camera cameraViewport;
    private void Start()
    {
        if (cameraName == string.Empty || cameraName == null)
        {
            cameraViewport = Camera.main;
        }
        else
        {
            var cam = GameObject.Find(cameraName);
            var camViewport = cam.GetComponent<Camera>();
            cameraViewport = camViewport ? camViewport : Camera.main;
        }


        if (!limitOnCanvas) return;
        _minCanvasPosition = new Vector2(
            canvas.position.x - canvas.rect.width / 2 + dragObj.rect.width / 2,
            canvas.position.y - canvas.rect.height / 2 + dragObj.rect.height / 2
        ) + offset;

        _maxCanvasPosition = new Vector2(
            canvas.position.x + canvas.rect.width / 2 - dragObj.rect.width / 2,
            canvas.position.y + canvas.rect.height / 2 - dragObj.rect.height / 2
        ) - offset;

    }
    public void Subscribe(Action<PointerEventData> callback)
    {
        callbacks -= callback;
        callbacks += callback;
    }
    public void UnSubscribe(Action<PointerEventData> callback)
    {
        callbacks -= callback;
    }
    public void OnPointerDown(PointerEventData data)
    {

    }

    public async void OnPointerUp(PointerEventData data)
    {
        _prevDir = Vector3.zero;
        if (buttonDisableWhenDrag)
        {
            await UniTask.NextFrame();
            buttonDisableWhenDrag.interactable = true;
        }
    }

    public void OnDrag(PointerEventData data)
    {
        if (buttonDisableWhenDrag)
        {
            buttonDisableWhenDrag.interactable = false;
        }
        callbacks?.Invoke(data);
        float x = (dirType & DirHVType.HORIZONTAL) == DirHVType.HORIZONTAL ? data.delta.x * ratio : 0;
        float y = (dirType & DirHVType.VERTICAL) == DirHVType.VERTICAL ? data.delta.y * ratio : 0;

        switch (dragType)
        {
            case DragType.MOVE:
                if (isEffectUIDrag)
                {
                    var position = dragObj.position + new Vector3(x, y);
                    if (limitOnCanvas)
                    {
                        var lx = Mathf.Clamp(position.x, _minCanvasPosition.x, _maxCanvasPosition.x);
                        var ly = Mathf.Clamp(position.y, _minCanvasPosition.y, _maxCanvasPosition.y);
                        position = new Vector2(lx, ly);
                    }
                    dragObj.position = position;
                }
                break;

            case DragType.ROTATE:
                var dir = (cameraViewport.ScreenToViewportPoint(data.position) - cameraViewport.ScreenToViewportPoint(dragObj.transform.position));

                var angle = (Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg);
                if (isEffectUIDrag)
                    dragObj.rotation = Quaternion.Euler(0, 0, -angle);
                break;

            case DragType.SCALE:
                if (!isEffectUIDrag) return;

                var dirs = (cameraViewport.ScreenToViewportPoint(data.position) - cameraViewport.ScreenToViewportPoint(dragObj.transform.position));

                if (dirs.x * _prevDir.x < 0 || dirs.y * _prevDir.y < 0) return;

                int ax = dirs.x > 0 ? 1 : -1;
                int ay = dirs.y > 0 ? 1 : -1;
                float s = Mathf.Abs(data.delta.x) > Mathf.Abs(data.delta.y) ? data.delta.x * ax : data.delta.y * ay;
                if (isContrain)
                {
                    float sizeX = Mathf.Clamp(dragObj.sizeDelta.x + s, minSize, maxSize);
                    float sizeY = Mathf.Clamp(dragObj.sizeDelta.y + s, minSize, maxSize);
                    dragObj.sizeDelta = new Vector2(sizeX, sizeY);
                }
                else
                {
                    float sizeX = Mathf.Clamp(dragObj.sizeDelta.x + data.delta.x * ax, minSize, maxSize);
                    float sizeY = Mathf.Clamp(dragObj.sizeDelta.y + data.delta.y * ay, minSize, maxSize);
                    dragObj.sizeDelta = new Vector2(sizeX, sizeY);
                }

                _prevDir = dirs;
                break;
        }
    }

    public void ResetSize(Vector2 size)
    {
        dragObj.sizeDelta = size;
    }

    public void SetMinMaxSize(float min, float max)
    {
        minSize = min;
        maxSize = max;
    }

    public void SetObjectDrag(RectTransform obj)
    {
        dragObj = obj;
    }
}

enum DragType
{
    MOVE,
    ROTATE,
    SCALE,
}

[System.Flags]
enum DirHVType
{
    nothing = 0,
    HORIZONTAL = 1 << 0,
    VERTICAL = 1 << 1,
    everything
}