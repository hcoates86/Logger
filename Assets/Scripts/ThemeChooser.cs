using System;
using UnityEngine;
using UnityEngine.Events;

public class ThemeChooser : MonoBehaviour
{
    public static UnityEvent onChangeTheme = new UnityEvent();

    public static ThemeType currentTheme = ThemeType.Default;

    void Start()
    {
        LoadTheme();
    }

    void LoadTheme()
    {
        // overloads into default if not set
        string loadedTheme = PlayerPrefs.GetString("Theme", ThemeType.Default.ToString());
        if (loadedTheme != ThemeType.Default.ToString())
        {
            ChangeTheme(loadedTheme);
        }
    }

    void SaveTheme()
    {
        PlayerPrefs.SetString("Theme", currentTheme.ToString());
    }

    public void ChangeTheme(string themeType)
    {
        Enum.TryParse(themeType, out currentTheme);
        onChangeTheme?.Invoke();
        SaveTheme();
    }

    public static void ChangeToGrayscale(ref Color _color)
    {
        float grayValue = _color.grayscale;
        _color = new Color(grayValue, grayValue, grayValue, _color.a);
    }
}

public enum ThemeType
{
    Default, Dark, Light
}