using UnityEngine;
using UnityEngine.Events;

public class AnimationCompleteCallback : MonoBehaviour
{
    public UnityEvent FlipSwitch;
    
    public UnityEvent OnComplete;

    public void OnAnimationComplete()
    {
        OnComplete?.Invoke();
    }

    public void OnFlipSwitch()
    {
        FlipSwitch?.Invoke();
    }
    
}
