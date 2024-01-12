using System;
using UnityEngine;
using UnityEngine.Events;

public class Collectible : MonoBehaviour
{

    public UnityEvent OnCollect;

    private int maxCollectibles;
    
    private void Awake()
    {
        var components = FindObjectsOfType<Collectible>();
        maxCollectibles = components.Length;
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            var components = FindObjectsOfType<Collectible>();
            
            UICanvas.Instance.SetDisappearingPromptText($"{maxCollectibles - components.Length + 1} / {maxCollectibles} Components Found!");
            OnCollect?.Invoke();
            Destroy(gameObject);
        }
    }
}
