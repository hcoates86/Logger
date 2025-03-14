using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// a generic method to sort. Attach one per button.
public class SortProfiles : MonoBehaviour
{
    // set in the inspector
    public SortBy sort;

    // the container that has the buttons you're clicking
    public CanvasGroupToggle sortContainer;

    // attach to button
    public void OnClick()
    {
        // passes in the sortby set in the inspector
        AppManager.Instance.SortProfiles(sort);
        sortContainer.HideElement();
    }
}
