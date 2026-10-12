using Michsky.MUIP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// changes the current color
public class ColorChooser : MonoBehaviour
{
    public Image image;
    public ItemType itemType;

    // The default is saved at start
    Color defaultImage;

    // called in editor when component is added
    void Reset()
    {
        TryGetComponent(out image);
    }

    void Awake()
    {
        ThemeChooser.onChangeTheme.AddListener(ChangeColors);
        if (image != null)
            defaultImage = image.color;
    }


    void ChangeColors()
    {
        if (image == null) return;

        if (ThemeChooser.currentTheme == ThemeType.Pastel)
        {
            image.color = defaultImage;
            HandleGradient(true);
        }
        else
        {
            image.color = ThemeChooser.allThemes[ThemeChooser.currentTheme.ToString()][itemType.ToString()];
            HandleGradient(false);
        }
    }

    // Some buttons have a gradient
    void HandleGradient(bool _on)
    {
        if (TryGetComponent(out UIGradient gradient))
        {
            gradient.enabled = _on;
        }
    }
}
