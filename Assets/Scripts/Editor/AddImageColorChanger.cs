using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public static class AddImageColorChanger
{
   
[MenuItem("Tools/Add Image Color Changer")]
    private static void FindAndAdd()
    {
        var components = Object.FindObjectsByType<Image>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);
            

        foreach (var go in components)
        {
            if (!go.TryGetComponent<ColorChooser>(out _))
            {
                Undo.AddComponent<ColorChooser>(go.gameObject);
                EditorUtility.SetDirty(go);
            }
        }
    }
}
