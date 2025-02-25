using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Error : MonoBehaviour
{
    public GameObject errorPopup;
    public TextMeshProUGUI text;
    [SerializeField] GameObject okButton;
    private bool isErrorRunning = false;
    public CanvasGroupToggle canvasGroupToggle;

    private float refreshAmount = 0.1f;

    private string[] errorArray = {"Something went wrong.", "Date is invalid. Please use day/month/year format using only numbers and slashes (/)."};

    void Start()
    {
        if (AppManager.Instance.error == null)
            AppManager.Instance.error = this;
    }

    // defaults to no delay, ok button used
    public void SetError(string errorText, float delay = 0, bool moveToObject = false)
    {
        //will blink off and back on if called while running
        if (isErrorRunning)
        {
                StopAllCoroutines();
                StartCoroutine(RefreshObject());
        }
        else
            canvasGroupToggle.ShowElement();

        text.text = errorText;

        isErrorRunning = true;

        if (delay > 0)
        {
            StartCoroutine(DeactivateObject(delay));
            if (okButton != null)
                okButton.SetActive(false);
        }
        else
        {
            if (okButton != null)
                okButton.SetActive(true);
        }
    }

    // shows an error for one second with text from the errorArray
    public void SetError(int errorNum, bool moveToObject = false)
    {
        //will blink off and back on if called while running
        if (isErrorRunning)
        {
            StopAllCoroutines();
            StartCoroutine(RefreshObject());
        }
        else
        {
            if (moveToObject)
            {
                //moves element to above object

            }
            canvasGroupToggle.ShowElement();
        }
        okButton.SetActive(false);

        text.text = errorArray[errorNum];

        isErrorRunning = true;

        StartCoroutine(DeactivateObject(1));

    }

    IEnumerator DeactivateObject(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        isErrorRunning = false;
        canvasGroupToggle.HideElement();
    }

    // refreshes for the "blink" when changing errors. never turns isErrorRunning off
    IEnumerator RefreshObject()
    {
        canvasGroupToggle.HideElement();
        yield return new WaitForSecondsRealtime(refreshAmount);
        canvasGroupToggle.ShowElement();
    }


    public void OkButton()
    {
        isErrorRunning = false;
        canvasGroupToggle.HideElement();
    }

}

