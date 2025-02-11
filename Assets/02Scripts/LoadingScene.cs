using UnityEngine;

public class LoadingScene : MonoBehaviour
{
    [SerializeField]
    private float minLoadTime = 1f;

    private void Awake()
    {
        Invoke("MoveToTargetScene", minLoadTime);
    }

    private void MoveToTargetScene()
    {
        SceneMover.MoveScene();
    }
}
