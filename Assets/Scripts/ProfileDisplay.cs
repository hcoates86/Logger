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

    public Transform eventContainer;
    public FavoriteEventDisplay[] favoriteDisplays = new FavoriteEventDisplay[3];

    // private bool canEdit = false;


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


        if (profile.profileImage != null)
        {
            if (useThumbnail)
                image.sprite = profile.thumbnail;
            else
                image.sprite = profile.profileImage;
        }
        else
        {
            image.sprite = AppManager.Instance.defaultImage;
        }

        if (profile.allEvents.Count > 0)
        {
            SetEvents(profile);
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
            for (int i = 0; i < profile.allEvents.Count; i++)
            {
                // if the item exists displays it, otherwise hides display
                if (profile.allEvents[i] != null)
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
