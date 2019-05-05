using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSave : MonoBehaviour {
    private static GameSave instance;

    int bestScore;

    private void Awake()
    {
        instance = this;
    }

    public static GameSave GetInstance()
    {
        return instance;
    }

    // Use this for initialization
    void Start () {
        bestScore = PlayerPrefs.GetInt(ConstantManager.SAVE_BEST_SCORE);
	}
	

    public void SetBestScore(int score)
    {
        Debug.Log("SetBestScore " + score);
        if (score <= 0 || bestScore - score >= 0)
            return;
        bestScore = score;
        PlayerPrefs.SetInt(ConstantManager.SAVE_BEST_SCORE, bestScore);
        PlayerPrefs.Save();
    }

    public int GetBestScore()
    {
        bestScore = PlayerPrefs.GetInt(ConstantManager.SAVE_BEST_SCORE);
        return bestScore;
    }
}
