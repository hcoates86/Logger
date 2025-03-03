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
    // for ages, like "5", "12"
    public TMP_InputField ageInput;

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

        // if a birthdate hasn't been input, uses the age
        if (dateTime == DateTime.MinValue && ageInput != null && ageInput.text != string.Empty)
        {
            int _age = int.Parse(ageInput.text);
            DateTime today = DateTime.Now;
            today = today.AddYears(-_age);
            dateTime = today;
        }

        AppManager.Instance.CreateProfile(id, nameInput.text, dateTime, uploadImage.imageUploaded, true);
        // cgtToHideOnSubmit.HideElement();
        ClearInput(true);

    }

    public void ClearInput(bool andHide)
    {
        if (nameInput != null)
            nameInput.text = string.Empty;
        if (dateInput != null)
            dateInput.text = string.Empty;
        if (image != null)
            image.sprite = null;
        if (ageInput != null)
            ageInput.text = string.Empty;

        // hides errors
        AppManager.Instance.error.OkButton();

        uploadImage.imageUploaded = false;

        if (andHide)
            cgtToHideOnSubmit.HideElement();
            

    }


}
