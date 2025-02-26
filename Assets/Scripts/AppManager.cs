using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


public class AppManager : MonoBehaviour
{
    public static AppManager Instance { get; private set; }

    public Error error;
    public CanvasGroupToggle editImage;
    public CanvasGroupToggle editName;
    public CanvasGroupToggle editBirthdate;
    public int currentProfile;

    public List<Profile> allProfiles = new List<Profile>();
    public Profile profilePrefab;
    public GameObject eventPrefab;

    public Sprite starHollow;
    public Sprite starFilled;

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

    void DeleteProfile(int id)
    {

        // finds and removes from profile list
        // foreach (Profile profile in allProfiles)
        // {
        //     if (profile.id == id)
        //     {
        //         FadeAndDestroy(profile.gameObject);
        //         allProfiles.Remove(profile);
        //     }
            
        // }
        Profile profile = FindProfile(id);
        FadeAndDestroy(profile.gameObject);
        allProfiles.Remove(profile);

        //delete related notifications


        // deletes the profile file
        string path = $"{Application.persistentDataPath}/profiles/{id}";
        DeleteItemAtPath(path);

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
