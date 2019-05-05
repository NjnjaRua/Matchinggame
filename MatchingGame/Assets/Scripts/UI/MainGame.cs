using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MainGame : MonoBehaviour {
    [SerializeField]
    public TextMeshPro txtScore;

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
        EventManager.StartListening(ConstantManager.EVENT_UPDATE_SCORE, OnUpdateScoreUI);
    }

    private void OnDisable()
    {
        EventManager.StopListening(ConstantManager.EVENT_UPDATE_SCORE, OnUpdateScoreUI);
    }

    void OnUpdateScoreUI()
    {
        if(userData != null)
            txtScore.text = Util.NumberFormat(userData.GetScore());
    }

    void UpdateUI()
    {
        if(userData != null)
            txtScore.text = Util.NumberFormat(userData.GetScore());
        //todo more others UI
    }
}
