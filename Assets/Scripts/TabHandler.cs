using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// This script should be placed on the parent of the tab buttons.
public class TabHandler : MonoBehaviour
{
    [SerializeField] List<Button> buttons = new List<Button>();
    [SerializeField] List<GameObject> containers = new List<GameObject>();

    [SerializeField] Button tabPrefab;

    public static Button buttonSelected;

    void Start()
    {
        foreach (var button in buttons)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    void Reset()
    {
        buttons = GetComponentsInChildren<Button>().ToList();
    }


    void OnButtonClicked()
    {

        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i] == buttonSelected)
            {
                // Selects matching button and container.
                buttons[i].interactable = false;
                containers[i].SetActive(true);
            }
            else
            {
                buttons[i].interactable = true;
                containers[i].SetActive(false);
            }
        }
    }

    void AddTab(string name)
    {
        Button newTab = Instantiate(tabPrefab, transform);
        newTab.GetComponentInChildren<TMPro.TMP_Text>().text = name;
        buttons.Add(newTab);
        // Clones one of the existing containers.
        if (containers.Count > 0)
        {
            GameObject container = Instantiate(containers[0], containers[0].transform.parent);
            containers.Add(container);
            container.SetActive(false);
        }

    }
}
