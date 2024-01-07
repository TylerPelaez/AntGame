using UnityEngine;
using UnityEngine.Events;

public class RollCanDirectionalTrigger : MonoBehaviour
{
    [SerializeField]
    private Vector3 rollDirection;

    public UnityEvent<Vector3> OnRollActive;
    public UnityEvent<Vector3> OnRollCancelled;

    private void Start()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnRollActive?.Invoke(rollDirection);
        }
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnRollActive?.Invoke(rollDirection);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnRollCancelled?.Invoke(rollDirection);
        }
    }
}
