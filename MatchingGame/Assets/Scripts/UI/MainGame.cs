using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainGame : MonoBehaviour {
    [SerializeField]
    public Text txtScore;

    private UserData userData;
    SoundManager soundManager;

    private UnityAction actionScoreUI;

    // Use this for initialization
    void Start () {
        userData = UserData.GetInstance();
        soundManager = SoundManager.getInstance();
        actionScoreUI = new UnityAction(OnUpdateScoreUI);
        UpdateUI();
    }

    private void OnEnable()
    {
        if (actionScoreUI == null)
            actionScoreUI = new UnityAction(OnUpdateScoreUI);
        if(EventManager.instance != null)
            EventManager.StartListening(ConstantManager.EVENT_UPDATE_SCORE, OnUpdateScoreUI);
    }

    private void OnDisable()
    {
        if (EventManager.instance != null)
            EventManager.StopListening(ConstantManager.EVENT_UPDATE_SCORE, OnUpdateScoreUI);
    }

    void OnUpdateScoreUI()
    {
        if (userData != null && txtScore != null)
        {
            txtScore.text = Util.NumberFormat(userData.GetScore());
            PlayAnim();
        }
    }

    void UpdateUI()
    {
        if (userData != null && txtScore != null)
        {
            txtScore.text = Util.NumberFormat(userData.GetScore());
            PlayAnim();
        }
        //todo more others UI
    }

    void PlayAnim()
    {
        if (txtScore == null)
            return;
        Sequence seq = DOTween.Sequence();
        float duration = 0.3f;
        if (soundManager != null)
            soundManager.PlaySound(SoundId.FLY);
        seq.Append(txtScore.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), duration));
        seq.Append(txtScore.transform.DOScale(new Vector3(1f, 1f, 1f), duration));
    }
}
