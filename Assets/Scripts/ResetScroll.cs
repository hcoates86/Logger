using UnityEngine;
using UnityEngine.UI;

public class ResetScroll : MonoBehaviour
{
    public Scrollbar scrollbar;

    public void Reset()
    {
        if (scrollbar != null)
            scrollbar.value = 0;
    }
}
