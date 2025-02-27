using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// a simplified error class
public class Error : MonoBehaviour
{
    public GameObject errorPopup;
    public TextMeshProUGUI text;
    [SerializeField]
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
    public void SetError(string errorText)
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
    }

    // shows an error for one second with text from the errorArray
    public void SetError(int errorNum)
    {
        //will blink off and back on if called while running
        if (isErrorRunning)
        {
            StopAllCoroutines();
            StartCoroutine(RefreshObject());
        }
        else
        {
            canvasGroupToggle.ShowElement();
        }

        text.text = errorArray[errorNum];

        isErrorRunning = true;

    }

    // refreshes for the "blink" when changing errors. never turns isErrorRunning off
    IEnumerator RefreshObject()
    {
        canvasGroupToggle.HideElementImmediate();
        yield return new WaitForSecondsRealtime(refreshAmount);
        canvasGroupToggle.ShowElementImmediate();
    }


    public void OkButton()
    {
        isErrorRunning = false;
        canvasGroupToggle.HideElement();
    }

}

