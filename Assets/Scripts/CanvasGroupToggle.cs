using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float destroyAfterFadeDelay = 0;

    void Awake()
    {
        // if element isn't set, attempts to get the canvas group on the gameobject it's attached to
        if (element == null)
        {
            element = GetComponent<CanvasGroup>();
        }
        //sets default fade speeds at start
        if(fadeOutSpeed == 0)
            fadeOutSpeed = 15f;
        if(fadeInSpeed == 0)
            fadeInSpeed = 15f;

        if (startHidden)
        {
            HideElementImmediate();
        }
        else
        {
            ShowElementImmediate();
        }
    }

    public void ToggleShow()
    {
        if (element.alpha == 1)
            HideElement();
        else
            ShowElement();
            
    }

    public void ShowElement()
    {
        if (fadeIn)
            StartCoroutine(FadeElement(false));
        else
            element.alpha = 1;
            
        element.interactable = true;
        element.blocksRaycasts = true;

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
        yield return new WaitForSeconds(hideAfterSeconds);
        HideElement();

    }

    public void HideElement()
    {
        if (fadeOut)
            StartCoroutine(FadeElement(true));
        else
            element.alpha = 0;
        
        element.interactable = false;
        element.blocksRaycasts = false;
    }

    public void HideElementImmediate()
    {
        element.alpha = 0;
        element.interactable = false;
        element.blocksRaycasts = false;
    }

    public void ShowElementImmediate()
    {
        element.alpha = 1;
        element.interactable = true;
        element.blocksRaycasts = true;
    }

    // bool controls whether fading in or out
    IEnumerator FadeElement(bool fadingOut)
    {
        if (fadingOut)
        {
            while (element != null && element.alpha > 0)
            {
                element.alpha -= 0.1f * fadeOutSpeed * Time.deltaTime;
                yield return null;
            }
            if (destroyAfterFade)
            {
                yield return new WaitForSeconds(destroyAfterFadeDelay);
                Destroy(gameObject);
            }
        }
        else
        {
            while (element != null && element.alpha < 1)
            {
                element.alpha += 0.1f * fadeInSpeed * Time.deltaTime;
                yield return null;
            }

        }
    }


}
