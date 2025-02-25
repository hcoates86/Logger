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

    public void OnSubmit()
    {
        // checks if the object is active (accepting input)
        if (nameInput.gameObject.activeInHierarchy)
        {
            // AppManager.Instance.currentProfile

        }
        if (dateValidator.gameObject.activeInHierarchy)
        {

        }
    }


}
