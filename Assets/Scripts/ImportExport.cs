using System.IO;
using UnityEngine;
using SimpleFileBrowser;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using System.Collections.Generic;

public class ImportExport : MonoBehaviour
{
    public void PickImportFolder()
    {
        // Folders-only mode
        FileBrowser.ShowLoadDialog(
            onSuccess: (paths) =>
            {
                if (paths == null || paths.Length == 0)
                {
                    Debug.Log("User cancelled folder picking.");
                    return;
                }

                string path = paths[0];
                Debug.Log($"Selected folder: {path}");
                OnFolderSelected(path);
            },
            onCancel: () =>
            {
                Debug.Log("User cancelled folder picking.");
            },
            pickMode: FileBrowser.PickMode.Folders
        );
    }

    private void OnFolderSelected(string sourceFolder)
    {
        Debug.Log($"Importing from: {sourceFolder}");

        CopyDirectory(sourceFolder);

        Debug.Log("Folder import complete.");
    }

    // maps old id to new id
    Dictionary<int, int> dict = new Dictionary<int, int>();

    void AddIDsToDict(string sourceDir)
    {
        int totalProfs = PlayerPrefs.GetInt("TotalProfiles");
        
        // Copy over IDs of all subdirectories
        foreach (var dir in Directory.GetDirectories(sourceDir))
        {
            string name = Path.GetFileName(dir);

            if (int.TryParse(name, out int id))
            {
                // adds id num to total profiles to get the same number for the same id, so you get
                // original id as key, new id as value
                dict[id] = id + totalProfs;
            }
            AddIDsToDict(dir);
        }
    }

