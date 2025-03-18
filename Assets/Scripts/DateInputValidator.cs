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

    void RestrictDateInput(string value)
    {
        oldInput = value;
        // Allow only numbers and slash (/) with a max number of characters
        if (!Regex.IsMatch(value, @"^[\d/]+$"))
        {
            //resets input if it fails validation
            dateInput.text = oldInput;
        }
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
