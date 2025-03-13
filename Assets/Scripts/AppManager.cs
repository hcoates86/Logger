using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;


public class AppManager : MonoBehaviour
{
    public static AppManager Instance { get; private set; }

    public Error error;
    public ConfirmationDialog confirm;
    public CanvasGroupToggle editImage;
    public CanvasGroupToggle editName;
    public CanvasGroupToggle editBirthdate;
    public Profile currentProfile;

    public Sprite defaultImage;

    public ProfileDisplay shortProfile;
    public CanvasGroupToggle shortProfileToggle;
    public ProfileDisplay fullProfile;
    public CanvasGroupToggle fullProfileToggle;

    public EditButton editButton;

    public List<Profile> allProfiles = new List<Profile>();
    public Profile profilePrefab;
    public Transform profileContainer;
    public GameObject eventPrefab;

    public NewEventHandler eventModal;

    public Sprite starHollow;
    public Sprite starFilled;

    public string deletePath;

    public bool canEdit = false;

    // colors of the pressed and unpressed profile items
    private const string PRESSED_HEX = "#696A8C";
    private const string NORMAL_HEX = "#758398";

    private Color pressedColor;
    private Color normalColor;

    // signifies an event was edited in the full profile and the view needs to be refreshed upon returning to the short profile
    public bool eventEdited = false;
    // signifies name/bday/picture was edited in the full profile and the short profile and profile item need to be refreshed
    public bool profileEdited = false;

    public Button changeOrderButton;
    public Button deleteProfileButton;
    public Button archiveProfileButton;


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
            Debug.Log($"making path, directory does not exist");

