using UnityEngine;

public class Fan : MonoBehaviour
{
    [SerializeField]
    private GameObject antPusher;

    [SerializeField]
    private KillArea fanBladesKillArea;

    [SerializeField]
    private ParticleSystem windParticles;
    
    [SerializeField]
    private bool running;

    private Animator animator;
    

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    

    // Update is called once per frame
    void Update()
    {
        if (!running)
        {
            animator.speed = 0;
            fanBladesKillArea.gameObject.SetActive(false);
            antPusher.SetActive(false);
            windParticles.Stop();
        }
        else
        {
            animator.speed = 1;
            antPusher.SetActive(true);
            fanBladesKillArea.gameObject.SetActive(true);
            if (!windParticles.isPlaying)
                windParticles.Play();
        }
    }
}
