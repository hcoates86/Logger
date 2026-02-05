using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System;

public class DateInputValidator : MonoBehaviour
{
    public TMP_InputField dateInput;
    public GameObject optionalObject;

    // submit output
    public DateTime dateValue;

    private string oldInput = string.Empty;
    private string value;
    public bool optional = false;

    void Start()
    {
        if (dateInput == null)
            dateInput = GetComponent<TMP_InputField>();

        if (optional)
            optionalObject.SetActive(true);
        else
            optionalObject.SetActive(false);

    }

    // a button that adds today's date to the input
    public void ClickToday()
    {
        DateTime today = DateTime.Now;
        dateInput.text = today.ToString("MM/dd/yyyy");
        oldInput = dateInput.text;
    }

    public void ReadDateInput()
    {
        value = dateInput.text;
        RestrictDateInput(value);
    }

    int prevLength;
    void AddSlash(string value)
    {
        // checks the length but also that it's typing and not deleting
        if ((value.Length == 2 || value.Length == 5) && prevLength < value.Length)
        {
            if (value[value.Length - 1] != '/')
            {
                dateInput.text = value + "/";
                // moves the caret over visually
                dateInput.caretPosition++;
                // moves the position over
                dateInput.stringPosition = dateInput.caretPosition;
            }
        }

        // saves the length after checks/changes
        prevLength = dateInput.text.Length;
    }

    void RestrictDateInput(string value)
    {
        oldInput = value;
        // Allow only numbers and slashes (/) with a max number of characters
        if (!Regex.IsMatch(value, @"^[\d/]+$") || value.Length > 10)
        {
            //resets input if it fails validation
            dateInput.text = oldInput;
        }
        // if it passes validation checks if it should add a slash
        else
            AddSlash(value);
    }

    public bool SubmitDateValidation()
    {
        if (DateTime.TryParse(dateInput.text, out dateValue))
        {
            return true;
        }
        if ((dateInput.text == null || dateInput.text == string.Empty) && optional)
        {
            // sets the date to minvalue if it's empty
            dateValue = DateTime.MinValue;
            return true;
        }
        // else defaults to false
        AppManager.Instance.error.SetError(1);
        return false;
    }

    void ResetInput()
    {
        dateInput.text = "";
        // previousLength = newLength = 0;
        // slashPlaced = false;
        oldInput = value = string.Empty;

    }

    void OnEnable()
    {
        ResetInput();
    }
}
