using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class InGameOptions : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private Slider cameraSensitivity;


    [SerializeField]
    private CinemachineInputProvider cmInput;
    
    public void Start()
    {
        cameraSensitivity.value = PlayerPrefs.GetFloat("sensitivity", 5f);
    }

    public void OnRespawn()
    {
        player.Kill();
        OnBack();
    }
    
    public void OnSliderChanged(float val)
    {
        Debug.Log(val);
        PlayerPrefs.SetFloat("sensitivity", val);
    }

    public void OnBack()
    {
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
        gameObject.SetActive(false);
        cmInput.enabled = true;
    }
}
