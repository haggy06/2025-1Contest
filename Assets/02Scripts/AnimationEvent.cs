using UnityEngine;
using UnityEngine.Events;

public class AnimationEvent : MonoBehaviour
{
    [SerializeField]
    private UnityEvent playerEvent;
    [SerializeField]
    private UnityEvent npcEvent;
    public void FromPlayerComplete()
    {
        playerEvent?.Invoke();
    }
    public void FromNPCComplete()
    {
        npcEvent?.Invoke();
    }
}
