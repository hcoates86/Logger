using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditButton : MonoBehaviour
{
    private ColorBlock cbPressed; 
    private ColorBlock cbNormal;
    public TMP_Text text;
    public Button button;

    private Color initialTextColor;
    private Color pressedTextColor;
    
    private string initial = "Edit";
    private string done = "Finish";
    public bool pressed = false;

    public InputHandler editInput;

    public bool nameEdited, bdayEdited, imageUploaded;

    void Start()
    {
        cbPressed = button.colors;
        cbNormal = button.colors;
        //saves the initial colors selected in inspector
        cbPressed.normalColor = cbPressed.pressedColor;
        cbPressed.selectedColor = cbPressed.pressedColor;

        initialTextColor = text.color;
        pressedTextColor = new Color32(233, 233, 233, 255);
    }

    public void ToggleButton()
    {
        if (!pressed)
        {
            pressed = true;
            text.text = done;
            button.colors = cbPressed;
            text.color = pressedTextColor;

            SetEventDeleteButtonsVisible(true);
            AppManager.Instance.ChangeEditable(true);

        }
        else
        {
            EditProfile();
            pressed = false;
            text.text = initial;
            button.colors = cbNormal;
            text.color = initialTextColor;

            SetEventDeleteButtonsVisible(false);
            AppManager.Instance.ChangeEditable(false);
        }


    }

    void EditProfile()
    {
        //on finish click

        // AppManager.Instance.EditCurrentProfile();
    }

    void SetEventDeleteButtonsVisible(bool show)
    {
        if (AppManager.Instance.currentProfile == null ||
            AppManager.Instance.currentProfile.allEvents.Count < 1) return;

        foreach (EventItem item in AppManager.Instance.currentProfile.allEvents)
        {
            if (show)
            {
                item.deleteButtonCGT.ShowElement();
            }
            else
            {
                item.deleteButtonCGT.HideElement();
            }
            
        }
    }
}
