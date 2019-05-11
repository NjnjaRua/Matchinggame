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

    public float duration = 0.5f;
    SoundManager soundManager;

    // Use this for initialization
    void Start () {
        soundManager = SoundManager.getInstance();
        StartCoroutine(Init());
	}
	
	IEnumerator Init()
    {
        yield return new WaitForSeconds(0.2f);
        if (soundManager != null)
            soundManager.PlaySound(SoundId.PLAYING, true);
        txtBestScore.text = (GameSave.GetInstance() ? Util.NumberFormat(GameSave.GetInstance().GetBestScore()) : Util.NumberFormat(0));
        Util.PlayAnim(btnPlay, btnPlay.transform.localScale, duration);
    }

    public void OnPlay()
    {
        if (soundManager != null)
            soundManager.PlaySound(SoundId.TOUCH);
        if (MainController.GetInstance())
            MainController.GetInstance().SwitchScene(MainController.SCENE_MAIN_GAME);
    }
}
