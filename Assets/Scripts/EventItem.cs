using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EventItem : MonoBehaviour
{
    // corresponds to the profile id
    public int profileId;
    // to be loaded in order
    public int eventId;
    public TMP_Text title;
    public TMP_Text notes;
    public TMP_Text startDate, dueDate;

    public GameObject startDateContainer;
    public GameObject dueDateContainer;

    void SetData()
    {
        // // save it to be able to edit it later
        // savedEvent = _event;

        // title.text = _event.title;
        // notes.text = _event.notes;
        // if (_event.hasStartDate)
        //     startDate.text = _event.startDate;
        // if (_event.hasDueDate)
        //     dueDate.text = _event.dueDate;

    }
}
