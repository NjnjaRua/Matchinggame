using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeMatches : MonoBehaviour {
    private MainNode mainNode;
    public List<GameObject> currentNodeMatches = new List<GameObject>();

    // Use this for initialization
    void Start () {
        if (GameController.GetInstance())
            mainNode = GameController.GetInstance().mainNode;
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(0.1f);
        if(mainNode != null)
        {
            for (int i = 0; i < mainNode.Width; i++)
            {
                for (int j = 0; j < mainNode.Height; j++)
                {
                    GameObject currentNode = mainNode.allCreatedNodes[i, j];
                    if (currentNode != null && ConstantManager.GetMapIndexStatus(i, j))
                    {
                        if (i > 0 && i < mainNode.Width - 1)
                        {
                            GameObject leftNode = mainNode.allCreatedNodes[i - 1, j];
                            GameObject rightNode = mainNode.allCreatedNodes[i + 1, j];
                            if (leftNode != null && rightNode != null)
                            {
                                if (leftNode.tag == currentNode.tag && rightNode.tag == currentNode.tag)
                                    SetMatchedNodes(leftNode, currentNode, rightNode);
                            }
                        }

                        if (j > 0 && j < mainNode.Height - 1)
                        {
                            GameObject upNode = mainNode.allCreatedNodes[i, j + 1];
                            GameObject downNode = mainNode.allCreatedNodes[i, j - 1];
                            if (upNode != null && downNode != null)
                            {
                                if (upNode.tag == currentNode.tag && downNode.tag == currentNode.tag)
                                    SetMatchedNodes(upNode, currentNode, downNode);
                            }
                        }

                    }
                }
            }
        }
    }

    private void SetMatchedNodes(GameObject node1, GameObject node2, GameObject node3)
    {
        AddMatchedNode(node1);
        AddMatchedNode(node2);
        AddMatchedNode(node3);
    }

    private void AddMatchedNode(GameObject node)
    {
        if (!currentNodeMatches.Contains(node))
            currentNodeMatches.Add(node);
        NodeController nodeController = node.GetComponent<NodeController>();
        if(nodeController != null)
            nodeController.isMatched = true;
    }
}
