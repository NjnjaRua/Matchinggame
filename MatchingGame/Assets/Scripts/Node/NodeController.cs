using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeController : MonoBehaviour {
    [Header("Node Properties")]
    public int column;
    public int row;
    public int resumeColumn;
    public int resumeRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;

    [Header("Swipe Stuff")]
    public float swipeAngle = 0;
    private float swipeDistance = 1f;

    [Header("References")]
    private NodeMatches nodeMatches;
    private MainNode mainNode;


    private Vector2 firstPosTouch;
    private Vector2 finalPosTouch;
    private Vector2 tempPosition;

    public GameObject targetNode;
    private float distanceMove = 0.1f;
    private float durationMove = 0.8f;
    private int cellCheck = 1;
    SoundManager soundManager;

    // Use this for initialization
    void Start ()
    {
        GameController gController = GameController.GetInstance();
        if(gController != null)
        {
            mainNode = gController.mainNode;
            nodeMatches = gController.nodeMatches;
        }
        soundManager = SoundManager.getInstance();
    }

    void Update()
    {
        targetX = column;
        targetY = row;

        //Move Horizontal
        if (Mathf.Abs(targetX - transform.position.x) > distanceMove)
        {
            //Move Horizontal
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, durationMove);
            
            if (mainNode.allCreatedNodes[column, row] != this.gameObject)
                mainNode.allCreatedNodes[column, row] = this.gameObject;
            nodeMatches.FindAllMatches();
        }
        else
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;

        }

        //Move Vertical
        if (Mathf.Abs(targetY - transform.position.y) > distanceMove)
        {           
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, durationMove);
            if (mainNode.allCreatedNodes[column, row] != this.gameObject)
            {
                mainNode.allCreatedNodes[column, row] = this.gameObject;
            }
            nodeMatches.FindAllMatches();
        }
        else
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }
    }

    #region Mouse Event
    private void OnMouseDown()
    {
        if(mainNode != null)
        {
            if (mainNode.CurrentState == State.READY)
            {
                if (soundManager != null)
                    soundManager.PlaySound(SoundId.TOUCH);
                bool isSwapDirect = false;                
                if (mainNode.IsShowingHighLight())
                {
                    Vector2 posHighLight = mainNode.GetPosHighLight();
                    float distanceX = Mathf.Abs(column - posHighLight.x);
                    float distanceY = Mathf.Abs(row - posHighLight.y);
                    //dont allow diagonal move
                    if ((distanceX == 0 && distanceY == cellCheck) || (distanceY == 0 && distanceX == cellCheck))
                    {
                        mainNode.HideHighLight();
                        isSwapDirect = true;
                        firstPosTouch = posHighLight;
                        finalPosTouch = new Vector2(column, row);
                        CheckSwapDirect();
                    }                    
                }
                
                if(!isSwapDirect)
                {
                    firstPosTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    if(mainNode != null)
                    {
                        mainNode.ShowHightLight(new Vector2(column, row));
                    }
                }
            }
        }
    }

    private void OnMouseUp()
    {
        if(mainNode != null)
        {
            if (mainNode.CurrentState == State.READY)
            {
                finalPosTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                CheckSwapBySwipeAngle();
            }
        }
    }

    #endregion

    #region Swap By Swipe Angle

    void CheckSwapBySwipeAngle()
    {
        if (Mathf.Abs(finalPosTouch.y - firstPosTouch.y) > swipeDistance || Mathf.Abs(finalPosTouch.x - firstPosTouch.x) > swipeDistance)
        {            
            swipeAngle = Mathf.Atan2(finalPosTouch.y - firstPosTouch.y, finalPosTouch.x - firstPosTouch.x) * 180 / Mathf.PI;
            if (MoveNodeBySwipAngle())
            {
                if (soundManager != null)
                    soundManager.PlaySound(SoundId.SWAP);
                StartCoroutine(OnMoveNode());
                mainNode.CurrentState = State.PAUSE;
                mainNode.currNode = this;
            }
        }
        else
        {
            mainNode.CurrentState = State.READY;
        }
    }

    bool MoveNodeBySwipAngle()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < mainNode.Width - 1)
        {
            //Right Swipe
            MoveRight();
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < mainNode.Height - 1)
        {
            //Up Swipe
            MoveUp();
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            //Left Swipe
            MoveLeft();
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            //Down Swipe
            MoveDown();
        }
        return true;
    }
    #endregion

    #region Navigation
    private bool MoveRight()
    {
        if (!ConstantManager.GetMapIndexStatus(column + 1, row))
            return false;
        targetNode = mainNode.allCreatedNodes[column + 1, row];
        resumeRow = row;
        resumeColumn = column;
        NodeController nodeController = targetNode.GetComponent<NodeController>();
        if (nodeController != null)
            nodeController.column -= 1;
        column += 1;
        return true;
    }

    private bool MoveLeft()
    {
        if (!ConstantManager.GetMapIndexStatus(column - 1, row))
            return false;
        targetNode = mainNode.allCreatedNodes[column - 1, row];
        resumeRow = row;
        resumeColumn = column;
        NodeController nodeController = targetNode.GetComponent<NodeController>();
        if (nodeController != null)
            nodeController.column += 1;
        column -= 1;
        return true;
    }

    private bool MoveUp()
    {
        if (!ConstantManager.GetMapIndexStatus(column, row + 1))
            return false;
        targetNode = mainNode.allCreatedNodes[column, row + 1];
        resumeRow = row;
        resumeColumn = column;
        NodeController nodeController = targetNode.GetComponent<NodeController>();
        if (nodeController != null)
            nodeController.row -= 1;
        row += 1;
        return true;
    }

    private bool MoveDown()
    {
        if (!ConstantManager.GetMapIndexStatus(column, row - 1))
            return false;
        targetNode = mainNode.allCreatedNodes[column, row - 1];
        resumeRow = row;
        resumeColumn = column;
        NodeController nodeController = targetNode.GetComponent<NodeController>();
        if (nodeController != null)
            nodeController.row += 1;
        row -= 1;
        return true;
    }
         

    #endregion

    #region Swap Direct
    void CheckSwapDirect()
    {
        if (Mathf.Abs(finalPosTouch.y - firstPosTouch.y) > 0 || Mathf.Abs(finalPosTouch.x - firstPosTouch.x) > 0)
        {
            if (MoveNodeDirect())
            {
                if (soundManager != null)
                    soundManager.PlaySound(SoundId.SWAP);
                StartCoroutine(OnMoveNode());
                mainNode.CurrentState = State.PAUSE;
                mainNode.currNode = this;
            }
        }
        else
        {
            mainNode.CurrentState = State.READY;
        }
    }

    bool MoveNodeDirect()
    {
        float distanceY = finalPosTouch.y - firstPosTouch.y;
        float distanceX = finalPosTouch.x - firstPosTouch.x;
        //dont allow diagonal move
        if (distanceX == 0 && distanceY == 0 
            || (distanceX >= cellCheck && distanceY >= cellCheck)
        )
            return false;
        if(distanceY == 0)
        {
            if (distanceX > 0)
            {
                MoveLeft();
            }
            else
                MoveRight();
        }
        else if(distanceX == 0)
        {
            if (distanceY > 0)
                MoveDown();
            else
                MoveUp();
        }
        return true;
    }

    #endregion


    #region Move Node
    public IEnumerator OnMoveNode()
    {
        yield return new WaitForSeconds(ConstantManager.DELAY_TIME);
        if (targetNode != null)
        {
            NodeController nodeController = targetNode.GetComponent<NodeController>();
            if (!isMatched && (nodeController != null && !nodeController.isMatched))
            {
                nodeController.row = row;
                nodeController.column = column;
                row = resumeRow;
                column = resumeColumn;
                yield return new WaitForSeconds(ConstantManager.DELAY_TIME);
                mainNode.currNode = null;
                mainNode.CurrentState = State.READY;
            }
            else
            {
                mainNode.DestroyNodeMatches();
            }
        }
    }

    #endregion
}
