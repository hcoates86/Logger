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

    public void Show(string message, Action onConfirm, string confirmText = "Confirm", 
    string cancelText = "Cancel", Action onCancel = null)
    {
        canvasToggle.ShowElement();

        confirmButton.text = confirmText;
        cancelButton.text = cancelText;
        messageText.text = message;
        onConfirmAction = onConfirm;
        onCancelAction = onCancel;
    }

    public void OnConfirm()
    {
        onConfirmAction?.Invoke();
        canvasToggle.HideElement();
    }

    public void OnCancel()
    {
        onCancelAction?.Invoke();
        canvasToggle.HideElement();
    }
}
