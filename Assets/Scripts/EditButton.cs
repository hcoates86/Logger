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

            AppManager.Instance.ChangeEditable(true);

        }
        else
        {
            EditProfile();
            pressed = false;
            text.text = initial;
            button.colors = cbNormal;
            text.color = initialTextColor;

            AppManager.Instance.ChangeEditable(false);
        }


    }

    void EditProfile()
    {

        // AppManager.Instance.EditCurrentProfile();
        
    }
}
