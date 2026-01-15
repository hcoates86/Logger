using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CanvasGroupToggle : MonoBehaviour
{
    public CanvasGroup element;
    public bool fadeOut = false;
    public bool fadeIn = false;
    public float fadeOutSpeed;
    public float fadeInSpeed;
    public bool startHidden = true;
    // hides num of seconds after showing
    public float hideAfterSeconds;

    // good for items that only need to be faded when destroyed. Can be set right before deletion also
    public bool destroyAfterFade = false;
    public float destroyAfterFadeDelay = 0.3f;

    public bool addSelf = true;

    // Global registry of active CanvasGroupToggles, added only if hideOnEsc is true
    public static readonly HashSet<CanvasGroupToggle> cgtHash = new HashSet<CanvasGroupToggle>();
    public bool hideOnEsc = false;
    public bool fixLayoutOnVisible = false;
    RectTransform rect;
    float fixDelay = 0.025f;

    public UnityEvent onVisible = new UnityEvent();
    public UnityEvent onInvisible = new UnityEvent();

    void Awake()
    {
        AddCanvasGroup();

        if (fixLayoutOnVisible)
            rect = GetComponent<RectTransform>();

        //sets default fade speeds at start
        if (fadeOutSpeed == 0)
            fadeOutSpeed = 15f;
        if (fadeInSpeed == 0)
            fadeInSpeed = 15f;

        if (destroyAfterFadeDelay == 0)
            destroyAfterFadeDelay = 0.3f;

        if (startHidden)
        {
            HideElementImmediate();
        }
        else
        {
            ShowElementImmediate();
        }

        AddSelfToList();
    }

    // called in editor when component is added
    void Reset()
    {
        AddCanvasGroup();
    }

    void AddCanvasGroup()
    {
        if (addSelf && element == null)
        {
            // Checks if CanvasGroup is present and adds it if not.
            if (!TryGetComponent<CanvasGroup>(out element))
            {
                element = gameObject.AddComponent<CanvasGroup>();
            }
        }
    }

    public void ToggleShow()
    {
        if (IsElementVisible())
            HideElement();
        else
            ShowElement();

    }

    public void ShowElement()
    {
        if (fadeIn)
            StartCoroutine(FadeElement(false));
        else
        {
            ShowElementImmediate();
        }
        if (hideAfterSeconds > 0)
        {
            StartCoroutine(HideAfterDelay());
        }
    }

    public void ShowElement(bool show)
    {
        if (show)
            ShowElement();
        else
            HideElement();
    }

    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSecondsRealtime(hideAfterSeconds);
        HideElement();
    }

    public void HideElement()
    {
        element.interactable = false;
        element.blocksRaycasts = false;

        if (fadeOut)
            StartCoroutine(FadeElement(true));
        else
        {
            element.alpha = 0;
            Invisible();
        }
    }

    // if there's a delay, waits before hiding it
    public void HideElement(float delay)
    {
        element.interactable = false;
        element.blocksRaycasts = false;

        StartCoroutine(HideDelayed(delay));
    }

    IEnumerator HideDelayed(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        HideElement();
    }

    // Bypasses any fadeouts set.
    public void HideElementImmediate()
    {
        element.alpha = 0;
        element.interactable = false;
        element.blocksRaycasts = false;
        Invisible();
    }

    // Bypasses any fadeouts set.
    public void ShowElementImmediate()
    {
        if (fixLayoutOnVisible)
        {
            StartCoroutine(WaitForFrameThenShowElement());
        }
        else
        {
            Visible();
            element.alpha = 1;
            element.interactable = true;
            element.blocksRaycasts = true;
        }
    }

    IEnumerator WaitForFrameThenShowElement()
    {
        Visible();
        // changes these first as some UI changes when interactable is changed
        element.interactable = true;
        element.blocksRaycasts = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        // waits a short time
        yield return new WaitForSecondsRealtime(fixDelay);
        element.alpha = 1;

    }

    // bool controls whether fading in or out
    IEnumerator FadeElement(bool fadingOut)
    {
        if (fadingOut)
        {
            element.interactable = false;
            element.blocksRaycasts = false;
            
            while (element != null && element.alpha > 0)
            {
                element.alpha -= 0.1f * fadeOutSpeed * Time.unscaledDeltaTime;
                yield return null;
            }
            Invisible();
            if (destroyAfterFade)
            {
                Debug.Log($"Destroying {gameObject.name}");

                yield return new WaitForSecondsRealtime(destroyAfterFadeDelay);
                Destroy(gameObject);
            }
        }
        else
        {
            Visible();
            // marks as dirty
            if (fixLayoutOnVisible)
                LayoutRebuilder.ForceRebuildLayoutImmediate(rect);

            while (element != null && element.alpha < 1)
            {
                element.alpha += 0.1f * fadeInSpeed * Time.unscaledDeltaTime;
                yield return null;
            }

            element.interactable = true;
            element.blocksRaycasts = true;
        }
    }

    public bool IsElementVisible()
    {
        if (element.alpha == 0)
            return false;
        else return true;
    }

    public void Visible()
    {
        if (onVisible != null)
            onVisible.Invoke();

    }

    public void Invisible()
    {
        if (onInvisible != null)
            onInvisible.Invoke();
    }

    // conducts a check before hiding
    public void HideIfVisibile()
    {
        if (IsElementVisible())
            HideElement();
    }

    // intended for popups
    void AddSelfToList()
    {
        if (hideOnEsc)
        {
            //adds self to a list of CanvasGroupToggles that will loop through and hide (in order? order in hierarchy probs) when esc is pressed
            cgtHash.Add(this);
        }
    }

    // this needs to be on a game manager to not run multiple times
    // removes in order from top to bottom
    // void HideFromVisible()
    // {
    //     int currentMax = 0;
    //     int currentIndex;
    //     CanvasGroupToggle currentItem = null;

    //     if (cgtHash.Count > 0)
    //     {
    //         foreach (CanvasGroupToggle item in cgtHash)
    //         {
    //             if (item.IsElementVisible())
    //             {
    // // hides the topmost item first
    //                 currentIndex = item.transform.GetSiblingIndex();
    //                 if (currentIndex > currentMax)
    //                 {
    //                     currentMax = currentIndex;
    //                     currentItem = item;
    //                 }
    //             }
    //         }
    //     }
    //     if (currentItem != null)
    //         currentItem.HideElement();
    // }
    // hides all
    //     void HideFromVisible()
    // {
    //     if (CanvasGroupToggle.cgtHash.Count > 0)
    //     {
    //         foreach (CanvasGroupToggle item in CanvasGroupToggle.cgtHash)
    //         {
    //             item.HideIfVisibile();
    //         }
    //     }
    // }


}
