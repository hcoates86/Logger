using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// fills the container with info from all profiles
public class ViewList : MonoBehaviour
{
    public RectTransform container;
    public ListItem listItem;

    void Start ()
    {
        FillList();

        // subscribes to the event
        AppManager.Instance.OnProfileEdit += FillList;
    }

    public void FillList()
    {
        // if (AppManager.Instance.allProfiles.Count == 0) return;

        ClearChildren(container);

        foreach (Profile profile in AppManager.Instance.allProfiles)
        {
            ListItem currItem = Instantiate(listItem, container);
            currItem.Setup(profile);
        }

        if (Options.showArchivedInList)
        {
            foreach (Profile profile in AppManager.Instance.allArchivedProfiles)
            {
                ListItem currItem = Instantiate(listItem, container);
                currItem.Setup(profile);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(container);
    }

    public void ClearChildren(Transform objectTrans)
    {
        int i = 0;

        //Array to hold all child obj
        GameObject[] allChildren = new GameObject[objectTrans.childCount];

        //Find all child obj and store to that array
        foreach (Transform child in objectTrans)
        {
            allChildren[i] = child.gameObject;
            i += 1;
        }

        //Now destroy them
        foreach (GameObject child in allChildren)
        {
            Destroy(child.gameObject);
        }

    }
}
