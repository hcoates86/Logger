using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputHandler : MonoBehaviour
{

    public TMP_Text dateInput;
    private int previousLength, newLength;


    public void ReadDateInput()
    {
        HandleDate();
        Debug.Log("bloop");

    }

    // adds / to dates
    // prevent it from adding on delete??
    // if user puts own / after /, overwrite it
    void HandleDate()
    {
        string value = dateInput.text;
        newLength = value.Length;
        
        if (previousLength < newLength)
        {
            if (value.Length == 2 || value.Length == 5)
            {
                dateInput.text = value + "/";

            }
        }
        previousLength = value.Length;
    }
}
