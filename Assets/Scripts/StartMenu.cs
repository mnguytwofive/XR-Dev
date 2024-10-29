using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void PlayGame(){
        SceneManager.LoadScene("SelectionSortDemo");
    }

    public void QuitGame(){
        Application.Quit();
    }
}
