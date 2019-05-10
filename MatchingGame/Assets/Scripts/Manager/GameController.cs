using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    private static GameController instance;

    public MainNode mainNode;
    public NodeMatches nodeMatches;
    public MainGame mainGame;

    [SerializeField]
    private Text flyText;

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

    public IEnumerator FadeSpriteToFullAlpha(float t, Image img)
    {
        if(img != null)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
            while (img.color.a < 1.0f)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, img.color.a + (Time.deltaTime / t));
                yield return null;
            }
        }
    }

    public IEnumerator FadeSpriteToZeroAlpha(float t, Image img)
    {
        if(img != null)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
            while (img.color.a > 0.0f)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, img.color.a - (Time.deltaTime / t));
                yield return null;
            }
        }
    }

    public void ShowFlyScore(string txt, Transform trans = null, float time = 1f)
    {
        if (flyText == null)
            return;
        if (!flyText.gameObject.activeInHierarchy)
        {
            RectTransform flyTextTrans = flyText.gameObject.GetComponent<RectTransform>();
            const int DELTA = 2;
            Vector3 pos;
            Text text = flyText.gameObject.GetComponent<Text>();

            text.text = txt;
            flyText.gameObject.SetActive(true);
            float w = Util.calculateTextWidth(text.text, text);
            if (w >= Screen.width * 0.7f)
                w = Screen.width * 0.7f;
            Vector3 size = flyTextTrans.sizeDelta;
            size.x = w;
            flyTextTrans.sizeDelta = size;

            if (trans != null)
                pos = trans.transform.position;
            else
                pos = Util.GetPostConvert(Input.mousePosition);
            flyTextTrans.position = pos;
            pos.y += DELTA;
            Tweener t = flyTextTrans.DOMoveY(pos.y, time);
            t.OnComplete(() =>
            {
                flyText.gameObject.SetActive(false);
                EventManager.TriggerEvent(ConstantManager.EVENT_UPDATE_SCORE);
            });
        }
    }
}
