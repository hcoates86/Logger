using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FavoriteEventDisplay : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text startDate, dueDate;
    public CanvasGroupToggle canvasGroupToggle;
    public bool displaySelf;
    public string note;

    public void Setup(EventItem _event, bool display)
    {
        if (!display)
        {
            canvasGroupToggle.HideElement();
            return;
        }

        title.text = _event.title.text;
        if (_event.hasStartDate)
            startDate.text = _event.startDate.text;
        else
            startDate.text = "";
        if (_event.hasDueDate)
            dueDate.text = _event.dueDate.text;
        else
            dueDate.text = "";
        if (_event.notes != null)
        note = _event.notes.text;

        canvasGroupToggle.ShowElement();
    }

//onclick
    public void OpenPopup()
    {
        AppManager.Instance.eventPopup.GetComponent<EventItem>().SetEventInfo(this);
        AppManager.Instance.eventPopup.ShowElement();
    }
}
