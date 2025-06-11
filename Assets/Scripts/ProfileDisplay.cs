using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileDisplay : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text ageText;
    public TMP_Text dateText;
    // image to set the sprite photo to
    public Image image;

    public bool useThumbnail = false;

    public RectTransform eventContainer;
    public FavoriteEventDisplay[] favoriteDisplays = new FavoriteEventDisplay[3];

    public void Setup(Profile profile)
    {
        nameText.text = profile.named;
        ageText.text = profile.Age;

        if (dateText != null)
        {
            if (profile.birthDate != DateTime.MinValue)
                dateText.text = profile.birthDate.ToString("MM/dd/yyyy");
            else
                dateText.text = "--/--/----";
        }

        //uses as shortcut to check if it's the small profile item
        if (useThumbnail)
        {
            // shows only the higher age part for the profile item
            if (profile.Age != string.Empty)
            {
                string[] ageSplit = profile.Age.Split(" ");
                string shortAge = $"{ageSplit[0]} {ageSplit[1]}";
                ageText.text = shortAge;
            }
        }

        if (image != null)
        {
            if (profile.profileImage != null)
            {
                if (useThumbnail)
                {
                    image.sprite = profile.thumbnail;
                }
                else
                    image.sprite = profile.profileImage;
            }
            else
            {
                image.sprite = AppManager.Instance.defaultImage;
            }

        }

        // Sets the archive status object active or not depending on state
        AppManager.Instance.archivedStatus.SetActive(profile.isArchived);
        //TEST
        AppManager.Instance.archivedStatusFull.SetActive(profile.isArchived);

        if (profile.allEvents.Count > 0)
        {
            SetEvents(profile);
        }
        else
            ClearEvents();
    }

    void ClearEvents()
    {
        //clears the favorite events on the short profile
        // checks for null to see if it's short profile instead of length due to fixed array size
        if (favoriteDisplays[0] != null)
        {
            for (int i = 0; i < 3; i++)
            {
                favoriteDisplays[i].Setup(null, false);
            }
        }
    }

    public void SetEvents(Profile profile)
    {
        // sets the events on the full profile
        if (eventContainer != null && profile.allEvents.Count > 0)
        {
            foreach (EventItem item in profile.allEvents)
            {
                item.gameObject.SetActive(true);
                item.transform.SetParent(eventContainer, false);
                
            }
        }

        //sets the events on the short profile
        // checks for null to see if it's short profile instead of length due to fixed array size
        if (favoriteDisplays[0] != null)
        {
            for (int i = 0; i < 3; i++)
            {
                // if the item count is higher than i displays it (errors checking for null), otherwise hides display
                if (profile.allEvents.Count > i)
                {
                    favoriteDisplays[i].Setup(profile.allEvents[i], true);
                }
                else
                {
                    favoriteDisplays[i].Setup(null, false);
                }
            }
        }
    }


}
