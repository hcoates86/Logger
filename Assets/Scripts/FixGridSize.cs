using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// fixes the grid size according to current container size. Should only run once.
public class FixGridSize : MonoBehaviour
{
    GridLayoutGroup gridLayoutGroup;
    // the rect holding the total size
    RectTransform parentRect;
    float newGridWidth;
    // number of pieces it's dividing the space into
    int splitIntoPieces = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parentRect = transform.parent.GetComponent<RectTransform>();
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        if (parentRect.rect.width <= 0)
        {
            StartCoroutine(CheckParent());
        }
        else
        {
            ChangeCellSize();
        }

    }

    //waits for layout to update
    IEnumerator CheckParent()
    {
        // waits one frame in case it's ready already before starting a loop
        yield return null;
        
        while (parentRect.rect.width <= 0)
        {
            yield return new WaitForSecondsRealtime(0.2f);
        }
        ChangeCellSize();
    }

    void ChangeCellSize()
    {
        newGridWidth = parentRect.rect.width / splitIntoPieces;
        // changes the width of the cell
        gridLayoutGroup.cellSize = new Vector2(newGridWidth, gridLayoutGroup.cellSize.y);
    }

}