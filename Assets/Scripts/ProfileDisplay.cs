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

    }


}
