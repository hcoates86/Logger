using UnityEngine;

public class ImportExport : MonoBehaviour
{
    const string FILENAME = "petlog.prf";
    // changeable by user
    string path;

    void ExportFile()
    {
        // just wrap the whole save up 

    }

    void ImportFile()
    {
        // unwrap save and place in folder, BUT don't overwrite other files
        // or place in own imported folder and load them after initial profiles

    }

    void ImportFolder()
    {
        
    }

    // allows the user to select the path to export to, import will have them choose the file
    void SelectPath(string newPath)
    {
        path = newPath;
    }
    
}
