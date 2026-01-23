using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using NodaTime;
using NodaTime.TimeZones;
using Faisalman.AgeCalc;


public class AppManager : MonoBehaviour
{
    public static AppManager Instance { get; private set; }

    public Error error;
    public ConfirmationDialog confirm;
    public Profile currentProfile;
    public Canvas appCanvas;
    public GameObject profileOutlinePrefab;

    public Sprite defaultImage;

    public ProfileDisplay shortProfile;
    public CanvasGroupToggle shortProfileToggle;
    public ProfileDisplay fullProfile;
    public CanvasGroupToggle fullProfileToggle;

    public EditButton editButton;
    // the expanded event that can be viewed by clicking any event
    public CanvasGroupToggle eventPopup;

    public List<Profile> allProfiles = new List<Profile>();
    public Profile profilePrefab;
    public Transform profileContainer;
    public GameObject eventPrefab;
    public GameObject shortEventPrefab;

    public NewEventHandler eventModal;

    public Sprite starHollow;
    public Sprite starFilled;

    public string deletePath;

    public bool canEdit = false;

    // colors of the pressed and unpressed profile items
    private const string PRESSED_HEX = "#5A2E82";
    private const string NORMAL_HEX = "#FE5E78";

    private Color pressedColor;
    private Color normalColor;

    // signifies an event was edited in the full profile and the view needs to be refreshed upon returning to the short profile
    public bool eventEdited = false;
    // signifies name/bday/picture was edited in the full profile and the short profile and profile item need to be refreshed
    private bool _profileEdited = false;
    public bool ProfileEdited
    {
        get { return _profileEdited; }
        set
        {
            if (_profileEdited != value)
            {
                _profileEdited = value;
                // if it was changed to true triggers action
                if (_profileEdited)
                    OnProfileEdit?.Invoke();
            }
        }
    }

    // changing this
    // public ButtonHandler changeOrderButton;
    public ButtonHandler deleteProfileButton;
    public ButtonHandler archiveProfileButton;

    public SortBy profilesSortedBy = SortBy.Created;
    private RectTransform profileContainerRect;

    public event Action OnProfileEdit;

    public GameObject archivedStatus;
    public GameObject archivedStatusFull;

    public bool debugMode;


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

        profileContainerRect = profileContainer.GetComponent<RectTransform>();

        // creates the profiles directory
        string path = $"{Application.persistentDataPath}/profiles";
        if (!Directory.Exists(path))
        {
            // Create the directory
            Debug.Log($"making path, directory does not exist");

            Directory.CreateDirectory(path);
        }
        else
        {
            LoadAllProfiles();
            // loads the value with a default fall-back of created
            string savedSortBy = PlayerPrefs.GetString("profilesSortedBy", "Created");
            profilesSortedBy = (SortBy)Enum.Parse(typeof(SortBy), savedSortBy);
            SortProfilesByCurrentCriteria();

            Debug.Log($"Loading from {path}");
        }

        ColorUtility.TryParseHtmlString(NORMAL_HEX, out normalColor);
        ColorUtility.TryParseHtmlString(PRESSED_HEX, out pressedColor);

