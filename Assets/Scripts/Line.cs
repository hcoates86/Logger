using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Line : MonoBehaviour
{
    public string title;
    public string notes;
    //for when something was given, and when it's due next
    // should be able to edit these alone. Press edit button then click? If edit button has been pressed, then allow edit
    public DateTime startDate, dueDate;
    public bool hasStartDate, hasDueDate;
}
