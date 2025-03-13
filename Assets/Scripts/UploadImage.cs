using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.IO;

public class UploadImage : MonoBehaviour
{
    public Image displayImage; // UI Image to display the selected image

    public bool imageUploaded = false;
    private Texture2D texture;

    void OnDisable()
    {
        imageUploaded = false;
        texture = null;
    }

    public void PickImage()
    {
        if (NativeGallery.IsMediaPickerBusy())
            return;

        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                texture = NativeGallery.LoadImageAtPath(path, -1);
                if (texture == null)
                {
                    return;
                }
                //resizes the image to fit
                texture = ResizeTexture(texture, 800, 800);
                displayImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                // sets bool true to note an image was uploaded for inputhandler
                imageUploaded = true;
                EditButton.editing = true;
            }
        }, "Select an image", "image/*");

    }

    public string Upload(int profileId)
    {
        byte[] imageData = texture.EncodeToPNG();
        string imagepath = $"{Application.persistentDataPath}/{profileId}";
        if (!Directory.Exists(imagepath))
        {
            // Create the directory
            Directory.CreateDirectory(imagepath);
        }
        string picturePath = $"{imagepath}/picture.png";
        System.IO.File.WriteAllBytes(picturePath, imageData);
        Debug.Log($"Image saved to {imagepath}");
        SaveThumbnail(profileId);
        return picturePath;
    }

    void SaveThumbnail(int profileId)
    {
        texture = ResizeTexture(texture, 300, 300);
        byte[] imageData = texture.EncodeToPNG();
        string imagepath = $"{Application.persistentDataPath}/{profileId}";
        if (!Directory.Exists(imagepath))
        {
            // Create the directory
            Directory.CreateDirectory(imagepath);
        }
        string picturePath = $"{imagepath}/thumbnail.png";
        System.IO.File.WriteAllBytes(picturePath, imageData);
        Debug.Log($"thumbnail saved to {imagepath}");


    }

    Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
    {
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        rt.filterMode = FilterMode.Bilinear;
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        Texture2D result = new Texture2D(newWidth, newHeight);
        result.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        result.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        return result;
    }
}
