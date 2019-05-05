using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour {

    [SerializeField]
    private TextMeshPro txtBestScore;

    [SerializeField]
    private TextMeshPro txtHint;

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
        txtHint.text = GetRandomHint();
        Util.PlayAnim(btnPlay, btnPlay.transform.localScale, duration);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.transform != null)
            {
                if ((hit.transform.gameObject.name == btnPlay.name))
                {
                    if (soundManager != null)
                        soundManager.PlaySound(SoundId.TOUCH);
                    hit.transform.localScale = new Vector3(0.7f, 0.7f, 0);
                    if (MainController.GetInstance())
                        MainController.GetInstance().SwitchScene(MainController.SCENE_MAIN_GAME);
                    Time.timeScale = 1;
                }
            }
        }
    }

    private string GetRandomHint()
    {
        List<string> hints = ConstantManager.GetHintConst();
        if (hints == null || hints.Count <= 0)
            return ConstantManager.HINT_DEFAULT;
        int random = Random.Range(0, hints.Count);
        return hints[random];
    }
}
