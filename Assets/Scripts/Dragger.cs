using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Dragger : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    Canvas canvas;
    // cloudy image over the draggable that activates on drag
    Image image;

    Color transparentDefault;
    Color draggedColor = new Color32(255, 255, 255, 92);

    public RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        canvas = AppManager.Instance.appCanvas;
        image.color = transparentDefault;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        // changes image color to cloudy when dragging
        image.color = draggedColor;
        // spawns a placeholder that will be moved around

    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        image.color = transparentDefault;
    }

    void IDropHandler.OnDrop(PointerEventData eventData)
    {
        AppManager.Instance.hasEditedOrder = true;
        
    }


}