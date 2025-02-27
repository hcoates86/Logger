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

    public CanvasGroupToggle editImage;
    public CanvasGroupToggle editName;
    public CanvasGroupToggle editBirthdate;

    private bool canEdit = false;


    public void Setup(Profile profile)
    {
        nameText.text = profile.named;
        ageText.text = profile.Age;
        dateText.text = profile.birthDate.ToString("MM/dd/yyyy");

        image.sprite = profile.profileImage;

    }
}
