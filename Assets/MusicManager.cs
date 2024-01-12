using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField]
    private Transform radio1, radio2;

    [SerializeField]
    private AudioSource audioSource;

    private Transform player;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var playerPos = player.position;
        var rad1distSq = (playerPos - radio1.position).sqrMagnitude;
        var rad2distSq = (playerPos - radio2.position).sqrMagnitude;
        audioSource.transform.position = rad1distSq < rad2distSq ? radio1.position : radio2.position;
    }
}
