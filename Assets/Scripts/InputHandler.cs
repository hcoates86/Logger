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
    public DateInputValidator endDateValidator;
    public TMP_InputField dateInput;
    public TMP_InputField endDateInput;
    // for ages, like "5", "12"
    public TMP_InputField ageYearsInput;
    public TMP_InputField ageMonthsInput;
    public TMP_InputField ageWeeksInput;
    public Toggle isDead;

    public Image image;
    public UploadImage uploadImage;

    public CanvasGroupToggle cgtToHideOnSubmit;

    private bool debug = false;


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
        else return;

        DateTime endDateTime;
        if (endDateValidator.SubmitDateValidation())
            endDateTime = endDateValidator.dateValue;
        else return;

        int id = AppManager.Instance.CreateNewId();
        if (uploadImage.imageUploaded)
        {
            uploadImage.Upload(id);
        }

        string savedAge = string.Empty;
        // if a birthdate hasn't been input, uses the age to calculate it
        if (dateTime == DateTime.MinValue && ageYearsInput != null)
        {
            string totalAge = string.Empty;
            int _years = 0;
            int _months = 0;
            int _weeks = 0;
            DateTime today = DateTime.Now;

            if (ageYearsInput.text != string.Empty)
            {
                _years = int.Parse(ageYearsInput.text);
                if (_years > 0)
                {
                    totalAge = $"{ageYearsInput.text} Year{(_years == 1 ? "" : "s")}";
                    // calculates the birthday
                    today = today.AddYears(-_years);
                }
            }

            if (ageMonthsInput.text != string.Empty)
            {
                _months = int.Parse(ageMonthsInput.text);
                if (_months > 0)
                {
                    // Adds a space if the string has text already
                    totalAge += $"{(totalAge.Length > 0 ? " ":"")}{ageMonthsInput.text} Month{(_months == 1 ? "" : "s")}";
                    today = today.AddMonths(-_months);
                }
            }

            if (ageWeeksInput.text != string.Empty)
            {
                _weeks = int.Parse(ageWeeksInput.text);
                if (_weeks > 0)
                {
                    // Adds a space if the string has text already
                    totalAge += $"{(totalAge.Length > 0 ? " ":"")}{ageWeeksInput.text} Week{(_weeks == 1 ? "" : "s")}";
                    int _days = _weeks * 7;
                    today = today.AddDays(-_days);
                }
            }

            dateTime = today;
// if age is set and profile is dead, saves age and clears birthdate
            if (isDead.isOn)
            {
                dateTime = DateTime.MinValue;
                savedAge = totalAge;
            }
        }

        AppManager.Instance.CreateProfile(id, nameInput.text, dateTime, endDateTime, 0, SortBy.None, uploadImage.imageUploaded, true, 500, false, savedAge);

        ClearInput(true);
    }

    public void ClearInput(bool andHide)
    {
        if (debug)
        {
            Debug.Log("Cleared input:");
            Debug.Log($"Name: {nameInput?.text}");
            Debug.Log($"Date: {dateInput?.text}");
            Debug.Log($"Age: {ageYearsInput?.text}");
        }

        if (nameInput != null)
            nameInput.text = string.Empty;
        if (dateInput != null)
            dateInput.text = string.Empty;
        if (endDateInput != null)
            endDateInput.text = string.Empty;
        if (image != null)
            image.sprite = null;
        if (ageYearsInput != null)
            ageYearsInput.text = string.Empty;
        if (ageMonthsInput != null)
            ageMonthsInput.text = string.Empty;
        if (ageWeeksInput != null)
            ageWeeksInput.text = string.Empty;

        if (isDead != null)
            isDead.isOn = false;

        // hides errors
        AppManager.Instance.error.OkButton();

        if (uploadImage != null)
            uploadImage.imageUploaded = false;

        if (andHide)
        {
            cgtToHideOnSubmit.HideElement();
            if (debug)
            {
                Debug.Log($"Hid {cgtToHideOnSubmit.gameObject.name}");

            }
        }
            

    }


}
