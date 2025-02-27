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

    public ProfileDisplay shortProfile;
    public CanvasGroupToggle shortProfileToggle;
    public ProfileDisplay fullProfile;
    public CanvasGroupToggle fullProfileToggle;

    public EditButton editButton;

    public List<Profile> allProfiles = new List<Profile>();
    public Profile profilePrefab;
    public Transform profileContainer;
    public GameObject eventPrefab;

    public Sprite starHollow;
    public Sprite starFilled;

    public string deletePath;

    private bool canEdit = false;

    // colors of the pressed and unpressed profile items
    private const string PRESSED_HEX = "#696A8C";
    private const string NORMAL_HEX = "#758398";


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

        // creates the profiles directory
        string path = $"{Application.persistentDataPath}/profiles";

        if (!Directory.Exists(path))
        {
            // Create the directory
            Directory.CreateDirectory(path);
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

    public void EditCurrentProfile(bool nameEdited, bool bdayEdited, bool pictureUploaded, string name = "", string birthDate = "")
    {
        if (canEdit)
        {


            shortProfile.Setup(currentProfile);
        }

    }

    public void DeleteProfileConfirm()
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

    public void SwitchProfile(Profile profile)
    {
        canEdit = false;
        if (editButton.pressed)
            editButton.ToggleButton();

        Color color;
        if (currentProfile != null && ColorUtility.TryParseHtmlString(NORMAL_HEX, out color))
        {
            currentProfile.profileBackground.color = color;
        }

        if (ColorUtility.TryParseHtmlString(PRESSED_HEX, out color))
        {
            profile.profileBackground.color = color;
        }

        shortProfile.Setup(profile);

        if (shortProfileToggle.element.alpha == 0)
        {
            shortProfileToggle.ShowElement();
        }

        currentProfile = profile;
    }

    void LoadAllProfiles()
    {
        // folder all profiles are saved to. Events are saved within profileid folders
        string path = $"{Application.persistentDataPath}/profiles";

    }

    public void CreateProfile(string newName, DateTime birthDate, bool imageUploaded, int id)
    {
        Profile profile = Instantiate(profilePrefab, profileContainer);
        profile.id = id;
        profile.named = newName;
        
        if (imageUploaded)
        {
            profile.profileImage = LoadImage(profile.id);
            profile.thumbnail = LoadThumbnail(profile.id);
        }

        // if (birthDate != DateTime.MinValue)
        // {
            profile.birthDate = birthDate;
        // }
        profile.SaveProfile();

        ProfileDisplay profileDisplay = profile.GetComponent<ProfileDisplay>();
        profileDisplay.Setup(profile);

        allProfiles.Add(profile);

        // clicks the new profile to display on the short profile and whatever else
        SwitchProfile(profile);

        if (shortProfileToggle.element.alpha == 0)
        {
            shortProfileToggle.ShowElement();
        }
    }

    public int CreateNewId()
    {
        int id = allProfiles.Count + 1;
        string path = $"{Application.persistentDataPath}/profiles/profile{id}.json";

        // if directory already exists, not a unique id
        while (File.Exists(path))
        {
            id = UnityEngine.Random.Range(500, 2001);
            Debug.Log(id);
            path = $"{Application.persistentDataPath}/profiles/profile{id}.json";
        }

        return id;

    }

    // the onclick method for the button
    public void DisplayFullProfile()
    {
        fullProfile.Setup(currentProfile);
        fullProfileToggle.ShowElement();

    }

    // loads the image files, picture and thumbnail from the id
    Sprite LoadImage(int profileId)
    {
        string imagePath = $"{Application.persistentDataPath}/{profileId}/picture.png";
        Sprite sprite = LoadPNG(imagePath);

        return sprite;
    }

    Sprite LoadThumbnail(int profileId)
    {
        string thumbnailPath =  $"{Application.persistentDataPath}/{profileId}/thumbnail.png";
        Sprite sprite = LoadPNG(thumbnailPath);

        return sprite;
    }

    public Sprite LoadPNG(string filePath) 
    {
        Texture2D tex = null;
        byte[] fileData;

        Sprite sprite = null;

        if (File.Exists(filePath)) 	{
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D (2, 2, TextureFormat.BGRA32,false);
            //this will auto-resize the texture dimensions.
            tex.LoadImage(fileData); 
        }
        else
        {
            AppManager.Instance.error.SetError("Failed to load image.");
        }
        if (tex != null)
        {
            sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

        }
        return sprite;
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

