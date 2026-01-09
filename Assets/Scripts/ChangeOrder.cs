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
            AppManager.Instance.error.SetError("Input a number with up to 3 digits.");
            return;
        }

        int submittedNum = int.Parse(orderInput.text);

        if (submittedNum <= 0)
        {
            AppManager.Instance.error.SetError("Please set an order number from 1-999");
            return;
        }

        AppManager.Instance.currentProfile.customSortNum = submittedNum;
        AppManager.Instance.currentProfile.SaveProfile();

        if (AppManager.Instance.profilesSortedBy == SortBy.Custom || AppManager.Instance.profilesSortedBy == SortBy.ReverseCustom)
        {
            AppManager.Instance.SortProfilesByCurrentCriteria();
        }

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
            orderInput.text = AppManager.Instance.currentProfile.customSortNum.ToString();
        }
    }


    
}
