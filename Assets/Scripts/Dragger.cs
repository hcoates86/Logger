using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Has Drag-and-drop behavior with raycast
public class Dragger : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    GraphicRaycaster raycaster;
    Canvas canvas;
    // cloudy image over the draggable that activates on drag
    Image image;
    // the Transform whose index is being used
    public Transform thisParent;

    Color transparentDefault;
    Color draggedColor = new Color32(255, 255, 255, 92);

    public RectTransform rectTransform;
    // this should be in the parent container
    DropHandler dropHandler;
    int initialIndex;
    bool isDragging = false;

    // Start is called before the first frame update
    void Start()
    {
        raycaster = AppManager.Instance.appCanvas.GetComponent<GraphicRaycaster>();

        if (rectTransform == null)
            rectTransform = thisParent.GetComponent<RectTransform>();
        image = GetComponent<Image>();

        canvas = AppManager.Instance.appCanvas;
        image.color = transparentDefault;
        // the parent that holds the profile and sibling index
        thisParent = transform.parent;
        dropHandler = thisParent.parent.GetComponent<DropHandler>();
    }

 float detachDistance = 600;
    float hideDistance = 580;
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        Vector2 localPointerPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
        (RectTransform)rectTransform.parent,
        eventData.position,
        eventData.pressEventCamera,
        out localPointerPos);

        float dist = Vector2.Distance(localPointerPos, rectTransform.anchoredPosition);
        Debug.Log(dist);
        // if too far from dragged obj hides the placeholder
        if (dist > hideDistance)
        {
            dropHandler.GetIndexAndHide();

            // if touch pulls away too far from the dragged obj, ends drag and sends item back to initial position
            if (dist > detachDistance)
            {
                // stop dragging
                ResetDrag();
                return;
            }
        }
        else
        {
            if (!dropHandler.placeHolder.activeSelf) dropHandler.placeHolder.SetActive(true);
        }

        Vector2 delta = localPointerPos - lastLocalPointerPos;
        rectTransform.anchoredPosition += delta;
        lastLocalPointerPos = localPointerPos;

        // clamps the dragged item into the container
        rectTransform.anchoredPosition = ClampIntoContainer(
        dropHandler.clampedContainer, rectTransform.anchoredPosition,
        padding: Vector2.zero);

        // if dragger is present
        if (CheckPosition(eventData))
        {
            // if no previous dragger uses this dragger's index
            if (currDragger == null)
                dropHandler.MovePosition(initialIndex);
            else
            // moves the placeholder to the currDragger's sibling index
                dropHandler.MovePosition(currDragger.thisParent.GetSiblingIndex());
        }
    }

Vector2 lastLocalPointerPos;
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        initialAnchoredPosition = rectTransform.anchoredPosition;
        isDragging = true;
        initialIndex = thisParent.GetSiblingIndex();
        // changes image color to cloudy when dragging
        image.color = draggedColor;
        // removes self from parent and spawns a placeholder in its place
        dropHandler.ShowPlaceholder(initialIndex);

    // capture world pos to avoid jump when reparenting
    Vector3 worldPos = rectTransform.position;
    thisParent.SetParent(dropHandler.clampedContainer, false);
    rectTransform.position = worldPos;

    // initialize pointer local-space tracking
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        (RectTransform)rectTransform.parent,
        eventData.position,
        eventData.pressEventCamera,
        out lastLocalPointerPos);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        isDragging = false;
        // sets edited as true if the index has been changed
        if (initialIndex != thisParent.GetSiblingIndex())
            AppManager.Instance.hasEditedOrder = true;

        currDragger = null;
        image.color = transparentDefault;
        // uses the returned index to place itself in the placeholder's spot
        int index = dropHandler.GetIndexAndHide();
        if (index != -1)
        {
            // animates move to the selected location before placing it there
            Vector2 endPos = dropHandler.placeHolder.GetComponent<RectTransform>().anchoredPosition;
            float speed = Vector2.Distance(rectTransform.anchoredPosition, endPos) / DropHandler.RETURN_SPEED;
            Tween.UIAnchoredPosition(rectTransform, endValue: endPos, duration: speed).OnComplete(() =>
            {
                // turns off layout group before reordering
                dropHandler.gridLayoutGroup.enabled = false;
                thisParent.SetParent(dropHandler.targetContainer);
                thisParent.SetSiblingIndex(index);
                dropHandler.gridLayoutGroup.enabled = true;
            });
        }
        else
            Debug.LogWarning("No placeholder found in DropHandler.");
    }
Vector2 initialAnchoredPosition;
    // resets the dragged item to the same initial position
    void ResetDrag()
    {
        isDragging = false;
        currDragger = null;
        image.color = transparentDefault;
        // uses the returned index to place itself in the placeholder's spot
        int index = initialIndex;

        // animates move to the selected location before placing it there
        Vector2 endPos = initialAnchoredPosition;
        float speed = Vector2.Distance(rectTransform.anchoredPosition, endPos) / DropHandler.RETURN_SPEED;
        Tween.UIAnchoredPosition(rectTransform, endValue: endPos, duration: speed).OnComplete(() =>
        {
            // turns off layout group before reordering
            dropHandler.gridLayoutGroup.enabled = false;
            thisParent.SetParent(dropHandler.targetContainer);
            thisParent.SetSiblingIndex(index);
            dropHandler.gridLayoutGroup.enabled = true;
        });
        
    }

    Dragger currDragger;
    // checks if it hits a different dragger
    bool CheckPosition(PointerEventData eventData)
    {
        // Raycasts from this object
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(eventData, results);

        bool isPresent = false;
        if (results.Count < 1) return false;

        foreach (var r in results)
        {
            // ignores nulls
            if (r.gameObject == null) continue;
            // if over the placeholder returns early
            if (r.gameObject == dropHandler.placeHolder) return false;

            if (r.gameObject.TryGetComponent(out Dragger drag))
            {
                // checks if dragger grabbed is not the same as the current one or this one
                if (drag != this && drag != currDragger)
                {
                    // saves the dragger
                    currDragger = drag;
                    break;
                }
                isPresent = true;
            }
        }
        return isPresent;
    }

    // the container must be a direct parent of target for this to work
    Vector2 ClampIntoContainer(RectTransform container, Vector2 desiredAnchoredPos, Vector2 padding)
    {
        float minY = container.rect.yMin;
        float maxY = container.rect.yMax; 

        float clampedY = Mathf.Clamp(desiredAnchoredPos.y, minY, maxY);

        desiredAnchoredPos.y = clampedY;
        // scrolls
        if (desiredAnchoredPos.y <= DropHandler.minY)
            dropHandler.Scroll(Direction.Down);
        else if (desiredAnchoredPos.y >= DropHandler.maxY)
            dropHandler.Scroll(Direction.Up);
        return desiredAnchoredPos;
    }

}