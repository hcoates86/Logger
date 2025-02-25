using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class AppManager : MonoBehaviour
{
    public static AppManager Instance { get; private set; }

    public Error error;
    private bool canEdit = false;
    public CanvasGroupToggle editImage;
    public CanvasGroupToggle editName;
    public CanvasGroupToggle editBirthdate;
    public int currentProfile;

    public List<Profile> allProfiles;
    public Profile profilePrefab;


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

    void DeleteProfile(int id)
    {

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

    public void CreateProfile(string newName, DateTime? birthDate = null, string imagePath = "")
    {

        Profile profile = Instantiate(profilePrefab);
        profile.id = allProfiles.Count + 1;
        profile.named = newName;
        
        profile.profileImage = LoadImage(imagePath);
        
        profile.birthDate = (DateTime) birthDate;
        allProfiles.Add(profile);


    }

    Sprite LoadImage(string path)
    {
        Sprite sprite;
        return null;

    }
}
