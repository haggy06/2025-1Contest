using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneMover
{
    private static SCENE targetScene = SCENE.Main;

    public static void MoveScene()
    {
        if (SceneManager.GetActiveScene().buildIndex == (int)SCENE.Loading) // 현재 로딩 씬에 있을 경우
        {
            SceneManager.LoadSceneAsync((int)targetScene);
        }
        else
        {
            SceneManager.LoadScene((int)SCENE.Loading);
        }
    }
    public static void MoveScene(SCENE targetScene)
    {
        if (targetScene == SCENE.Loading)
        {
            Debug.LogError("Loading씬을 최종 목적지로 지정할 수 없음");
            return;
        }

        SceneMover.targetScene = targetScene;

        MoveScene();
    }
}

public enum SCENE
{
    Main,
    Loading,
    Ingame,
    End
}