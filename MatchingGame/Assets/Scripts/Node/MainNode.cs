using System.Collections;
using UnityEngine;

public enum State
{
    PAUSE,
    READY
}

public class MainNode : MonoBehaviour {
    private const int CONST_WIDTH = 10;
    private const int CONST_HEIGHT = 10;

    private State currentState = State.PAUSE;
    public State CurrentState
    {
        get { return currentState; }
        set { currentState = value;
        }
    }

    private int width;
    public int Width
    {
        get { return width; }
        set { width = value; }
    }

    private int height;
    public int Height
    {
        get { return height; }
        set { height = value; }
    }

    private int offSet;

    [Header("Types of Node")]
    [SerializeField]
    private GameObject[] nodeTypes;

    [Header("Prefabs")]
    [SerializeField]
    public GameObject destroyParticle;

    [SerializeField]
    private GameObject disableNodePrefab;

    [SerializeField]
    private GameObject highLightPrefab;

    [Header("Root")]
    [SerializeField]
    private Transform enableNodeRoot;

    [SerializeField]
    private Transform disableNodeRoot;

    [Header("Others")]
    [SerializeField]
    private SpriteRenderer spriteBackground;

    [SerializeField]
    public float fadeTime = 1f;

    public GameObject[,] allCreatedNodes;
    private NodeMatches nodeMatches;
    public NodeController currNode;
    private bool isInit;
    
    GameController gController;
    SoundManager soundManager;
    UserData userData;

    // Use this for initialization
    void Start () {
        gController = GameController.GetInstance();
        if (gController != null)
            nodeMatches = GameController.GetInstance().nodeMatches;
        soundManager = SoundManager.getInstance();
        userData = UserData.GetInstance();
        CurrentState = State.PAUSE;
        Init();
        StartCoroutine(gController.FadeSpriteToFullAlpha(fadeTime, spriteBackground));
        StartCoroutine(SetUp());
    }

    #region Setup
    private void Init()
    {
        if(!isInit)
        {
            isInit = true;
            if (ConstantManager.GetInstance() != null)
            {
                width = ConstantManager.GetWidthMap();
                height = ConstantManager.GetHeightMap();
            }
            else
            {
                width = CONST_WIDTH;
                height = CONST_HEIGHT;
            }
            offSet = Mathf.RoundToInt(height / 2f);
            allCreatedNodes = new GameObject[width, height];
        }
    }

