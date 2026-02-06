using TMPro;
using UnityEngine;
using UnityEngine.UI;

// changes the current color
public class ColorChooser : MonoBehaviour
{
    public TMP_Text text;
    public Image image;

    // The default is saved at start
    Color defaultText, defaultImage;

    // called in editor when component is added
    void Reset()
    {
        TryGetComponent(out text);
        TryGetComponent(out image);
    }

    void Awake()
    {
        ThemeChooser.onChangeTheme.AddListener(ChangeColors);
        if (text != null)
            defaultText = text.color;
        if (image != null)
            defaultImage = image.color;
    }


    void ChangeColors()
    {
        switch (ThemeChooser.currentTheme)
        {
            case ThemeType.Default:
                if (text != null)
                    text.color = defaultText;
                if (image != null)
                    image.color = defaultImage;
                break;
            case ThemeType.Dark:
                if (image != null)
                {
                    float grayImageValue = defaultImage.grayscale;
                    Color grayImage = new Color(grayImageValue, grayImageValue, grayImageValue, 255);
                    image.color = grayImage;
                }
                if (text != null)
                {
                    float grayTextValue = defaultText.grayscale;
                    Color grayText = new Color(grayTextValue, grayTextValue, grayTextValue, 255);
                    text.color = grayText;
                }
                break;
        }
    }
}
