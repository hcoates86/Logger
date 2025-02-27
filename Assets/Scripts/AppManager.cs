using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


public class AppManager : MonoBehaviour
{
    public static AppManager Instance { get; private set; }

    public Error error;
    public ConfirmationDialog confirm;
    public CanvasGroupToggle editImage;
    public CanvasGroupToggle editName;
    public CanvasGroupToggle editBirthdate;
    public Profile currentProfile;
    public UploadImage profileCreationUpload;

    public List<Profile> allProfiles = new List<Profile>();
    public Profile profilePrefab;
    public Transform profileContainer;
    public GameObject eventPrefab;

    public Sprite starHollow;
    public Sprite starFilled;

    public string deletePath;

    private bool canEdit = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject); // Destroy other instance
        }
    }

    public void ChangeEditable(bool changed)
    {
        canEdit = changed;

        if (canEdit)
        {
            editImage.ShowElement();
            editName.ShowElement();
            editBirthdate.ShowElement();
        }
        else
        {
            editImage.HideElement();
            editName.HideElement();
            editBirthdate.HideElement();
        }
    }

    void DeleteProfileConfirm()
    {
        AppManager.Instance.confirm.Show("Are you sure you want to delete this profile?", DeleteProfile);

    }

    void DeleteProfile()
    {
        //delete related notifications


        // deletes the profile file
        string path = $"{Application.persistentDataPath}/profiles/{currentProfile.id}";
        DeleteItemAtPath(path);

        FadeAndDestroy(currentProfile.gameObject);
        allProfiles.Remove(currentProfile);

        currentProfile = null;
    }

    public void DeleteItemAtPath(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"File at {path} has been deleted.");
        }
        else
        {
            Debug.Log($"File at {path} does not exist.");
        }
    }

    public void DeleteItemAtPath()
    {
        // takes the string set here to delete
        if (File.Exists(deletePath))
        {
            File.Delete(deletePath);
            Debug.Log($"File at {deletePath} has been deleted.");
            // empties the string
            deletePath = string.Empty;
        }
        else
        {
            Debug.Log($"File at {deletePath} does not exist.");
            deletePath = string.Empty;
        }
    }

    //fade + delete the gameobject
    public void FadeAndDestroy(GameObject item)
    {
        CanvasGroupToggle toggle;
        // checks if the component exists, if not adds it
        if (!item.TryGetComponent<CanvasGroupToggle>(out toggle))
        {
            toggle = item.AddComponent<CanvasGroupToggle>();
        }

        toggle.destroyAfterFade = true;
        toggle.HideElement();
    }

    void AddProfile(int id)
    {

    }

    void SwitchProfile()
    {
        canEdit = false;
        // currentProfile = 
    }

    void LoadAllProfiles()
    {
        // folder all profiles are saved to. Events are saved within profileid folders
        string path = $"{Application.persistentDataPath}/profiles";

    }

    public void CreateProfile(string newName, DateTime? birthDate = null, bool imageUploaded = false)
    {
        Profile profile = Instantiate(profilePrefab, profileContainer);
        profile.id = allProfiles.Count + 1;
        profile.named = newName;
        
        if (imageUploaded)
            profile.profileImage = LoadImage(profile.id);

        profile.birthDate = (DateTime) birthDate;
        allProfiles.Add(profile);
    }

    // loads the image files, picture and thumbnail from the id
    Sprite LoadImage(int profileId)
    {
        Sprite sprite;
        string imagePath = $"{Application.persistentDataPath}/{profileId}/picture.png";
        string thumbnailPath =  $"{Application.persistentDataPath}/{profileId}/thumbnail.png";

        return null;
    }


    public Profile FindProfile(int id)
    {
        foreach (Profile profile in allProfiles)
        {
            if (profile.id == id)
            {
                return profile;
            }
        }
        return null;
    }
}
