using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeOrder : MonoBehaviour
{
    public TMP_InputField orderInput;
    public CanvasGroupToggle cgt;

    public void SubmitInput()
    {
        if (orderInput.text == string.Empty)
        {
            AppManager.Instance.error.SetError("Input a 3 digit number or cancel.");
            return;
        }

        AppManager.Instance.currentProfile.customSortNum = int.Parse(orderInput.text);
        AppManager.Instance.currentProfile.SaveProfile();
        ClearInput();
    }

    void ClearInput()
    {
        cgt.HideElement();
        orderInput.text = string.Empty;
    }

    public void OnClick()
    {
        if (AppManager.Instance.currentProfile == null)
            AppManager.Instance.error.SetError("Select a profile before changing the order.");
        else
        {
            cgt.ShowElement();
            orderInput.Select();
        }
    }


    
}
