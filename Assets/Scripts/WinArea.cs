using UnityEngine;
using UnityEngine.SceneManagement;

public class WinArea : MonoBehaviour
{
    public bool canWin;

    private void OnTriggerEnter(Collider other)
    {
        if (canWin && other.gameObject.CompareTag("Player"))
        {
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("WinScreen");
        }
    }
}
