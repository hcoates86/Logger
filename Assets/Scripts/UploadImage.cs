using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class UploadImage : MonoBehaviour
{
public Image displayImage; // UI Image to display the selected image
    public TMP_Text statusText; // TextMesh Pro text to display status messages

    public void PickImage()
    {
        if (NativeGallery.IsMediaPickerBusy())
            return;

        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                Texture2D texture = NativeGallery.LoadImageAtPath(path, -1);
                if (texture == null)
                {
                    statusText.text = "Couldn't load texture from " + path;
                    return;
                }

                displayImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                StartCoroutine(Upload(texture));
            }
        }, "Select an image", "image/*");

        statusText.text = "Permission result: " + permission;
    }

    private IEnumerator Upload(Texture2D texture)
    {
        byte[] imageData = texture.EncodeToPNG();
        WWWForm form = new WWWForm();
        form.AddBinaryData("image", imageData, "image.png", "image/png");

        using (UnityWebRequest www = UnityWebRequest.Post("YOUR_UPLOAD_URL", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                statusText.text = "Upload failed: " + www.error;
            }
            else
            {
                statusText.text = "Upload successful!";
            }
        }
    }
}
