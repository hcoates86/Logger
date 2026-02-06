using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    [SerializeField] Sprite disabledSprite;
    [SerializeField] Sprite enabledSprite;
    [SerializeField] Color disabledTextColor;
    [SerializeField] Color enabledTextColor;
    [SerializeField] Color disabledImageColor;
    [SerializeField] Color enabledImageColor;

    Color[] defaultColors = new Color[4];

    [SerializeField] Button button;
    [SerializeField] Image image;
    [SerializeField] TMP_Text text;

    Color defaultColor = new Color();

    void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        SaveOriginalColors();
        ThemeChooser.onChangeTheme.AddListener(ChangeColors);
    }

    void SaveOriginalColors()
    {
        defaultColors[0] = disabledTextColor;
        defaultColors[1] = enabledTextColor;
        defaultColors[2] = disabledImageColor;
        defaultColors[3] = enabledImageColor;
    }

    void ChangeColors()
    {
        // change the saved color AND the current color
        switch (ThemeChooser.currentTheme)
        {
            case ThemeType.Default:
                disabledTextColor = defaultColors[0];
                enabledTextColor = defaultColors[1];
                disabledImageColor = defaultColors[2];
                enabledImageColor = defaultColors[3];
                break;
            case ThemeType.Dark:
                ThemeChooser.ChangeToGrayscale(ref disabledTextColor);
                ThemeChooser.ChangeToGrayscale(ref enabledTextColor);
                ThemeChooser.ChangeToGrayscale(ref disabledImageColor);
                ThemeChooser.ChangeToGrayscale(ref enabledImageColor);
                break;
        }

        SetCurrentColors();
    }

    void SetCurrentColors()
    {
        if (button.interactable)
        {
            EnableButton();
        }
        else
        {
            DisableButton();
        }
    }

    public void DisableButton()
    {
        button.interactable = false;
        if (image != null)
        {
            if (disabledSprite != null)
                image.sprite = disabledSprite;
            if (disabledImageColor != defaultColor)
                image.color = disabledImageColor;
        }

        if (disabledTextColor != defaultColor && text != null)
            text.color = disabledTextColor;
    }

    public void EnableButton()
    {
        button.interactable = true;
        if (image != null)
        {
            if (enabledSprite != null)
                image.sprite = enabledSprite;
            if (enabledImageColor != defaultColor)
                image.color = enabledImageColor;
        }
        
        if (enabledTextColor != defaultColor && text != null)
        text.color = enabledTextColor;
    }
}
