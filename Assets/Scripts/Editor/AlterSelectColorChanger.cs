using UnityEditor;
using UnityEngine;

public static class AlterSelectColorChange
{
    [MenuItem("Tools/Alter Color Changers")]
    private static void Alter()
    {
        var components = Object.FindObjectsByType<ColorChooser>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);
        
        foreach (var go in components)
        {
            if (go.gameObject.name.ToLower().Contains("icon"))
            {
                go.itemType = ItemType.Text;
            }
// so "icon confirm button" won't change to something else
            else
            {
                if (go.gameObject.name.ToLower().Contains("button"))
                {
                    go.itemType = ItemType.Secondary;
                }

                if (go.gameObject.name.ToLower().Contains("confirm"))
                {
                    go.itemType = ItemType.Accent;
                }
            }


        }
    }
}
