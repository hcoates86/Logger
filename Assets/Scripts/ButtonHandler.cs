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

    [SerializeField] Button button;
    [SerializeField] Image image;
    [SerializeField] TMP_Text text;

    Color defaultColor = new Color();

    void Start()
    {
        if (button == null)
            button = GetComponent<Button>();
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
