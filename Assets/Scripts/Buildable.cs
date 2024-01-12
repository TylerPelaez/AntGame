using UnityEngine;

public class Buildable : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private ParticleSystem buildParticles;

    private void OnTriggerEnter(Collider other)
    {
        // Show UI For Building pieces missing
    }
}
