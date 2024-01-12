using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinArea : MonoBehaviour
{
    public bool canWin;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (canWin)
            {
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene("WinScreen");
            }
            else
            {
                UICanvas.Instance.SetPromptText("Collect 30 Cookies And Return Home!");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            UICanvas.Instance.SetDisappearingPromptText("Collect 30 Cookies And Return Home!");

        }
    }
}
