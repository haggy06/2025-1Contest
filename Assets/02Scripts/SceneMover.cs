using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneMover
{
    private static SCENE targetScene = SCENE.Main;

    public static void MoveScene()
    {
        if (SceneManager.GetActiveScene().buildIndex == (int)SCENE.Loading) // ���� �ε� ���� ���� ���
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
            Debug.LogError("Loading���� ���� �������� ������ �� ����");
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