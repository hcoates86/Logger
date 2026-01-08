using UnityEngine;

public class ImportExport : MonoBehaviour
{
    const string FILENAME = "petlog.bak";
    // changeable by user
    string path;

    void ExportFile()
    {
        // just wrap the whole save up 

    }

    void ImportFile()
    {
        // unwrap save and place in folder, BUT don't overwrite other files. Maybe assign new random large ID on export?

    }

    // allows the user to select the path to export to, import will have them choose the file
    void SelectPath(string newPath)
    {
        path = newPath;
    }
    
}
