using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Dragger : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    GraphicRaycaster raycaster;
    EventSystem eventSystem;
    Canvas canvas;
    // cloudy image over the draggable that activates on drag
    Image image;
    public Transform thisParent;

    Color transparentDefault;
    Color draggedColor = new Color32(255, 255, 255, 92);

    public RectTransform rectTransform;
    public Vector2 offset = new Vector2(0, 5);
    // this should be in the parent container
    DropHandler dropHandler;


    // Start is called before the first frame update
    void Start()
    {
        raycaster = AppManager.Instance.appCanvas.GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;

        if (rectTransform == null)
            rectTransform = thisParent.GetComponent<RectTransform>();
        image = GetComponent<Image>();

        canvas = AppManager.Instance.appCanvas;
        image.color = transparentDefault;
        // the parent that holds the profile and sibling index
        thisParent = transform.parent;
        dropHandler = thisParent.parent.GetComponent<DropHandler>();
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        Debug.Log("dragging");
        // if dragger is present and saved to currDragger
        if (CheckPosition(eventData))
        {
            // moves the placeholder to the currDragger's sibling index
            dropHandler.MovePosition(currDragger.thisParent.GetSiblingIndex());
            // and nulls out currDragger
            currDragger = null;
        }

    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("drag begin");

        // changes image color to cloudy when dragging
        image.color = draggedColor;
        // removes self from parent and spawns a placeholder in its place
        dropHandler.SpawnPlaceholder(thisParent.GetSiblingIndex());
        // parents to the canvas
        thisParent.SetParent(AppManager.Instance.appCanvas.transform);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("drag end");
        // sets edited as true if the dragger has been changed
        if (currDragger != null || currDragger != this)
            AppManager.Instance.hasEditedOrder = true;

        currDragger = null;
        image.color = transparentDefault;
        // uses the returned index to place itself in the placeholder's spot
        int index = dropHandler.GetIndexAndDestroy();
        if (index != -1)
        {
            thisParent.SetParent(dropHandler.targetContainer);
            thisParent.SetSiblingIndex(index);
        }
        else
        Debug.LogWarning("No placeholder found in DropHandler.");
    }

    Dragger currDragger;
    // checks if it hits a different dragger
    bool CheckPosition(PointerEventData eventData)
    {
        Debug.Log("check");

        // Raycasts from the center of this object upwards
        eventData.position = (Vector2)transform.position + offset;
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(eventData, results);
        // for visual guide, OnDrawGizmos
        lastRayPosition = eventData.position + offset;

        bool isPresent = false;
        foreach (var r in results)
        {
            // ignores nulls
            if (r.gameObject == null) continue;

            if (r.gameObject.TryGetComponent(out Dragger drag))
            {
                // checks if dragger grabbed is not the same as the current one or this one
                if (drag != this || drag != currDragger)
                {
                    // saves the dragger
                    currDragger = drag;
                    isPresent = true;
                    break;
                }
            }
        }
        return isPresent;
    }


public Vector2 lastRayPosition;

private void OnDrawGizmos()
{
    
    Gizmos.color = Color.red;
    Vector3 world = Camera.main.ScreenToWorldPoint(
        new Vector3(lastRayPosition.x, lastRayPosition.y, 10f)
    );
    Gizmos.DrawSphere(world, 0.1f);
}

}