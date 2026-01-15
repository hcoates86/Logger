using UnityEngine;

public class DropHandler : MonoBehaviour
{
    public Transform targetContainer;
    // the item that will be placed as a placeholder in the container. Spawned as needed.
    GameObject placeHolder;
    Vector2 offset = new Vector2(5, 5);

    void Start()
    {
        if (targetContainer == null)
            targetContainer = transform;
    }

    // spawns at the index
    public void SpawnPlaceholder(int index)
    {
        placeHolder = Instantiate(AppManager.Instance.profileOutlinePrefab, targetContainer);
        placeHolder.transform.SetSiblingIndex(index);
    }

    // destroys the placeholder and returns its pre-destruction index
    public int GetIndexAndDestroy()
    {
        if (placeHolder == null) return -1;

        int index = placeHolder.transform.GetSiblingIndex();
        Destroy(placeHolder);

        return index;
    }


    public void MovePosition(int index)
    {
        placeHolder.transform.SetSiblingIndex(index);
    }
}
