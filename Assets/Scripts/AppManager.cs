using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


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

        // finds and removes from profile list
        foreach (Profile profile in allProfiles)
        {
            if (profile.id == id)
            {
                CanvasGroupToggle toggle;
                //fade + delete the gameobject
                // checks if the component exists, if not adds it
                if (!profile.TryGetComponent<CanvasGroupToggle>(out toggle))
                {
                    toggle = profile.gameObject.AddComponent<CanvasGroupToggle>();
                }

                toggle.destroyAfterFade = true;
                toggle.HideElement();



            }
            
        }
        //delete related notifications


        // deletes the profile file
        string path = $"{Application.persistentDataPath}/profiles/{id}";

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
