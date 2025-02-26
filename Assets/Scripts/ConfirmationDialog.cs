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
    [SerializeField] private CanvasGroupToggle confirmGameObject;
    [SerializeField] private TMP_Text confirmButton; 
    [SerializeField] private TMP_Text cancelButton;

    public void Show(string message, Action onConfirm, string confirmText = "Confirm", string cancelText = "Cancel")
    {
        confirmGameObject.ShowElement();

        confirmButton.text = confirmText;
        cancelButton.text = cancelText;
        messageText.text = message;
        onConfirmAction = onConfirm;
    }

    public void OnConfirm()
    {
        onConfirmAction?.Invoke();
        confirmGameObject.HideElement();
    }

    public void OnCancel()
    {
        confirmGameObject.HideElement();
    }
}
