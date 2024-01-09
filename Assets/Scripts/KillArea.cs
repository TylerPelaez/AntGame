using UnityEngine;

public class KillArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (enabled && other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().Kill();
        }
    }
}
