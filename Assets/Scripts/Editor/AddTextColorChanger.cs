using TMPro;
using UnityEngine;
using UnityEditor;

public static class AddTextColorChanger
{

[MenuItem("Tools/Add Text Color Changer")]
    private static void FindAndAdd()
    {
        var components = Object.FindObjectsByType<TMP_Text>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);
            

        foreach (var go in components)
        {
            if (!go.TryGetComponent<ColorChooserText>(out _))
            {
                Undo.AddComponent<ColorChooserText>(go.gameObject);
                EditorUtility.SetDirty(go);
            }
        }
    }
}
