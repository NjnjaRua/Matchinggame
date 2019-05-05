using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour {
    private static GameController instance;

    public MainNode mainNode;
    public NodeMatches nodeMatches;
    public MainGame mainGame;

    [SerializeField]
    private TextMeshPro flyText;

    private int textMeshProSortingLayer = 20;

    private void Awake()
    {
        instance = this;
        if (mainNode == null)
            mainNode = FindObjectOfType<MainNode>();
        if (nodeMatches == null)
            nodeMatches = FindObjectOfType<NodeMatches>();
    }
    

    public static GameController GetInstance()
    {
        return instance;
    }

    public IEnumerator FadeSpriteToFullAlpha(float t, SpriteRenderer spriteRenderer)
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
        while (spriteRenderer.color.a < 1.0f)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    public IEnumerator FadeSpriteToZeroAlpha(float t, SpriteRenderer spriteRenderer)
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
        while (spriteRenderer.color.a > 0.0f)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }

    public void ShowFlyScore(string txt, Transform trans = null, float time = 1)
    {
        if (flyText == null)
            return;
        if (!flyText.gameObject.activeInHierarchy)
        {
            Transform flyTextTrans = flyText.gameObject.GetComponent<Transform>();
            const int DELTA = 10;
            Vector3 pos;
            TextMeshPro text = flyText.gameObject.GetComponent<TextMeshPro>();
            text.sortingOrder = textMeshProSortingLayer;
            text.text = txt;
            flyText.gameObject.SetActive(true);
            if(trans != null)
                pos = trans.transform.position;
            else
                pos = Util.GetPostConvert(Input.mousePosition);
            flyTextTrans.position = pos;
            if (mainGame != null && mainGame.txtScore != null)
                pos = mainGame.txtScore.transform.position;
            else
                pos.y += DELTA;
            Tweener t = flyTextTrans.DOMove(pos, time);
            t.OnComplete(() =>
            {
                flyText.gameObject.SetActive(false);
                EventManager.TriggerEvent(ConstantManager.EVENT_UPDATE_SCORE);
            });
        }
    }
}
