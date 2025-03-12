using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class NewEventHandler : MonoBehaviour
{
    public TMP_InputField titleInput;
    public TMP_InputField notesInput;
    public DateInputValidator startDateValidator;
    public DateInputValidator dueDateValidator;
    public CanvasGroupToggle cgtToHideOnSubmit;

    public Profile profile;

    private EventItem editingItem;

    // for setting inputs directly, not read
    public TMP_InputField startDateInput;
    public TMP_InputField dueDateInput;

    public static string blankDate = "--/--/----";


    public void OnSubmit()
    {
        // receives true if dates are valid (includes null for optional dates)
        if (startDateValidator.SubmitDateValidation() && dueDateValidator.SubmitDateValidation())
        {
            if (editingItem == null)
                CreateEvent(titleInput.text, startDateValidator.dateValue, dueDateValidator.dateValue, notesInput.text);
            else
                EditEvent(editingItem, titleInput.text, startDateValidator.dateValue, dueDateValidator.dateValue, notesInput.text);
        }
        else
        {
            return;
        }

        // editingItem = null;
        ClearInput(true);
    }

    public void CreateEvent(string title, DateTime startDate, DateTime dueDate, string notes)
    {
        AppManager.Instance.eventEdited = true;
        profile = AppManager.Instance.currentProfile;

        GameObject newEvent = Instantiate(AppManager.Instance.eventPrefab, AppManager.Instance.fullProfile.eventContainer);
        EventItem eventItem = newEvent.GetComponentInChildren<EventItem>();
        
        // adds one to events added before setting it as the new event id
        profile.totalEventsAdded++;
        profile.SaveProfile();

        eventItem.profileId = profile.id;
        eventItem.eventId = profile.totalEventsAdded;
        eventItem.title.text = title;
        eventItem.notes.text = notes;

        if (startDate != DateTime.MinValue)
        {
            eventItem.hasStartDate = true;
            eventItem.startDate.text = startDate.ToString("MM/dd/yyyy");
        }
        else
        {
            eventItem.hasStartDate = false;
            eventItem.startDate.text = blankDate;
        }

        if (dueDate != DateTime.MinValue)
        {
            eventItem.hasDueDate = true;
            eventItem.dueDate.text = dueDate.ToString("MM/dd/yyyy");
        }
        else
        {
            eventItem.hasDueDate = false;
            eventItem.dueDate.text = blankDate;
        }

        // displays the things passed in
        eventItem.DisplayOptional(true);


        eventItem.SaveEvent();
        profile.allEvents.Add(eventItem);

        if (AppManager.Instance.currentProfile.allEvents.Count > 1)
                AppManager.Instance.currentProfile.SortByCurrentCriteria();
        AppManager.Instance.shortProfile.Setup(AppManager.Instance.currentProfile);
    }

    // Sets the event to edit
    public void SetEditEvent(EventItem _event)
    {
        editingItem = _event;
        //sets the text to the passed in event values
        titleInput.text = _event.title.text;
        notesInput.text = _event.notes.text;
        // TMP_InputField startInput = startDateValidator.GetComponent<TMP_InputField>();

        // if the text is "--/--/----" sets an empty string instead
        startDateInput.text = _event.startDate.text == blankDate ? string.Empty : _event.startDate.text;
        // TMP_InputField dueInput = dueDateValidator.GetComponent<TMP_InputField>();
        dueDateInput.text = _event.dueDate.text  == blankDate ? string.Empty : _event.dueDate.text;
    }

    //called on submit
    void EditEvent(EventItem _editingItem, string title, DateTime startDate, DateTime dueDate, string notes)
    {
        _editingItem.title.text = title;
        _editingItem.notes.text = notes;

        if (startDate != DateTime.MinValue)
        {
            _editingItem.hasStartDate = true;
            _editingItem.startDate.text = startDate.ToString("MM/dd/yyyy");
        }
        else
        {
            _editingItem.hasStartDate = false;
            _editingItem.startDate.text = blankDate;
        }

        if (dueDate != DateTime.MinValue)
        {
            _editingItem.hasDueDate = true;
            _editingItem.dueDate.text = dueDate.ToString("MM/dd/yyyy");
        }
        else
        {
            _editingItem.hasDueDate = false;
            _editingItem.dueDate.text = blankDate;
        }
        _editingItem.DisplayOptional(true);


        _editingItem.SaveEvent();

    }

    public void ClearInput(bool andHide)
    {
        titleInput.text = "";
        notesInput.text = "";
        startDateInput.text = "";
        dueDateInput.text = "";
        profile = null;
        editingItem = null;

        // hides errors
        AppManager.Instance.error.OkButton();

        if (andHide)
            cgtToHideOnSubmit.HideElement();
    }

}