        ActivateProfileButtons(false);

    }

    public void ArchiveProfileConfirm()
    {
        if (!currentProfile.isArchived)
        {
            confirm.Show($"Are you sure you want to archive {currentProfile.named}'s profile?\n Their age will no longer be updated.",
            currentProfile.ArchiveProfile);
        }
        else
        {
            confirm.Show($"Are you sure you want to remove {currentProfile.named}'s profile from archive?\n Their age will start to be updated again.",
            currentProfile.RemoveFromArchive);
        }

    }

    public void DeleteProfileConfirm()
    {
        if (currentProfile == null)
        {
            error.SetError("Select a profile before deleting it.");
        }
        else
            confirm.Show($"Are you sure you want to delete {currentProfile.named}'s profile?\nThis will also delete all of its events.",
            // \nThis will also delete all relevant notifications.", 
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
        fullProfileToggle.HideElement();
        ActivateProfileButtons(false);

        Audio.Instance.PlayClip(Audio.Instance.deleteProfile);
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
        Audio.Instance.PlayClip(Audio.Instance.switchProfile);

        editButton.SetEventDeleteButtonsVisible(false);
        EditButton.isEventDeleteVisible = false;

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
                    int customSortNum = int.Parse(data[5]);

                    // enum loads as string name of enum eg "None"
                    Enum.TryParse(data[4], out SortBy sortedBy);

                    // checks if the profile has an image
                    string imagePath = $"{Application.persistentDataPath}/{profileId}/picture.png";
                    bool profileHasImage = false;
                    if (File.Exists(imagePath)) profileHasImage = true;

                    if (customSortNum == 0)
                        customSortNum = 500;

                    // creates the profile from the prefab but doesn't save it since it just loaded it
                    // int id, string newName, DateTime birthDate, int totalEventsAdded, SortBy eventsSortedBy, bool imageUploaded, bool save, bool isarchived, string archivedAge
                    CreateProfile(profileId, data[1], dateValue, totalEvents, sortedBy, profileHasImage, false, customSortNum, bool.Parse(data[6]), data[7]);

                }
            }
        }
    }

    public void SortProfiles(SortBy sortBy, bool scriptOnly = false)
    {
        // bool checks if sorting should be forced by script and not clicked. 
        // Sets previous to none before sorting again to avoid logic that checks profilesSortedBy
        if (scriptOnly)
        {
            profilesSortedBy = SortBy.None;
        }

        // if profiles are already sorted by this, sort by ascending (for button click)
        // also set explicitely for script setup
        if ((sortBy == SortBy.Alphabetical && profilesSortedBy == SortBy.Alphabetical)
            || sortBy == SortBy.ReverseAlphabetical)
        {
            allProfiles.Sort((a, b) => string.Compare(b.named, a.named, StringComparison.OrdinalIgnoreCase));
            profilesSortedBy = SortBy.ReverseAlphabetical;
        }
        else if (sortBy == SortBy.Alphabetical)
        {
            allProfiles.Sort((a, b) => string.Compare(a.named, b.named, StringComparison.OrdinalIgnoreCase));
            profilesSortedBy = SortBy.Alphabetical;
        }

        // if profiles are already sorted by this, sort by ascending (for button click)
        // also set explicitely for script setup
        if ((sortBy == SortBy.Created && profilesSortedBy == SortBy.Created)
            || sortBy == SortBy.ReverseCreated)
        {
            allProfiles.Sort((a, b) => b.id.CompareTo(a.id));
            profilesSortedBy = SortBy.ReverseCreated;
        }
        else if (sortBy == SortBy.Created)
        {
            allProfiles.Sort((a, b) => a.id.CompareTo(b.id));
            profilesSortedBy = SortBy.Created;
        }

        // if profiles are already sorted by this, sort by ascending (for button click)
        // also set explicitely for script setup
        if ((sortBy == SortBy.Age && profilesSortedBy == SortBy.Age)
            || sortBy == SortBy.ReverseAge)
        {
            allProfiles.Sort((a, b) => a.birthDate.CompareTo(b.birthDate));
            profilesSortedBy = SortBy.ReverseAge;
        }
        else if (sortBy == SortBy.Age)
        {
            allProfiles.Sort((a, b) => b.birthDate.CompareTo(a.birthDate));
            profilesSortedBy = SortBy.Age;
        }

        // if profiles are already sorted by custom, sort by ascending custom (for button click)
        // also set explicitely for script setup
        if ((sortBy == SortBy.Custom && profilesSortedBy == SortBy.Custom)
            || sortBy == SortBy.ReverseCustom)
        {
            allProfiles.Sort((a, b) => b.customSortNum.CompareTo(a.customSortNum));
            profilesSortedBy = SortBy.ReverseCustom;
        }
        else if (sortBy == SortBy.Custom)
        {
            allProfiles.Sort((a, b) => a.customSortNum.CompareTo(b.customSortNum));
            profilesSortedBy = SortBy.Custom;
        }

        PlayerPrefs.SetString("profilesSortedBy", profilesSortedBy.ToString());

        // doesn't bother rearranging everything if there are fewer than 2 profiles
        if (allProfiles.Count < 2) return;

        // places the gameobjects in the order of the list in the hierarchy
        for (int i = 0; i < allProfiles.Count; i++)
        {
            allProfiles[i].transform.SetSiblingIndex(i);
        }

        LayoutRebuilder.MarkLayoutForRebuild(profileContainerRect);
    }

    public void SortProfilesByCurrentCriteria()
    {
        SortProfiles(profilesSortedBy, true);

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
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            ProfileData data = JsonUtility.FromJson<ProfileData>(json);

            string[] dataArray = new string[]
            {
                            data.id.ToString(),
                            data.named,
                            data.birthDate,
                            data.totalEventsAdded.ToString(),
                            data.eventsSortedBy.ToString(),
                            data.customSortNum.ToString(),
                            data.isArchived.ToString(),
                            data.archivedAge,
            };

            return dataArray;
        }
        else
            return null;
    }

    public string[] LoadEventData(string path)
    {
        if (File.Exists(path))
        {
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

    public void CreateProfile(int id, string newName, DateTime birthDate, int totalEventsAdded, SortBy eventsSortedBy, bool imageUploaded, bool save, int customSortNum, bool isArchived, string archivedAge)
    {
        Profile profile = Instantiate(profilePrefab, profileContainer);
        profile.id = id;
        profile.named = newName;
        profile.totalEventsAdded = totalEventsAdded;
        profile.eventsSortedBy = eventsSortedBy;
        profile.customSortNum = customSortNum;
        profile.isArchived = isArchived;
        profile.archivedAge = archivedAge;

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

    // set on the onclick for the full prof's close view button. Refreshes with the current profile
    public void HideFullProfile()
    {
        canEdit = false;

        if (eventEdited || ProfileEdited)
        {
            // sorts and refreshes the short profile. No need to sort if under two items
            if (currentProfile.allEvents.Count > 1)
                currentProfile.SortByCurrentCriteria();
            shortProfile.Setup(currentProfile);
        }

        if (ProfileEdited)
        {
            // reloads the image if a new one was uploaded
            if (EditButton.profileImageReplaced)
            {
                currentProfile.profileImage = LoadImage(currentProfile.id);
                currentProfile.thumbnail = LoadThumbnail(currentProfile.id);
                EditButton.profileImageReplaced = false;
            }

            shortProfile.Setup(currentProfile);

            // grabs and refreshes the small profile item
            ProfileDisplay currentProfDisplay = currentProfile.GetComponent<ProfileDisplay>();
            currentProfDisplay.Setup(currentProfile);
        }

        fullProfileToggle.HideElement();

        ProfileEdited = false;
        eventEdited = false;
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
        string thumbnailPath = $"{Application.persistentDataPath}/{profileId}/thumbnail.png";
        Sprite sprite = LoadPNG(thumbnailPath);

        return sprite;
    }

    public Sprite LoadPNG(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        Sprite sprite = null;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2, TextureFormat.BGRA32, false);
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
        if (activate)
        {
            deleteProfileButton.EnableButton();
            archiveProfileButton.EnableButton();
        }
        else
        {
            deleteProfileButton.DisableButton();
            archiveProfileButton.DisableButton();
        }
    }

    string GetFutureDate(DateTime birthDate)
    {
        // Convert both dates to NodaTime LocalDate
        LocalDate today = LocalDate.FromDateTime(DateTime.Today);
        LocalDate future = LocalDate.FromDateTime(birthDate);

        // Calculate the period between today and the future date
        Period period = Period.Between(today, future, PeriodUnits.Years | PeriodUnits.Months | PeriodUnits.Days);

        string returnString = "Due in ";
        if (period.Years > 0)
            returnString += $"{period.Years} {(period.Years == 1 ? "Year" : "Years")} ";
        // if period has more than 1 year, include (0) months, otherwise exlude 0 from months
        if (period.Months > 0 || period.Years > 0)
            returnString += $"{period.Months} {(period.Months == 1 ? "Month" : "Months")} ";
        // int weeksLeft = Mathf.FloorToInt(period.Days / 7);
        int weeksLeft = period.Days % 7;
        // To display only two measurements at a time, doesn't add weeks if there are more than 0 years
        if ((weeksLeft > 0 || period.Months > 0) && period.Years < 1)
            returnString += $"{weeksLeft} {(weeksLeft == 1 ? "Week" : "Weeks")} ";
        // doesn't add days if there are more than 0 years or months
        if ((period.Days > 0 || weeksLeft > 0) && period.Years < 1 && period.Months < 1)
            returnString += $"{period.Days} {(period.Days == 1 ? "Day" : "Days")}";
        return returnString;
    }

    public string GetAge(DateTime birthDate)
    {
        // if date is default value, treat as null
        if (birthDate == DateTime.MinValue)
            return "";

        // since the age calc can't handle future dates, just list as unborn until date
        if (birthDate > DateTime.Now)
        {
            //uses Noda Time to calculate time until birth.
            return GetFutureDate(birthDate);
        }

        Age age = new Age(birthDate, DateTime.Now);

        string pluralWeeks;
        string pluralMonths;
        string pluralYears;
        if (Mathf.Floor(age.Days / 7) == 1)
            pluralWeeks = "Week";
        else
            pluralWeeks = "Weeks";

        if (age.Months == 1)
            pluralMonths = "Month";
        else
            pluralMonths = "Months";
        if (age.Years == 1)
            pluralYears = "Year";
        else
            pluralYears = "Years";

        if (age.Years < 1)
        {
            if (age.Months < 1)
            {
                // the days left over after it's divided into weeks
                int days = age.Days % 7;
                return $"{Mathf.Floor(age.Days / 7)} {pluralWeeks} {days} {(days == 1 ? "day" : "days")}";
            }
            return $"{age.Months} {pluralMonths} {Mathf.Floor(age.Days / 7)} {pluralWeeks}";
        }
        return $"{age.Years} {pluralYears} {age.Months} {pluralMonths}";
    }

    [SerializeField] CanvasGroupToggle editingBackground;
    [SerializeField] CanvasGroupToggle editOrderButtonContainer;
    public bool hasEditedOrder = false;

    public void AllowEditOrder()
    {
        // hides the full profile
        HideFullProfile();
        // changes to custom sorting
        SortProfiles(SortBy.Custom, true);
        editingBackground.ShowElement();
        editOrderButtonContainer.ShowElement();
        // hides the short profile so no profile is showing
        shortProfileToggle.HideElement();
        // nulls out the current profile
        currentProfile = null;
        ProfileDisplay profileDisplay;

        foreach (var item in allProfiles)
        {
            profileDisplay = item.GetComponent<ProfileDisplay>();
            profileDisplay.draggable.SetActive(true);
        }
    }

    // attached to order confirm button (the background image lol)
    // reassigns order number according to current index if it was changed
    public void ConfirmEditOrder()
    {
        editingBackground.HideElement();
        editOrderButtonContainer.HideElement();
        ProfileDisplay profileDisplay;

        for (int i = 0; i < allProfiles.Count; i++)
        {
            if (hasEditedOrder)
            {
                // if order was changed change number to index and save profiles
                allProfiles[i].customSortNum = i;
                allProfiles[i].SaveProfile();
            }
            // turns off draggable
            profileDisplay = allProfiles[i].GetComponent<ProfileDisplay>();
            profileDisplay.draggable.SetActive(false);
        }

        hasEditedOrder = false;
    }
}