    public void CopyDirectory(string sourceDir)
    {
        AddIDsToDict(sourceDir);

        string itemPath = "";
        string profilePath = Path.Combine(Application.persistentDataPath, "profiles");
        // profile folder is created in appmanager's awake but just in case
        if (!Directory.Exists(profilePath))
        {
            Directory.CreateDirectory(profilePath);
        }

        // Copy all files
        foreach (string file in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
        {
            string ext = Path.GetExtension(file);
            string fileName = Path.GetFileNameWithoutExtension(file);
            string name = Path.GetFileName(file);

            // checks if the file is a json that would have to be renamed and have the id changed inside
            if (ext == ".json")
            {
                string json = File.ReadAllText(file);

                // check if event (num only)
                if (int.TryParse(fileName, out int _))
                {
                    Debug.Log($"did parse: {fileName}");
                    EventData data = JsonUtility.FromJson<EventData>(json);

                    int newId = dict[data.profileId];
                    // creates new folder at id if not present
                    itemPath = Path.Combine(Application.persistentDataPath, newId.ToString());
                    if (!Directory.Exists(itemPath))
                    {
                        Directory.CreateDirectory(itemPath);
                    }

                    data.profileId = newId;
                    // Save
                    string destFile = Path.Combine(itemPath, name);
                    string updated = JsonUtility.ToJson(data, true);
                    File.WriteAllText(destFile, updated);

                }
                // or profile (profile{num})
                else
                {
                    Debug.Log($"did not parse: {fileName}");

                    ProfileData data = JsonUtility.FromJson<ProfileData>(json);

                    int newId = dict[data.id];
                    // creates new folder at id if not present
                    itemPath = Path.Combine(Application.persistentDataPath, newId.ToString());
                    if (!Directory.Exists(itemPath))
                    {
                        Directory.CreateDirectory(itemPath);
                    }
                    data.id = newId;

                    // Save
                    string destFile = Path.Combine(profilePath, $"profile{newId}.json");
                    string updated = JsonUtility.ToJson(data, true);
                    File.WriteAllText(destFile, updated);

                }
            }
            // otherwise just copies the files
            else
            {
                string parentFolder = Path.GetFileName(Path.GetDirectoryName(file));
                if (int.TryParse(parentFolder, out int id))
                {
                    // creates new folder at id if not present
                    itemPath = Path.Combine(Application.persistentDataPath, dict[id].ToString());
                    if (!Directory.Exists(itemPath))
                    {
                        Directory.CreateDirectory(itemPath);
                    }
                }

                string destFile = Path.Combine(itemPath, name);
                Debug.Log($"Copying files to: {destFile}");
                File.Copy(file, destFile, true);
            }
        }
        // if dictionary contains new values sets it as the new max number
        if (dict.Count > 0)
        {
            int maxValue = dict.Values.Max();
            PlayerPrefs.SetInt("TotalProfiles", maxValue);
        }
        // saves new total prof number, the highest key in dictionary before wiping dict
        dict.Clear();

        // reloads scene after importing
        SceneManager.LoadScene(0);
    }

    void CopyAllFiles(string from, string to)
    {
        foreach (var file in Directory.GetFiles(from))
        {
            string name = Path.GetFileName(file);
            string dest = Path.Combine(to, name);
            File.Copy(file, dest, true);
        }

        // 2) Recurse into each subdirectory
        foreach (var dir in Directory.GetDirectories(from))
        {
            string name = Path.GetFileName(dir);
            string destSubDir = Path.Combine(to, name);
            CopyAllFiles(dir, destSubDir);
        }

    }


    // Optional: default subfolder name appended to the selected folder
    // Set to "" if you want to export directly into the chosen folder.
    private string defaultExportFolderName = "PetLog";

    // Optional: add a date-time suffix to avoid overwriting existing backups
    [SerializeField] private bool appendDateSuffix = true;

    // Optional: overwrite existing files if the destination already has some
    [SerializeField] private bool overwrite = true;


    /// <summary>
    /// Opens a folder picker and starts export once a folder is chosen.
    /// Bind this to your "Export" UI button.
    /// </summary>
    public void PickExportFolder()
    {
        FileBrowser.ShowLoadDialog(
            onSuccess: (paths) =>
            {
                if (paths == null || paths.Length == 0)
                {
                    Debug.Log("User cancelled export folder picking.");
                    return;
                }

                string chosenRoot = paths[0];

                // Compose final destination path
                string destRoot = BuildDestinationPath(chosenRoot);
                Directory.CreateDirectory(destRoot);

                string source = Application.persistentDataPath;
                Debug.Log($"Exporting data from:\n{source}\n→\n{destRoot}");

                StartCoroutine(ExportRoutine(source, destRoot));
            },
            onCancel: () =>
            {
                Debug.Log("User cancelled export folder picking.");
            },
            pickMode: FileBrowser.PickMode.Folders
        );
    }

    private string BuildDestinationPath(string chosenRoot)
    {
        string finalName = defaultExportFolderName;

        if (appendDateSuffix)
        {
            // Example: 2026-01-19_1435
            string stamp = System.DateTime.Now.ToString("yyyy-MM-dd-HHmm");
            finalName = $"{finalName}_{stamp}";
        }

        return Path.Combine(chosenRoot, finalName);
    }

    private IEnumerator ExportRoutine(string sourceDir, string destDir)
    {
        if (!Directory.Exists(sourceDir))
        {
            Debug.LogError("Source not found: " + sourceDir);
            yield break;
        }

        yield return StartCoroutine(CopyDirectoryAsync(sourceDir, destDir));

        Debug.Log($"Export complete! Files saved to:\n{destDir}");
    }

    private IEnumerator CopyDirectoryAsync(string source, string dest)
    {
        Directory.CreateDirectory(dest);

        // Copy all files in current directory
        string[] files = Directory.GetFiles(source);
        for (int i = 0; i < files.Length; i++)
        {
            string file = files[i];
            string name = Path.GetFileName(file);
            string target = Path.Combine(dest, name);

            // Copy, optionally overwriting
            File.Copy(file, target, overwrite);

            // Optional: progress logging (lightweight)
            if (i % 8 == 0) // log every 8 files to reduce spam
                Debug.Log($"Copying: {name}");

            // Yield to keep UI responsive on big exports
            if (i % 16 == 0)
                yield return null;
        }

        // Recurse into subdirectories
        string[] dirs = Directory.GetDirectories(source);
        foreach (string dir in dirs)
        {
            string name = Path.GetFileName(dir);
            string target = Path.Combine(dest, name);
            yield return StartCoroutine(CopyDirectoryAsync(dir, target));
        }
    }
}
