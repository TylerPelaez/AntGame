using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu: MonoBehaviour
{

    [SerializeField]
    private Slider cameraSensitivity;


    public void Start()
    {
        cameraSensitivity.value = PlayerPrefs.GetFloat("sensitivity", 5f);
    }


    public void PlayGame() {  
        SceneManager.LoadScene("One Room");  
    }  
    public void QuitGame() {  
        Debug.Log("QUIT");  
        Application.Quit(); 
    } 
    public void OpenURL(string url) { 
        Application.OpenURL(url);
    }

    public void OnSliderChanged(float val)
    {
        PlayerPrefs.SetFloat("sensitivity", val);
    }
}  
