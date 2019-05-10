using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeHighlight : MonoBehaviour {
    [Header("Properties")]
    public int column;
    public int row;


    public void SetNodeHighlightPos(int column, int row)
    {
        this.column = column;
        this.row = row;
        ShowHighlight();
        RectTransform rectTrans = this.gameObject.GetComponent<RectTransform>();

        int sizeX, sizeY;
        if (rectTrans != null)
        {
            Vector2 sizeDelta = rectTrans.sizeDelta;
            sizeX = (int)sizeDelta.x;
            sizeY = (int)sizeDelta.y;
        }
        else
        {
            sizeX = sizeY = ConstantManager.NODE_SIZE;
        }
        Vector2 tempPosition = new Vector2((column * sizeX) + sizeX, (row * sizeY) + sizeY);
        this.transform.localPosition = tempPosition;
        
    }

    public void ShowHighlight()
    {
        if (gameObject.activeSelf)
            return;
        gameObject.SetActive(true);
    }

    public void HideHighlight()
    {
        if (!gameObject.activeSelf)
            return;
        gameObject.SetActive(false);
    }

    public Vector2 GetHighlightInfo()
    {
        return new Vector2(column, row);
    }
}
