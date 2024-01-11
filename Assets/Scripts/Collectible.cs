using UnityEngine;
using UnityEngine.Events;

public class Collectible : MonoBehaviour
{

    public UnityEvent OnCollect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnCollect?.Invoke();
            Destroy(gameObject);
        }
    }
}
