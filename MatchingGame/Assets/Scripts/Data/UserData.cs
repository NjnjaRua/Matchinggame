using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData : MonoBehaviour {
    private static UserData instance;

    private long score;
    public long Score
    {
        get { return score; }
        set { score = value; }
    }

    bool isInit;

    private void Awake()
    {
        instance = this;
        Init();
    }

    private void OnApplicationQuit()
    {
        QuitGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif

        }
    }

    public static UserData GetInstance()
    {
        return instance;
    }


    void Init()
    {
        if(!isInit)
        {
            isInit = true;
            Score = 0;
        }
    }

    #region Score
    public void IncreaseScore(long _score, bool isTrigger = true)
    {
        if (_score <= 0)
            return;
        score += _score;
        if(isTrigger)
            EventManager.TriggerEvent(ConstantManager.EVENT_UPDATE_SCORE);
    }

    public void DecreaseScore(long _score, bool isTrigger = true)
    {
        if (_score <= 0)
            return;
        score -= _score;
        if (score <= 0)
            score = 0;
        if(isTrigger)
            EventManager.TriggerEvent(ConstantManager.EVENT_UPDATE_SCORE);
    }

    public long GetScore()
    {
        return Score;
    }
    #endregion

    #region Quit
    private void QuitGame()
    {
        if (SoundManager.getInstance())
            SoundManager.getInstance().PlaySound(SoundId.QUIT);
        if (GameSave.GetInstance())
        {
            //todo: save best score with FireBase or other local
            GameSave.GetInstance().SetBestScore((int)GetScore());
        }
    }
    #endregion
}