            Directory.CreateDirectory(path);
        }
        else
            LoadAllProfiles();

        ColorUtility.TryParseHtmlString(NORMAL_HEX, out normalColor);
        ColorUtility.TryParseHtmlString(PRESSED_HEX, out pressedColor);

        Debug.Log($"loading from {path}");

        ActivateProfileButtons(false);

    }

    public void ChangeEditable(bool changed)
    {
        // if changing canedit to false, confirm if changes should be saved first
        // if (changed == false && canEdit == true && profileEdited)
        // {
        //     confirm.Show("Do you want to close without saving changes?", SaveEdits, "Save", "Close");
        // }

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

    public void DeleteProfileConfirm()
    {
        if (currentProfile == null)
        {
            error.SetError("Select a profile before deleting it.");
        }
        else
        confirm.Show("Are you sure you want to delete this profile? \nThis will also delete all relevant notifications.", 
        DeleteProfile);

    }

    void DeleteProfile()
    {
        //delete related notifications


        // deletes the profile file
        string path = $"{Application.persistentDataPath}/profiles/profile{currentProfile.id}.json";
        DeleteItemAtPath(path);

        // repoints path to the directory
        path = $"{Application.persistentDataPath}/{currentProfile.id}";
        // deletes the profile's directory where image and events are saved
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }

        FadeAndDestroy(currentProfile.gameObject);
        allProfiles.Remove(currentProfile);

        currentProfile = null;

        shortProfileToggle.HideElement();
        ActivateProfileButtons(false);
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
        // CanvasGroup canvas;
        // checks if the component exists, if not adds it
        if (!item.TryGetComponent<CanvasGroupToggle>(out toggle))
        {
            CanvasGroup canvas = item.AddComponent<CanvasGroup>();
            toggle = item.AddComponent<CanvasGroupToggle>();
            toggle.element = canvas;
        }
        toggle.fadeOut = true;
        toggle.destroyAfterFade = true;
        toggle.HideElement();
    }

    public void SwitchProfile(Profile profile)
    {
        canEdit = false;
        if (editButton.pressed)
            editButton.ToggleButton();

        if (currentProfile != null)
        {
            currentProfile.profileBackground.color = normalColor;
            currentProfile.ShowAllEvents(false);
        }

        profile.profileBackground.color = pressedColor;
        profile.ShowAllEvents(true);

        shortProfile.Setup(profile);

        // shows short profile if it's hidden
        if (!shortProfileToggle.IsElementVisible())
        {
            shortProfileToggle.ShowElement();
            ActivateProfileButtons(true);
        }

        currentProfile = profile;
    }

    void LoadAllProfiles()
    {
        // folder all profiles are saved to. Events are saved within profileid in "{Application.persistentDataPath}/{profileId}"
        string path = $"{Application.persistentDataPath}/profiles";
        string getJson = "*.json";

        if (Directory.Exists(path))
        {
            string[] filePaths = Directory.GetFiles(path, getJson);

            if (filePaths.Length > 0)
            {
                foreach (string filePath in filePaths)
                {
                    string[] data = LoadProfileData(filePath);
                    DateTime dateValue;
                    DateTime.TryParse(data[2], out dateValue);

                    // parses ints/enum
                    int profileId = int.Parse(data[0]);
                    int totalEvents = int.Parse(data[3]);
                    // enum loads as string name of enum eg "None"
                    Enum.TryParse(data[4], out SortBy sortedBy);

                    // checks if the profile has an image
                    string imagePath = $"{Application.persistentDataPath}/{profileId}/picture.png";
                    bool profileHasImage = false;
                    if (File.Exists(imagePath)) profileHasImage = true;

                    // creates the profile from the prefab but doesn't save it since it just loaded it
                    // int id, string newName, DateTime birthDate, int totalEventsAdded, SortBy eventsSortedBy, bool imageUploaded, bool save
                    CreateProfile(profileId, data[1], dateValue, totalEvents, sortedBy, profileHasImage, false);
                    
                }
            }
        }
    }

    public void ReloadProfile(int id)
    {
        string path = $"{Application.persistentDataPath}/profiles/profile{id}.json";
        string[] data = LoadProfileData(path);

        DateTime dateValue;
        DateTime.TryParse(data[2], out dateValue);

        // parses ints/enum
        int profileId = int.Parse(data[0]);
        int totalEvents = int.Parse(data[3]);
        Enum.TryParse(data[4], out SortBy sortedBy);


        if (currentProfile.id == id)
        {
            currentProfile.totalEventsAdded = totalEvents;
            currentProfile.eventsSortedBy = sortedBy;
            currentProfile.name = data[1];
            currentProfile.birthDate = dateValue;
        }
        else
        {
            Profile profile = FindProfile(id);

            // these shouldn't even be needed??? but just in case for future needs
            profile.id = profileId;
            profile.totalEventsAdded = totalEvents;
            profile.eventsSortedBy = sortedBy;

            profile.name = data[1];
            profile.birthDate = dateValue;


        }


        
    }

    public string[] LoadProfileData(string path)
    {
        if (File.Exists(path)) {
                string json = File.ReadAllText(path);
                ProfileData data = JsonUtility.FromJson<ProfileData>(json);

                string[] dataArray = new string[]
                {
                            data.id.ToString(),
                            data.named,
                            data.birthDate,
                            data.totalEventsAdded.ToString(),
                            data.eventsSortedBy.ToString(),
                };

                return dataArray;
            }
        else
            return null;
    }

    public string[] LoadEventData(string path)
    {
        if (File.Exists(path)) {
                string json = File.ReadAllText(path);
                EventData data = JsonUtility.FromJson<EventData>(json);

                string[] dataArray = new string[]
                {
                            data.profileId.ToString(),
                            data.eventId.ToString(),
                            data.title,
                            data.notes,
                            data.startDate,
                            data.dueDate,
                            data.hasStartDate.ToString(),
                            data.hasDueDate.ToString(),
                            data.isFavorite.ToString()
                };

                return dataArray;
            }
        else
            return null;
    }

    public void CreateProfile(int id, string newName, DateTime birthDate, int totalEventsAdded, SortBy eventsSortedBy, bool imageUploaded, bool save)
    {
        Profile profile = Instantiate(profilePrefab, profileContainer);
        profile.id = id;
        profile.named = newName;
        profile.totalEventsAdded = totalEventsAdded;
        profile.eventsSortedBy = eventsSortedBy;
        
        if (imageUploaded)
        {
            profile.profileImage = LoadImage(profile.id);
            profile.thumbnail = LoadThumbnail(profile.id);
        }

        profile.birthDate = birthDate;

        if (save)
        {
            profile.SaveProfile();
            // increases the total profile count on profile creation
            PlayerPrefs.SetInt("TotalProfiles", id + 1);
        }

        ProfileDisplay profileDisplay = profile.GetComponent<ProfileDisplay>();
        profileDisplay.Setup(profile);

        allProfiles.Add(profile);

        // clicks the new profile to display on the short profile and whatever else
        // SwitchProfile(profile);
    }

    public int CreateNewId()
    {
        int totalProfiles = PlayerPrefs.GetInt("TotalProfiles");
        Debug.Log("totalProfiles key: " + totalProfiles);

        // if key doesn't exist or is 0, set it to 1 and return that number
        if (totalProfiles <= 0)
        {
            totalProfiles = 1;
            PlayerPrefs.SetInt("TotalProfiles", 1);
        }

        string path = $"{Application.persistentDataPath}/profiles/profile{totalProfiles}.json";

        // if file already exists, not a unique id
        while (File.Exists(path))
        {
            totalProfiles++;
            path = $"{Application.persistentDataPath}/profiles/profile{totalProfiles}.json";
        }

        return totalProfiles;

    }

    // the onclick method for the button
    public void DisplayFullProfile()
    {
        fullProfile.Setup(currentProfile);
        fullProfileToggle.ShowElement();
    }


    void CancelEditsAndCloseFullProf()
    {
        EditButton.editing = false;
        HideFullProfile();
    }

    // set on the onclick for the full prof's close view button. Refreshes with the current profile
    public void HideFullProfile()
    {
        if (EditButton.editing)
        {
            confirm.Show("Do you want to close without saving changes to the profile?", editButton.SubmitEdit, "Save", "Close", CancelEditsAndCloseFullProf);
            return;
        }

        if (eventEdited || profileEdited)
        {
            // sorts and refreshes the short profile. No need to sort if under two items
            if (currentProfile.allEvents.Count > 1)
                currentProfile.SortByCurrentCriteria();
            shortProfile.Setup(currentProfile);
        }

        if (profileEdited)
        {
            // reloads the image in case a new one was uploaded
            currentProfile.profileImage = LoadImage(currentProfile.id);
            currentProfile.thumbnail = LoadThumbnail(currentProfile.id);

            shortProfile.Setup(currentProfile);

            // grabs and refreshes the small profile item
            ProfileDisplay currentProfDisplay = currentProfile.GetComponent<ProfileDisplay>();
            currentProfDisplay.Setup(currentProfile);
        }

        fullProfileToggle.HideElement();

        profileEdited = false;
        eventEdited = false;

        //TODO: confirm
        ChangeEditable(false);
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

    public void ClearChildren(Transform objectTrans)
    {
        int i = 0;

        //Array to hold all child obj
        GameObject[] allChildren = new GameObject[objectTrans.childCount];

        //Find all child obj and store to that array
        foreach (Transform child in objectTrans)
        {
            allChildren[i] = child.gameObject;
            i += 1;
        }

        //Now destroy them
        foreach (GameObject child in allChildren)
        {
            Destroy(child.gameObject);
        }

    }

    // enables or disables buttons that can only be used with currentProfile
    public void ActivateProfileButtons(bool activate)
    {
        // changeOrderButton.interactable = activate;
        // deleteProfileButton.interactable = activate;
        // archiveProfileButton.interactable = activate;
        
        // a dumb one-liner just for fun
        changeOrderButton.interactable = deleteProfileButton.interactable = archiveProfileButton.interactable = activate;
    }

    // set on the onclick for the full prof's close view button. Refreshes with the current profile
    // public void RefreshShortProfile()
    // {
    //     if (profileEdited)
    //     {
    //         // refreshes the short profile
    //         shortProfile.Setup(currentProfile);
    //         // grabs and refreshes the small profile item
    //         ProfileDisplay profileItem = currentProfile.GetComponent<ProfileDisplay>();
    //         profileItem.Setup(currentProfile);

    //     }
    //     if (eventEdited)
    //     {
    //         shortProfile.SetEvents(currentProfile);
    //     }

    //     profileEdited = false;
    //     eventEdited = false;
    // }
}

