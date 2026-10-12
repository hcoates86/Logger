using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Michsky.MUIP;

public class ThemeChooser : MonoBehaviour
{
    public static UnityEvent onChangeTheme = new UnityEvent();

    public static ThemeType currentTheme = ThemeType.Pastel;
    [SerializeField] UIManager michskyManager;

    // for pastels just loads default colors
    // [SerializeField] List<Color> pastels = new();
    [Tooltip("In Main, Secondary, Accent, Text order")]
    [SerializeField] List<Color> darks = new();
    [SerializeField] List<Color> lights = new();

    public Dictionary<string, Color> darkDictionary = new();
    public Dictionary<string, Color> lightDictionary = new();

    public static Dictionary<string, Dictionary<string, Color>> allThemes = new();

    void Start()
    {
        // Builds dictionaries
        darkDictionary = new Dictionary<string, Color>()
        {
            {ItemType.Main.ToString(), darks[0]},
            {ItemType.Secondary.ToString(), darks[1]},
            {ItemType.Accent.ToString(), darks[2]},
            {ItemType.Text.ToString(), darks[3]},
        };

        lightDictionary = new Dictionary<string, Color>()
        {
            {ItemType.Main.ToString(), lights[0]},
            {ItemType.Secondary.ToString(), lights[1]},
            {ItemType.Accent.ToString(), lights[2]},
            {ItemType.Text.ToString(), lights[3]},
        };

        allThemes = new Dictionary<string, Dictionary<string, Color>>()
        {
            {ThemeType.Dark.ToString(), darkDictionary},
            {ThemeType.Light.ToString(), lightDictionary}
        };


        LoadTheme();
    }

    void LoadTheme()
    {
        // overloads into Pastel if not set
        string loadedTheme = PlayerPrefs.GetString("Theme", ThemeType.Pastel.ToString());
        if (loadedTheme != ThemeType.Pastel.ToString())
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

        if (currentTheme != ThemeType.Pastel)
            michskyManager.inputFieldColor = allThemes[currentTheme.ToString()][ItemType.Secondary.ToString()];
        else
            michskyManager.inputFieldColor = Color.white;

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
    Pastel, Dark, Light
}

public enum ItemType
{
    Main, Secondary, Accent, Text
}