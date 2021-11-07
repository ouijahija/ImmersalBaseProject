using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class MeshRaycaster : MonoBehaviour
{
    private Camera _mainCamera;

    private bool _isTouching;
#if UNITY_EDITOR
    public bool useScreenCenter;
#endif

    void Awake()
    {
        _mainCamera = Camera.main;
    }

    void Update()
    {
        if (Application.isEditor)
            RaycastInEditor(out var hitPosition);
        else
            RaycastOnDevice(out var hitPosition);
    }

    private bool RaycastOnDevice(out Vector3 hitPosition)
    {
        hitPosition = Vector3.negativeInfinity;
        Touch touch;
        Vector3 touchPosition;

        if (Input.touchCount == 0)
        {
            _isTouching = false;
            return false;
        }

        if (Input.touchCount != 1 || _isTouching || Input.GetTouch(0).phase != TouchPhase.Began)
            return false;

        _isTouching = true;

        if (Input.touches.Select(touch => touch.fingerId)
            .Any(id => EventSystem.current.IsPointerOverGameObject(id)))
            return false;

        touch = Input.GetTouch(0);
        touchPosition = touch.position;


        return RaycastToMesh(touchPosition, out hitPosition);
    }

    private bool RaycastInEditor(out Vector3 hitPosition)
    {
        hitPosition = Vector3.negativeInfinity;
        Vector3 touchPosition;
#if UNITY_EDITOR
        if (useScreenCenter)
        {
            if (Input.GetMouseButtonDown(0))
                touchPosition = new Vector3(Screen.width / 2f, Screen.height / 2f);
            else
                return false;

            return RaycastToMesh(touchPosition, out hitPosition);
        }
#endif

        if (EventSystem.current.IsPointerOverGameObject())
            return false;

        if (Input.GetMouseButtonDown(0))
            touchPosition = Input.mousePosition;
        else
            return false;


        return RaycastToMesh(touchPosition, out hitPosition);
    }

    private bool RaycastToMesh(Vector3 touchPosition, out Vector3 hitPosition)
    {
        hitPosition = Vector3.negativeInfinity;
        Ray gazeRay = _mainCamera.ScreenPointToRay(touchPosition);
        var hasHit = Physics.Raycast(gazeRay, out var hit, float.PositiveInfinity);

        if (!hasHit)
            return false;

        var raycastSelectable = hit.transform.GetComponent<IRaycastSelectable>();
        if (raycastSelectable == null)
            return false;

        raycastSelectable.OnRaycastReceived();

        hitPosition = hit.point;
        return true;
    }
}