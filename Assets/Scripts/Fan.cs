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
    public bool running;

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
            if (animator != null)
                animator.speed = 0;
            if (fanBladesKillArea) 
                fanBladesKillArea.gameObject.SetActive(false);
            windParticles.Stop();
        }
        else
        {
            if (animator != null)
                animator.speed = 1;
            if (fanBladesKillArea)
                fanBladesKillArea.gameObject.SetActive(true);
            if (!windParticles.isPlaying)
                windParticles.Play();
        }
    }

    public void Activate()
    {
        running = true;
    }
}
