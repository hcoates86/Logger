using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System;

public class InputHandler : MonoBehaviour
{
    public TMP_InputField nameInput;
    public DateInputValidator dateValidator;
    public string imagePath;
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
        if (uploadImage.imageUploaded)
        {
            uploadImage.Upload(AppManager.Instance.allProfiles.Count + 1);
        }

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



        inputParentToDeactivate.SetActive(false);
    }


}
