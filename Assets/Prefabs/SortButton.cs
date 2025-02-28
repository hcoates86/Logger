using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortButton : MonoBehaviour
{
    private Profile profile;
    // the container that has the buttons
    public CanvasGroupToggle sortContainer;

    public void OpenSortBy()
    {
        sortContainer.ShowElement();
    }

    public void SortByCreated()
    {
        profile = AppManager.Instance.currentProfile;

        //sorts and sets the events on the profile
        profile.SortEvents(SortBy.Created);
        // AppManager.Instance.shortProfile.SetEvents(profile);
        AppManager.Instance.fullProfile.SetEvents(profile);
        sortContainer.HideElement();
    }

    public void SortByDue()
    {
        profile = AppManager.Instance.currentProfile;

        //sorts and sets the events on the profile
        profile.SortEvents(SortBy.DueDate);
        AppManager.Instance.fullProfile.SetEvents(profile);
        sortContainer.HideElement();
    }

    public void SortByGiven()
    {
        profile = AppManager.Instance.currentProfile;

        //sorts and sets the events on the profile
        profile.SortEvents(SortBy.GivenDate);
        AppManager.Instance.fullProfile.SetEvents(profile);
        sortContainer.HideElement();
    }

    public void SortByNone()
    {
        profile = AppManager.Instance.currentProfile;

        //sorts and sets the events on the profile
        profile.SortEvents(SortBy.None);
        AppManager.Instance.fullProfile.SetEvents(profile);
        sortContainer.HideElement();
    }
}
