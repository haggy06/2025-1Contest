using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TutorialBlibdManager : MonoBehaviour
{
    [SerializeField]
    private GameObject pointer3D;

    [Space(5)]
    [SerializeField]
    private GameObject pointer2D;
    [SerializeField]
    private Vector2[] pointerVec;

    private void Awake()
    {
        pointer2D.SetActive(false);
        pointer3D.SetActive(false);

        GameManager.Inst.Subscribe(EventType.TutorialStart, TutorialStart);
        GameManager.Inst.Subscribe(EventType.TutorialNext, TutorialNext);
    }

    private void TutorialStart()
    {
        pointer3D.SetActive(true);
    }
    private void TutorialNext()
    {
        pointer3D.SetActive(false);
        switch (GameManager.Inst.tutorialState)
        {
            case TutorialState.OrderCheck:
                MovePointer2D(0);
                break;

            case TutorialState.GoShop:
                MovePointer2D(1);
                break;

            case TutorialState.GoInventory:
                MovePointer2D(2);
                break;

            case TutorialState.GoForge:
                MovePointer2D(3);
                break;

            case TutorialState.Bell:
                MovePointer2D(4);
                break;

            case TutorialState.GoInfo:
                MovePointer2D(5);
                break;

            default:
                pointer2D.SetActive(false);
                break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void MovePointer2D(int pointerVecIndex)
    {
        pointer2D.SetActive(true);
        pointer2D.transform.position = pointerVec[pointerVecIndex];
    }
}
