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
    private const int imageSize = 1024;
    private const int thumbnailSize = 256;

    void OnDisable()
    {
        imageUploaded = false;
        texture = null;
    }

    void ResizePolicy(ref int width, ref int height)
    {
        width = imageSize;
        height = imageSize;
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
        // ImageCropper.Instance.Show( Texture image, CropResult onCrop, Settings settings = null, ImageResizePolicy croppedImageResizePolicy = null )
			if( ImageCropper.Instance.IsOpen ) return;

                ImageCropper.Instance.Show(
                image: texture,
                onCrop: (bool result, Texture original, Texture2D cropped) =>
                {
                    if (!result)
                    {
                        Debug.Log("Cropping canceled.");
                        return;
                    }

                    texture = cropped;
                    // assign to UI Image using a Sprite:
                    displayImage.sprite = Sprite.Create(cropped, new Rect(0, 0, cropped.width, cropped.height), new Vector2(0.5f, 0.5f));
                },
                settings: new ImageCropper.Settings()
                {
                    // autoZoomEnabled = true,
                    // imageBackground = Color.clear, // transparent background
                    markTextureNonReadable = false,
                    // square selection
                    selectionMinAspectRatio = 1,
                    selectionMaxAspectRatio = 1
                },
                croppedImageResizePolicy: ( ref int width, ref int height ) =>
                {
                    width = imageSize;
                    height = imageSize;
                }
            );
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
        texture = ResizeTexture(texture, thumbnailSize, thumbnailSize);
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
