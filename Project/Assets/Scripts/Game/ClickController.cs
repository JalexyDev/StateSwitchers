using UnityEngine;
using UnityEngine.EventSystems;

public class ClickController : MonoBehaviour
{
    private const int LEFT = 0;
    private const float CLICK_TIME = 0.2f;
    private float clickTime = 0;

    private RaycastHit2D curHit;
    private IDragable curDragable = null;

    private bool isClicked = false;


    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) { return; }
        HandleLeftMouse();
    }

    private void HandleLeftMouse()
    {
        if (Input.GetMouseButton(LEFT))
        {
            if (Input.GetMouseButtonDown(LEFT))
            {
                HandleLeftDown();
            }

            if (!isClicked) { return; }

            if (IsDrag())
            {
                HandleLeftDrag();
            }
        }
        if (Input.GetMouseButtonUp(LEFT))
        {
            if (!isClicked) { return; }

            HandleLeftUp();
            ClearData();
        }
    }

    private void HandleLeftDown()
    {
        curHit = Physics2D.CircleCast(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.1f, Vector2.one);

        if (curHit.collider == null) { return; }
        isClicked = true;
    }

    private bool IsDrag()
    {
        clickTime += Time.deltaTime;
        return clickTime > CLICK_TIME;
    }

    private void HandleLeftDrag()
    {
        if (curDragable == null)
        {
            curDragable = curHit.collider.gameObject.GetComponent<IDragable>();
        }
        curDragable?.OnDrag();
    }

    private void HandleLeftUp()
    {
        if (IsDrag())
        {
            curDragable?.StopDrag();
            curDragable = null;
        }
        else
        {
            OnLeftClickUp();
        }
    }

    private void OnLeftClickUp()
    {
        IClickable clickable = curHit.collider.gameObject.GetComponent<IClickable>();
        clickable?.OnClick();
    }

    private void ClearData()
    {
        isClicked = false;
        clickTime = 0;
    }
}
