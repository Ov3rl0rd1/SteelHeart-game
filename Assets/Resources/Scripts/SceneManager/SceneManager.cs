using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class SceneManager : MonoBehaviour
{
    public UnityEvent<SceneLoad> OnLoadScene;

    [Inject] private SaveManager _saveManager;

    private List<string> _scenes = new List<string>();

    public string NextScene() => _scenes[GetNextSceneIndex()];

    public void ReloadCurrent()
    {
        AsyncLoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, true);
    }

    public void LoadMenu()
    {
        AsyncLoadScene(_scenes[0], true);
    }

    public AsyncOperation LoadNext(bool callback = true)
    {
        return AsyncLoadScene(NextScene(), callback);
    }

    public void LoadFromSave()
    {
        PlayerSaveData playerSaveData = _saveManager.Load();
        AsyncLoadScene(playerSaveData.Scene, true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private AsyncOperation AsyncLoadScene(string name, bool callback)
    {
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);

        if(callback)
            OnLoadScene.Invoke(new SceneLoad(asyncOperation, name));

        return asyncOperation;
    }

    private void Awake()
    {
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < sceneCount; i++)
            _scenes.Add(Path.GetFileNameWithoutExtension(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i)));
    }

    private int GetNextSceneIndex()
    {
        int currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

        int nextScene = 0;

        if (currentScene < _scenes.Count - 1)
            nextScene = currentScene + 1;

        return nextScene;
    }
}

public class SceneLoad
{
    public readonly string Name;
    public readonly AsyncOperation Operation;

    public SceneLoad(AsyncOperation operation, string name)
    {
        Name = name;
        Operation = operation;
    }
}
