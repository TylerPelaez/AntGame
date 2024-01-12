using UnityEngine;
using UnityEngine.Events;

public class FoodItem : MonoBehaviour
{

    public UnityEvent OnCollect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {            
            var components = FindObjectsOfType<FoodItem>();
            if (components.Length == 1)
            {
                UICanvas.Instance.SetDisappearingPromptText($"All Cookies Found! Return Home!");
            }
            OnCollect?.Invoke();
            Destroy(gameObject);
        }
    }
}
