using UnityEngine;

public class DebugTest : MonoBehaviour
{
    bool isDebugOn = false;
    public void CheckClick()
    {
        if (isDebugOn)
        Debug.Log($"{gameObject.name} clicked");
    }
}
