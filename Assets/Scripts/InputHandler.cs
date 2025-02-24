using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System;

public class InputHandler : MonoBehaviour
{
    // do not set future dates as invalid as it could be a future litter
    public TMP_InputField dateInput;
    public TMP_InputField nameInput;

    private int previousLength, newLength;
    private bool slashPlaced = false;
    private string oldInput = string.Empty;
    private string value;
    private int maxDateChars = 10;

    public void ReadDateInput()
    {
        value = dateInput.text;
        // if it passes validation, check other things
        if (RestrictDateInput(value))
        {
            HandleDate(value);

        }

    }

    // adds / to dates
    void HandleDate(string value)
    {
        // string value = dateInput.text;
        newLength = value.Length;

        // if a slash was placed last, check if the next character is also a slash and remove it
        if (slashPlaced == true)
        {
            if (value[value.Length - 1] == '/' && value[value.Length - 2] == '/')
            {
                value = value.Remove(value.Length - 1);
                dateInput.text = value;
            }
        }
        slashPlaced = false;
        
        if (previousLength < newLength)
        {
            // checks if the new value is longer than the last to see if user is deleting
            if (value.Length == 2 || value.Length == 5)
            {
                dateInput.text = value + "/";
                dateInput.MoveToEndOfLine(false, false);
                slashPlaced = true;

            }
        }
        previousLength = value.Length;
        oldInput = dateInput.text;
    }


    bool RestrictDateInput(string value)
    {
        // Allow only numbers and slash (/) with a max number of characters
        if (!Regex.IsMatch(value, @"^[\d/]+$") || value.Length > maxDateChars)
        {
            //resets input if it fails validation
            dateInput.text = oldInput;
            return false;
        }
        else
        return true;
    }

    public bool SubmitDateValidation()
    {
        DateTime dateValue;
        if (DateTime.TryParse(dateInput.text, out dateValue))
        {
            return true;
        } 
        else
            return false;
    }
}
