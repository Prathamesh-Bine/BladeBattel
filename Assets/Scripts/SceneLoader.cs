using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : Singleton<SceneLoader>
{
    private string sceneNameToBeLoaded;

    protected override void Awake()
   {
    base.Awake();
   }

    public void LoadScene(string _sceneName)
    {
        sceneNameToBeLoaded = _sceneName;
        StartCoroutine(InitializeSceneLoading());
    }

    IEnumerator InitializeSceneLoading()
    {
        // Open loading screen
        yield return SceneManager.LoadSceneAsync("Scene_Loading");

        // Start loading target scene
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation asyncSceneLoading =
            SceneManager.LoadSceneAsync(sceneNameToBeLoaded);

        asyncSceneLoading.allowSceneActivation = false;

        while (!asyncSceneLoading.isDone)
        {
            Debug.Log("Loading progress: " + asyncSceneLoading.progress);

            if (asyncSceneLoading.progress >= 0.9f)
            {
                asyncSceneLoading.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}