using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using Unity.Notifications.Android;

public class EventItem : MonoBehaviour
{
    public CanvasGroupToggle deleteButtonCGT;
    public CanvasGroupToggle expandedContainer;
    public CanvasGroupToggle shortBackground;
    // corresponds to the profile id
    public int profileId;
    // to be loaded in order
    public int eventId;
    public TMP_Text title;
    public TMP_Text notes;
    public TMP_Text startDate, dueDate;

    public CanvasGroupToggle startDateContainer;
    public CanvasGroupToggle dueDateContainer;

    public bool isFavorite = false;
    public Image favoriteStar;

    public bool hasStartDate;
    public bool hasDueDate;
    // the notes scroll bar
    public Scrollbar scrollbar;

    public Image bgImage;
    Color normalColor;
    Color editableColor = new Color32(163, 255, 163, 255);


    void Start()
    {
        ResetScrollbar();
        if (bgImage != null)
        normalColor = bgImage.color;
    }

    // sets the visual changes when edit event is on
    public void SetEditChanges(bool _editable)
    {
        deleteButtonCGT.ShowElement(_editable);

        if (_editable)
        {
            bgImage.color = editableColor;
        }
        else
        {
            bgImage.color = normalColor;
        }
    }

    // displays optional items
    // bool asks if full view is on to show expanded note view
    public void DisplayOptional()
    {
        startDateContainer.ShowElement(hasStartDate);
        dueDateContainer.ShowElement(hasDueDate);
    }

    public void ResetScrollbar()
    {
        if (scrollbar != null)
            scrollbar.value = 1;
    }


    public void ToggleFavorite()
    {
        isFavorite = !isFavorite;
        SaveEvent();
        ChangeFavorite();
    }

    public void ChangeFavorite()
    {
        if (isFavorite)
        {
            favoriteStar.sprite = AppManager.Instance.starFilled;
        }
        else
        {
            favoriteStar.sprite = AppManager.Instance.starHollow;
        }
    }

    public void SaveEvent()
    {
        AppManager.Instance.eventEdited = true;
        EventData data = new EventData();

        data.profileId = profileId;
        data.eventId = eventId;
        data.title = title.text;
        if (notes != null)
            data.notes = notes.text;
        else
            data.notes = string.Empty;
        data.startDate = startDate.text;
        data.dueDate = dueDate.text;
        data.hasStartDate = hasStartDate;
        data.hasDueDate = hasDueDate;
        data.isFavorite = isFavorite;

        string json = JsonUtility.ToJson(data);

        string directoryPath = $"{Application.persistentDataPath}/{profileId}";
        if (!Directory.Exists(directoryPath))
        {
            // Create the directory
            Directory.CreateDirectory(directoryPath);
        }
        string filePath = $"{directoryPath}/{eventId}.json";

        File.WriteAllText(filePath, json);
    }

    // deletes an event. Doesn't need confirmation. Located on onclick of eventitem delete button
    public void DeleteEvent()
    {
        // deletes self from saved events
        string path = $"{Application.persistentDataPath}/{profileId}/{eventId}.json";
        AppManager.Instance.DeleteItemAtPath(path);

        //TODO: delete notifications too


        Profile profile = AppManager.Instance.FindProfile(profileId);
        // removes self from profile's list of events
        profile.allEvents.Remove(this);

        AppManager.Instance.FadeAndDestroy(gameObject);
        //reorganize the short events list
        AppManager.Instance.eventEdited = true;
        Audio.Instance.PlayClip(Audio.Instance.deleteEvent);
    }

    // brings up the event modal. Attached to eventitem prefab's onclick
    public void StartEditEvent()
    {
        //if can't edit opens the popup
        if (!AppManager.Instance.canEdit)
        {
            OpenPopup();
            return;
        }

        // opens modal and sets event info
        NewEventHandler handler = AppManager.Instance.eventModal;
        handler.SetEditEvent(this);
        handler.cgtToHideOnSubmit.ShowElement();
    }

    public void SetEventInfo(EventItem eventItem)
    {
        title.text = eventItem.title.text;
        // not all events have notes text areas
        if (notes != null && eventItem.notes != null)
        {
            notes.text = eventItem.notes.text;
            ResetScrollbar();
        }
        startDate.text = eventItem.startDate.text;
        dueDate.text = eventItem.dueDate.text;
    }

    public void SetEventInfo(FavoriteEventDisplay eventItem)
    {
        title.text = eventItem.title.text;
        if (notes != null)
        {
            notes.text = eventItem.note;
            ResetScrollbar();
        }
        startDate.text = eventItem.startDate.text;
        dueDate.text = eventItem.dueDate.text;
    }

// placed on event onclick
    public void OpenPopup()
    {
        AppManager.Instance.eventPopup.GetComponent<EventItem>().SetEventInfo(this);
        AppManager.Instance.eventPopup.ShowElement();
    }
}

[System.Serializable]
public class EventData
{
    // corresponds to the profile id
    public int profileId;
    public int eventId;
    public string title;
    public string notes;
    public string startDate;
    public string dueDate;
    public bool hasStartDate; 
    public bool hasDueDate;
    public bool isFavorite;
}

public enum DataType
{
    ProfileID, EventID, Title, Notes, StartDate, DueDate, HasStart, HasDue, IsFavorite, All, Edit
}

