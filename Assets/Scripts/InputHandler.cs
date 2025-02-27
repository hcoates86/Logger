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
    public GameObject inputParentToDeactivate;
    public bool deactivateParentOnSubmit = false;
    //save image to /create directory id

    public void OnSubmit()
    {
        // checks if the object is active (accepting input)
        if (nameInput.gameObject.activeInHierarchy)
        {
            // AppManager.Instance.currentProfile

        }
        if (dateValidator.gameObject.activeInHierarchy)
        {
            DateTime dateTime;
            if (dateValidator.SubmitDateValidation())
                dateTime = dateValidator.dateValue;

        }

        if (deactivateParentOnSubmit)
        {
            inputParentToDeactivate.SetActive(false);
        }
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

        if (uploadImage.imageUploaded)
        {
            uploadImage.Upload(AppManager.Instance.allProfiles.Count + 1);
        }


        AppManager.Instance.CreateProfile(nameInput.text, dateTime, uploadImage.imageUploaded);

        nameInput.text = string.Empty;
        dateInput.text = string.Empty;
        image.sprite = null;

        inputParentToDeactivate.SetActive(false);
    }


}
