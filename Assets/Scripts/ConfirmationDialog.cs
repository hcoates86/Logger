using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

// a simple confirmation dialogue (not my most updated)
public class ConfirmationDialog : MonoBehaviour
{
    public TMP_Text messageText;
    private Action onConfirmAction;
    private Action onCancelAction;
    [SerializeField] private CanvasGroupToggle canvasToggle;
    [SerializeField] private TMP_Text confirmButton; 
    [SerializeField] private TMP_Text cancelButton;
    // This needs to be enabled/disabled to fix its content
    [SerializeField] private GameObject content;

    public void Show(string message, Action onConfirm, string confirmText = "Confirm",
    string cancelText = "Cancel", Action onCancel = null)
    {
        canvasToggle.ShowElement();


        confirmButton.text = confirmText;
        cancelButton.text = cancelText;
        messageText.text = message;
        onConfirmAction = onConfirm;
        onCancelAction = onCancel;
        if (content.activeSelf)
            content.SetActive(false);
        content.SetActive(true);

    }

    public void OnConfirm()
    {
        onConfirmAction?.Invoke();
        canvasToggle.HideElement();
        content.SetActive(false);
    }

    public void OnCancel()
    {
        onCancelAction?.Invoke();
        canvasToggle.HideElement();
        content.SetActive(false);
    }
}
