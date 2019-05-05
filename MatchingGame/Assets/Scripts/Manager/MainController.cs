using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour {
    private static MainController mainController;
    public static string SCENE_INIT = "Init";
    public static string SCENE_MAIN_GAME = "Main";

    private string currentSceneName;
    private string nextSceneName;

    private AsyncOperation resourceUnloadTask;
    private AsyncOperation sceneLoadTask;

    protected void Awake()
    {
        mainController = this;
    }

    private void Start()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene != null)
            currentSceneName = scene.name;
    }

    public static MainController GetInstance()
    {
        return mainController;
    }

    public void SwitchScene(string _nextSceneName)
    {
        if (mainController != null && !string.IsNullOrEmpty(_nextSceneName))
        {
            if (currentSceneName != _nextSceneName)
            {
                currentSceneName = nextSceneName;
                this.nextSceneName = _nextSceneName;
                StartCoroutine(ChangeScene());
            }
            
        }
    }

    IEnumerator ChangeScene()
    {
        sceneLoadTask = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);
        sceneLoadTask.allowSceneActivation = false;
        while (!sceneLoadTask.isDone)
        {
            if (sceneLoadTask.progress >= 0.9f)
                sceneLoadTask.allowSceneActivation = true;
            yield return null;
        }
        sceneLoadTask.allowSceneActivation = true;
    }
}
