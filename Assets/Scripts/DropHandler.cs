using UnityEngine;
using UnityEngine.UI;

public class DropHandler : MonoBehaviour
{
    public Transform targetContainer;
    public GridLayoutGroup gridLayoutGroup;

    // the item that will be placed as a placeholder in the container. Spawned as needed.
    public GameObject placeHolder { get; private set; }
    // whether to auto scroll when dragged to edge (in this case only checked up/down)
    public bool scrollOnEdge;
    // scrolls by this amount
    public float scrollValue;
    public Scrollbar scrollbar;
    // the container dragged item should be clamped to. Will scroll based on this.
    public RectTransform clampedContainer;
    public static float minY;
    public static float maxY;
    // the speed at which the dragger returns in animation
    public const float RETURN_SPEED = 3200;

    void Start()
    {
        if (targetContainer == null)
            targetContainer = transform;
        if (gridLayoutGroup == null)
            gridLayoutGroup = targetContainer.GetComponent<GridLayoutGroup>();
    }

    // spawns at the index on first drag so it can get the corners too
    public void SpawnPlaceholder()
    {
        placeHolder = Instantiate(AppManager.Instance.profileOutlinePrefab, targetContainer);
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
        if (!placeHolder.activeSelf)
            placeHolder.SetActive(true);
        if (placeHolder.transform.GetSiblingIndex() != index)
            placeHolder.transform.SetSiblingIndex(index);
    }

    // hides the placeholder and returns its index
    public int GetIndexAndHide()
    {
        if (placeHolder == null) return -1;

        placeHolder.SetActive(false);
        return placeHolder.transform.GetSiblingIndex();
    }

    public void ShowPlaceholder(int index)
    {
        if (placeHolder == null)
            SpawnPlaceholder();
        //moves to location then activates
        placeHolder.transform.SetSiblingIndex(index);
        Debug.Log("ShowPlaceholder" + index);
        placeHolder.SetActive(true);
    }

    public void Scroll(Direction direction)
    {
        if (!scrollOnEdge) return;

        switch (direction)
        {
            case Direction.Up:
                if (scrollbar.value < 1)
                    scrollbar.value += scrollValue;
                break;
            case Direction.Down:
                scrollbar.value -= scrollValue;
                break;
                // left and right should move value on horizontal scroll bar
        }
    }
}

public enum Direction
{
    Up, Down, Right, Left
}