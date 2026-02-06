using System;
using UnityEngine;
using UnityEngine.Events;

public class ThemeChooser : MonoBehaviour
{
    public static UnityEvent onChangeTheme;

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
}

public enum ThemeType
{
    Default, Dark, Light
}