using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager gameManager;

    public int horizontalGridValue;
    public int verticalGridValue;
    public int maxScoreValue;
    public bool isEndless = false;

    public static GameManager Instance
    {
        get
        {
            return gameManager;
        }
    }

    private void Awake()
    {
        if (gameManager)
        {
            DestroyImmediate(gameObject);
            return;
        }
        gameManager = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetGameValues(int horiVal, int vertVal)
    {
        horizontalGridValue = horiVal;
        verticalGridValue = vertVal;
    }

    public void SetGameValues(int horiVal, int vertVal, int scoreVal)
    {
        horizontalGridValue = horiVal;
        verticalGridValue = vertVal;
        maxScoreValue = scoreVal;
    }

    public static void LoadNewLevel(string lvl)
    {
        Instance.StartCoroutine(LoadLevelAsync(lvl));
    }

    private static IEnumerator LoadLevelAsync(string lvl)
    {
        AsyncOperation asyn = SceneManager.LoadSceneAsync(lvl);

        while (!asyn.isDone)
        {
            //float loadBarProgress = Mathf.Clamp01(asyn.progress / 0.9f);
            //loadBar.value = loadBarProgress;
            yield return null;
        }
    }
}
