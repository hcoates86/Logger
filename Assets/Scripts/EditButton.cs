using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Faisalman.AgeCalc;


public class EditButton : MonoBehaviour
{
    private ColorBlock cbPressed; 
    private ColorBlock cbNormal;
    public TMP_Text text;
    public Button button;

    private Color initialTextColor;
    private Color pressedTextColor;
    
    private string initial = "Edit";
    private string done = "Finish";
    public bool pressed = false;

    public InputHandler editInput;
    public UploadImage uploadImage;
    public static bool editing;

    public CanvasGroupToggle floatingInput;
    public GameObject nameInput;
    public GameObject dateInput;

    void Start()
    {
        cbPressed = button.colors;
        cbNormal = button.colors;
        //saves the initial colors selected in inspector
        cbPressed.normalColor = cbPressed.pressedColor;
        cbPressed.selectedColor = cbPressed.pressedColor;

        initialTextColor = text.color;
        pressedTextColor = new Color32(233, 233, 233, 255);
    }

    public void ToggleButton()
    {
        if (!pressed)
        {
            pressed = true;
            text.text = done;
            button.colors = cbPressed;
            text.color = pressedTextColor;

            SetEventDeleteButtonsVisible(true);
            AppManager.Instance.ChangeEditable(true);
            SetEditInfo();

        }
        else
        {
            // EditProfile();
            pressed = false;
            text.text = initial;
            button.colors = cbNormal;
            text.color = initialTextColor;

            SetEventDeleteButtonsVisible(false);

            if (editing)
                SubmitEdit();

            AppManager.Instance.ChangeEditable(false);
        }
    }

    // not currently in use. Completely cancels all editing
    public void CancelEdit()
    {
        //on cancel reload profile
        AppManager.Instance.fullProfile.Setup(AppManager.Instance.currentProfile);
        editing = false;
    }

    void SetEventDeleteButtonsVisible(bool show)
    {
        if (AppManager.Instance.currentProfile == null ||
            AppManager.Instance.currentProfile.allEvents.Count < 1) return;

        foreach (EventItem item in AppManager.Instance.currentProfile.allEvents)
        {
            if (show)
            {
                item.deleteButtonCGT.ShowElement();
            }
            else
            {
                item.deleteButtonCGT.HideElement();
            }
        }
    }

    public void EditName()
    {
        // takes the current text on the full profile
        editInput.nameInput.text = AppManager.Instance.fullProfile.nameText.text;
        floatingInput.ShowElement();
        nameInput.SetActive(true);
        dateInput.SetActive(false);
    }

    public void EditBday()
    {
        // takes the current text on the full profile
        // if (AppManager.Instance.fullProfile.dateText.text != string.Empty)
        editInput.dateInput.text = AppManager.Instance.fullProfile.dateText.text;
        // else
        //     editInput.dateInput.text = "";

        floatingInput.ShowElement();
        nameInput.SetActive(false);
        dateInput.SetActive(true);

    }

    // attached to the confirm button on the floating input on full profile
    public void OnConfirm()
    {
        //change the visuals on full profile. don't change actual profile unless submitedit is called
        if (nameInput.activeSelf)
        {
            if (editInput.nameInput.text == null || editInput.nameInput.text == string.Empty)
            {
                AppManager.Instance.error.SetError("Name is required. Cannot leave blank.");
                return;
            }
            AppManager.Instance.fullProfile.nameText.text = editInput.nameInput.text;
        }
        if (dateInput.activeSelf)
        {    
            if (editInput.dateValidator.SubmitDateValidation())
            {
                AppManager.Instance.fullProfile.ageText.text = GetAge(editInput.dateValidator.dateValue);
                AppManager.Instance.fullProfile.dateText.text = editInput.dateValidator.dateValue.ToString("MM/dd/yyyy");
            }
            else return;
        }

        floatingInput.HideElement();
        editing = true;
    }

    // this is called from the second edit button press
    public void SubmitEdit()
    {
        Profile profile = AppManager.Instance.currentProfile;

        profile.named = editInput.nameInput.text;
        profile.birthDate = editInput.dateValidator.dateValue;

        AppManager.Instance.profileEdited = true;

        if (uploadImage.imageUploaded)
        {
            //save the new image
            Debug.Log("attempting to save");
            uploadImage.Upload(AppManager.Instance.currentProfile.id);
        }
        profile.SaveProfile();

        editInput.ClearInput(false);
        editing = false;
    }

    //copy of the one on profile
    string GetAge(DateTime birthDate)
    {
        // if date is default value, treat as null
        if (birthDate == DateTime.MinValue)
            return "";

        // DateTime currentDate = DateTime.Now;

        // since the age calc can't handle future dates, just list as unborn until date
        if (birthDate > DateTime.Now)
        {
            return "Unborn";
            // TODO: "Due in x weeks/months"
        }

        Age age = new Age(birthDate, DateTime.Now);

        string pluralDays;
        string pluralWeeks;
        string pluralMonths;
        string pluralYears;
        if (age.Days == 1)
            pluralDays = "Day";
        else
            pluralDays = "Days";
        if (Mathf.Floor(age.Days / 7) == 1)
            pluralWeeks = "Week";
        else
            pluralWeeks = "Weeks";

        if (age.Months == 1)
            pluralMonths = "Month";
        else
            pluralMonths = "Months";
        if (age.Years == 1)
            pluralYears = "Year";
        else
            pluralYears = "Years";


        if (age.Years < 1)
        {
            if (age.Months < 1)
            {
                return $"{Mathf.Floor(age.Days / 7)} {pluralWeeks} {age.Days} {pluralDays}";

            }



            return $"{age.Months} {pluralMonths} {Mathf.Floor(age.Days / 7)} {pluralWeeks}";
        }

        return $"{age.Years} {pluralYears} {age.Months} {pluralMonths}";
    }

    // needs to be set on entering edit mode or can accidentally save blank info
    void SetEditInfo()
    {
        ProfileDisplay disp = AppManager.Instance.fullProfile;
        disp.nameText.text = AppManager.Instance.currentProfile.named;
        disp.dateText.text = AppManager.Instance.currentProfile.birthDate.ToString("MM/dd/yyyy");
    }

}