    private IEnumerator SetUp()
    {
        if (!isInit)
            Init();
        //Nodes
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (soundManager != null)
                    soundManager.PlaySound(SoundId.CREATE);
                CreateNode(i, j);
                yield return null;
            }
        }
        CurrentState = State.READY;
    }

    private GameObject CreateNode(int column, int row)
    {
        if (column < 0 || row < 0)
            return null;
        Vector2 tempPos = new Vector2(column, row + offSet);
        GameObject gObjPrefab, gNode;
        Vector2 localScale;

        if (!ConstantManager.GetMapIndexStatus(column, row))
        {
            localScale = disableNodePrefab.transform.localScale;
            gNode = Instantiate(disableNodePrefab, tempPos, Quaternion.identity) as GameObject;
            gNode.transform.SetParent(disableNodeRoot);
        }
        else
        {
            int randomType = Random.Range(0, nodeTypes.Length);
            int maxIteration = 0;
            while (CheckNodeMatchesByMapIndex(column, row, nodeTypes[randomType]) && maxIteration < ConstantManager.MAX_ITERATION)
            {
                randomType = Random.Range(0, nodeTypes.Length);
                maxIteration++;
            }
            maxIteration = 0;
            gObjPrefab = nodeTypes[randomType];
            localScale = gObjPrefab.transform.localScale;
            gNode = Instantiate(gObjPrefab, tempPos, Quaternion.identity) as GameObject;
            gNode.transform.SetParent(enableNodeRoot);
        }
        NodeController nodeController = gNode.GetComponent<NodeController>();
        if(nodeController != null)
        {
            nodeController.column = column;
            nodeController.row = row;
        }
        else
        {
            Debug.LogError("Missing NodeController component at ( " + column + "-" + row + ")");
        }
        gNode.transform.localScale = localScale;
        allCreatedNodes[column, row] = gNode;
        gNode.name = "(" + column + "-" + row + ")";
        return gNode;
    }

    private bool CheckNodeMatchesByMapIndex(int column, int row, GameObject gObj)
    {
        if (column > 1 && row > 1)
        {
            if (allCreatedNodes[column - 1, row] != null && allCreatedNodes[column - 2, row]
                && allCreatedNodes[column - 1, row].tag == gObj.tag 
                && allCreatedNodes[column - 2, row].tag == gObj.tag
            )
            {
                return true;
            }
            if (allCreatedNodes[column, row - 1] != null && allCreatedNodes[column, row - 2]
                && allCreatedNodes[column, row - 1].tag == gObj.tag 
                && allCreatedNodes[column, row - 2].tag == gObj.tag
            )
            {
                return true;
            }

        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allCreatedNodes[column, row - 1] != null && allCreatedNodes[column, row - 2]
                    && allCreatedNodes[column, row - 1].tag == gObj.tag 
                    && allCreatedNodes[column, row - 2].tag == gObj.tag
                )
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allCreatedNodes[column - 1, row] != null && allCreatedNodes[column - 2, row] != null
                     && allCreatedNodes[column - 1, row].tag == gObj.tag 
                     && allCreatedNodes[column - 2, row].tag == gObj.tag
                )
                {
                    return true;
                }
            }
        }
        return false;
    }
    #endregion


    #region Destroy Nodes
    public void DestroyNodeMatches()
    {
        int numMatchedNode = 0;
        Transform trans = null;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCreatedNodes[i, j] != null)
                {
                    if (DestroyNodeMatchesByMapIndex(i, j, ref trans))
                        numMatchedNode++;
                }
            }
        }
        
        int score = Util.CalculateScore(numMatchedNode);
        if(score > 0)
        {
            if(gController != null)
                gController.ShowFlyScore(Util.NumberFormat(score), trans);
            if(userData != null)
                userData.IncreaseScore(score, false);
        }
        if (nodeMatches != null && nodeMatches.currentNodeMatches != null)
            nodeMatches.currentNodeMatches.Clear();
        StartCoroutine(MoveDownNodes());
    }

    private bool DestroyNodeMatchesByMapIndex(int column, int row, ref Transform trans)
    {
        NodeController nodeController = allCreatedNodes[column, row].GetComponent<NodeController>();
       if (nodeController != null && nodeController.isMatched)
        {
            trans = nodeController.gameObject.transform;
            if (soundManager != null)
                soundManager.PlaySound(SoundId.SCORE);
            GameObject particle = Instantiate(destroyParticle,
                                              allCreatedNodes[column, row].transform.position,
                                              Quaternion.identity);
            Destroy(particle, 0.5f);
            Destroy(allCreatedNodes[column, row]);
            allCreatedNodes[column, row] = null;
            return true;
        }
        return false;
    }
    #endregion

    #region Move down Nodes
    private IEnumerator MoveDownNodes()
    {
        yield return new WaitForSeconds(ConstantManager.DELAY_TIME);
        int numNullRow = 0;
        bool blackNode;
        int i = 0, j = 0;
        for (i = 0; i < width; i++)
        {
            blackNode = false;
            for (j = 0; j < height; j++)
            {
                if (allCreatedNodes[i, j] == null)
                {
                    numNullRow++;
                }                
                else if (numNullRow > 0)
                {
                    if(!ConstantManager.GetMapIndexStatus(i, j))
                    {
                        blackNode = true;
                        if (j > 0 && j < height - 1 && ConstantManager.GetMapIndexStatus(i, j-1) && allCreatedNodes[i, j - 1] != null)
                        {
                            numNullRow = 0;
                        }
                        else
                        {
                            numNullRow++;
                        }
                    }
                    else
                    {
                        allCreatedNodes[i, j].GetComponent<NodeController>().row -= numNullRow;
                        if (soundManager != null)
                            soundManager.PlaySound(SoundId.CREATE);
                        allCreatedNodes[i, j] = null;
                        if (blackNode)
                        {
                            numNullRow = 1;
                            blackNode = false;
                        }
                        yield return null;
                    }
                }
            }
            numNullRow = 0;
        }        
        yield return new WaitForSeconds(ConstantManager.DELAY_TIME);
        StartCoroutine(OnUpdateNodes());
    }

    private IEnumerator OnUpdateNodes()
    {
        StartCoroutine(ReFillNodes());
        yield return new WaitForSeconds(ConstantManager.DELAY_TIME);

        while (CheckNodeMatches())
        {
            yield return new WaitForSeconds(ConstantManager.DELAY_TIME);
            DestroyNodeMatches();
        }
        if (nodeMatches != null && nodeMatches.currentNodeMatches != null)
            nodeMatches.currentNodeMatches.Clear();
        currNode = null;
        yield return new WaitForSeconds(ConstantManager.DELAY_TIME);
        CurrentState = State.READY;
    }

    private bool CheckNodeMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCreatedNodes[i, j] != null)
                {
                    NodeController nodeController = allCreatedNodes[i, j].GetComponent<NodeController>();
                    if (nodeController != null && nodeController.isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    #endregion

    #region ReFill Nodes full
    private IEnumerator ReFillNodes()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCreatedNodes[i, j] == null)
                {
                    if (soundManager != null)
                        soundManager.PlaySound(SoundId.CREATE);
                    CreateNode(i, j);
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }
    }
    #endregion


    #region HighLight Node
    public void ShowHightLight(Vector2 pos)
    {
        if (highLightPrefab == null)
            return;
        highLightPrefab.transform.position = pos;
        highLightPrefab.SetActive(true);
    }

    public void HideHighLight()
    {
        if (highLightPrefab == null)
            return;
        highLightPrefab.SetActive(false);
    }

    public Vector2 GetPosHighLight()
    {
        if (highLightPrefab == null)
            return Vector2.zero;
        return highLightPrefab.transform.position;
    }

    public bool IsShowingHighLight()
    {
        if (highLightPrefab == null)
            return false;
        return highLightPrefab.activeSelf;
    }

    #endregion
}
