using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// do not use with toggle buttons! they will cancel each other out
public class HideOnClickOutside : MonoBehaviour
{
    private GraphicRaycaster raycaster;
    private EventSystem eventSystem;
    CanvasGroupToggle canvasGroupToggle;
    Coroutine coroutine;

    void Start()
    {
        raycaster = AppManager.Instance.appCanvas.GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;
        canvasGroupToggle = GetComponent<CanvasGroupToggle>();

        // subs to on visible and on invisible
        canvasGroupToggle.onVisible.AddListener(Enabled);
        canvasGroupToggle.onInvisible.AddListener(Disabled);
    }

    IEnumerator CheckForClick()
    {
        while (true)
        {
            // Mouse down
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                Check(Mouse.current.position.ReadValue());
            }

            // Touch down
            if (Touchscreen.current != null)
            {
                foreach (var touch in Touchscreen.current.touches)
                {
                    if (touch.press.wasPressedThisFrame)
                    {
                        Check(touch.position.ReadValue());
                        break;
                    }
                }
            }
            yield return null;
        }
    }

    void Enabled()
    {
        coroutine = StartCoroutine(CheckForClick());
    }

    void Disabled()
    {
        StopCoroutine(coroutine);
    }

    private void Check(Vector2 screenPos)
    {
        var ped = new PointerEventData(eventSystem);
        ped.position = screenPos;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(ped, results);

        // If any result is this object or its children do NOT hide
        foreach (var r in results)
        {
            if (r.gameObject.transform == transform ||
                r.gameObject.transform.IsChildOf(transform))
            {
                return;
            }
        }

        // Otherwise hide
        canvasGroupToggle.HideElement();
    }
}
