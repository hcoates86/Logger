using TMPro;
using UnityEngine;
using UnityEngine.UI;

// changes the current color
public class ColorChooserText : MonoBehaviour
{
    public TMP_Text text;

    // The default is saved at start
    Color defaultText;

    // called in editor when component is added
    void Reset()
    {
        TryGetComponent(out text);
    }

    void Awake()
    {
        ThemeChooser.onChangeTheme.AddListener(ChangeColors);
        if (text != null)
            defaultText = text.color;
    }


    void ChangeColors()
    {
        if (text == null) return;

        if (ThemeChooser.currentTheme == ThemeType.Pastel)
        {
            text.color = defaultText;
        }
        else
        {
            text.color = ThemeChooser.allThemes[ThemeChooser.currentTheme.ToString()][ItemType.Text.ToString()];
        }
    }
}
