using System.Collections;  
using System.Collections.Generic;  
using UnityEngine;  
using UnityEngine.SceneManagement;  

public class MainMenu: MonoBehaviour {  
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
}  
