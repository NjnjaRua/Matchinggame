using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    [SerializeField]
    private Text txtBestScore;

    [SerializeField]
    private GameObject btnPlay;

    [SerializeField]
    private GameObject btnFBConnect;

    [SerializeField]
    private GameObject btnFbConnectd;

    [SerializeField]
    private FacebookDisplay fbDisplay;

    public float duration = 0.5f;
    SoundManager soundManager;
    FacebookManager fbManager;

    // Use this for initialization
    void Start () {
        soundManager = SoundManager.getInstance();
        fbManager = FacebookManager.GetInstance();
        StartCoroutine(Init());
	}
	
	IEnumerator Init()
    {
        yield return new WaitForSeconds(0.2f);
        if (soundManager != null)
            soundManager.PlaySound(SoundId.PLAYING, true);
        txtBestScore.text = (GameSave.GetInstance() ? Util.NumberFormat(GameSave.GetInstance().GetBestScore()) : Util.NumberFormat(0));
        UpdateFacebookUI();
        Util.PlayAnim(btnPlay, btnPlay.transform.localScale, duration);
    }

    public void OnPlay()
    {
        if (soundManager != null)
            soundManager.PlaySound(SoundId.TOUCH);
        if (MainController.GetInstance())
            MainController.GetInstance().SwitchScene(MainController.SCENE_MAIN_GAME);
    }

    public void ConnectFB()
    {
        if (fbManager == null)
            fbManager = FacebookManager.GetInstance();
        if (fbManager == null)
            return;
        fbManager.FBLogin(isSuccess => {
            if (isSuccess)
                UpdateFacebookUI();
        });
    }

    void UpdateFacebookUI()
    {
        if (fbManager == null)
            fbManager = FacebookManager.GetInstance();
        if (fbManager == null)
            return;
        if (fbManager.IsFBConnected())
        {
            btnFBConnect.SetActive(false);
            btnFbConnectd.SetActive(true);
            fbDisplay.UpdateFacebookUI();
        }
        else
        {
            btnFBConnect.SetActive(true);
            btnFbConnectd.SetActive(false);
        }
    }
}
