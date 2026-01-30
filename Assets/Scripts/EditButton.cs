using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Faisalman.AgeCalc;


public class EditButton : MonoBehaviour
{
    public bool pressed = false;

    public InputHandler editInput;
    public UploadImage uploadImage;

    public CanvasGroupToggle floatingInput;
    public CanvasGroupToggle nameInput;
    public CanvasGroupToggle dateInput;

    public GameObject editEventTip;
[SerializeField]
    private bool wasDateChanged = false;
    public static bool profileImageReplaced = false;
    // the bg panel that will also be able to close edit events
    public GameObject panel;

    void ToggleEditVisibility(bool visible)
    {
        editEventTip.SetActive(visible);
        panel.SetActive(visible);
    }

    // not currently in use. Completely cancels all editing
    public void CancelEdit()
    {
        //on cancel reload profile
        AppManager.Instance.fullProfile.Setup(AppManager.Instance.currentProfile);
        wasDateChanged = false;
    }

    public void SetEventDeleteButtonsVisible(bool show)
    {
        if (AppManager.Instance.currentProfile == null ||
            AppManager.Instance.currentProfile.allEvents.Count < 1) return;

        foreach (EventItem item in AppManager.Instance.currentProfile.allEvents)
        {            
                item.SetEditChanges(show);
        }
    }

    public static bool isEventDeleteVisible = false;

    // on the event button and panel once edit event is on
    public void ShowEventDeleteToggle()
    {
        isEventDeleteVisible = !isEventDeleteVisible;
        ToggleEditVisibility(isEventDeleteVisible);
        SetEventDeleteButtonsVisible(isEventDeleteVisible);
        AppManager.Instance.canEdit = !AppManager.Instance.canEdit;
    }

    public void EditName()
    {
        // takes the current text on the full profile
        editInput.nameInput.text = AppManager.Instance.fullProfile.nameText.text;
        floatingInput.ShowElement();
        nameInput.ShowElement(true);
        dateInput.ShowElement(false);
    }

    public void EditBday()
    {
        // takes the current text on the full profile
        if (AppManager.Instance.fullProfile.dateText.text == NewEventHandler.blankDate)
            editInput.dateInput.text = string.Empty;
        else
            editInput.dateInput.text = AppManager.Instance.fullProfile.dateText.text;

        floatingInput.ShowElement();
        nameInput.ShowElement(false);
        dateInput.ShowElement(true);
    }

    // attached to the confirm button on the floating input on full profile
    public void OnConfirm()
    {
        //change the visuals on full profile
        if (nameInput.IsElementVisible())
        {
            if (editInput.nameInput.text == null || editInput.nameInput.text == string.Empty)
            {
                AppManager.Instance.error.SetError("Name is required. Cannot leave blank.");
                return;
            }
            AppManager.Instance.fullProfile.nameText.text = editInput.nameInput.text;
        }
        if (dateInput.IsElementVisible())
        {
            wasDateChanged = true;
            if (editInput.dateValidator.SubmitDateValidation())
            {
                // if the date was blank place blank text
                if (editInput.dateValidator.dateValue == DateTime.MinValue)
                {
                    AppManager.Instance.fullProfile.dateText.text = NewEventHandler.blankDate;
                    AppManager.Instance.fullProfile.ageText.text = "";
                }
                else
                {
                    AppManager.Instance.fullProfile.ageText.text = AppManager.Instance.GetAge(editInput.dateValidator.dateValue);
                    AppManager.Instance.fullProfile.dateText.text = editInput.dateValidator.dateValue.ToString("MM/dd/yyyy");

                }
            }
            else return;
        }
        // saves submission
        SubmitEdit();
        floatingInput.HideElement();
    }

    public void SubmitEdit()
    {
        Profile profile = AppManager.Instance.currentProfile;
        
        if (editInput.nameInput.text != string.Empty)
            profile.named = editInput.nameInput.text;

        if (wasDateChanged)
        {
            profile.birthDate = editInput.dateValidator.dateValue;
            // if the date is edited when the profile is archived, changes the archived age
            if (profile.isArchived)
            {
                profile.archivedAge = AppManager.Instance.GetAge(profile.birthDate);
            }
        }
    
        if (uploadImage.imageUploaded)
        {
            profileImageReplaced = true;
            //save the new image
            uploadImage.Upload(AppManager.Instance.currentProfile.id);
            uploadImage.imageUploaded = false;
        }
        profile.SaveProfile();
        AppManager.Instance.ProfileEdited = true;

        editInput.ClearInput(false);
    }

    // needs to be set on entering edit mode or can accidentally save blank info
    void SetEditInfo()
    {
        editInput.nameInput.text = AppManager.Instance.fullProfile.nameText.text;

        if (AppManager.Instance.currentProfile.birthDate != DateTime.MinValue)
            editInput.dateInput.text = AppManager.Instance.fullProfile.dateText.text;
        else
            editInput.dateInput.text = string.Empty;
    }
}
