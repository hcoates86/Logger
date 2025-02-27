using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    public TMP_InputField nameInput;
    public DateInputValidator dateValidator;
    public TMP_InputField dateInput;
    // public string imagePath;
    public Image image;
    public UploadImage uploadImage;

    public CanvasGroupToggle cgtToHideOnSubmit;

    //save image to /create directory id

    public void OnSubmit()
    {



    }

    public void SubmitNewProfile()
    {
        if (nameInput.text == null || nameInput.text == string.Empty)
        {
            AppManager.Instance.error.SetError("A name is required to create a profile.");
            return;
        }

        DateTime dateTime;
        if (dateValidator.SubmitDateValidation())
            dateTime = dateValidator.dateValue;
        else
            return;

        int id = AppManager.Instance.CreateNewId();
        if (uploadImage.imageUploaded)
        {
            uploadImage.Upload(id);
        }


        AppManager.Instance.CreateProfile(nameInput.text, dateTime, uploadImage.imageUploaded, id);
        cgtToHideOnSubmit.HideElement();
        ClearInput();

    }

    void ClearInput()
    {
        if (nameInput != null)
            nameInput.text = string.Empty;
        if (dateInput != null)
            dateInput.text = string.Empty;
        if (image != null)
            image.sprite = null;

        // hides errors
        AppManager.Instance.error.OkButton();

        uploadImage.imageUploaded = false;

    }


}
