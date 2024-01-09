using UnityEngine;
using UnityEngine.Events;

public class TriggerColliderProxy : MonoBehaviour
{
    public UnityEvent<Collider> onTriggerEnter;
    public UnityEvent<Collider> onTriggerExit;
    public UnityEvent<Collider> onTriggerStay;

    
    void OnTriggerEnter(Collider col)
    {
        if(onTriggerEnter != null) onTriggerEnter.Invoke(col);
    }

    void OnTriggerExit(Collider col)
    {
        if(onTriggerExit != null) onTriggerExit.Invoke(col);
    }
    
    void OnTriggerStay(Collider col)
    {
        if(onTriggerStay != null) onTriggerStay.Invoke(col);
    }
}
