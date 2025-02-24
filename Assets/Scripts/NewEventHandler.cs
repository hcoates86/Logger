using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewEventHandler : MonoBehaviour
{
    public TMP_InputField titleInput;
    public TMP_InputField notesInput;
    public DateInputValidator startDateValidator;
    public DateInputValidator dueDateValidator;


    void OnSubmit()
    {
        // receives true if dates are valid (includes null for optional dates)
        if (startDateValidator.SubmitDateValidation() && dueDateValidator.SubmitDateValidation())
        {
            // startDateValidator.dateValue;
        }
        else
        {
            return;
        }
        
    }


}
