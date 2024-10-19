using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    public Button SelectionSortDemo; 
    public Button InteractiveSelectionSort;

    public Transform MainMenuCamera;

    void Start()
    {
        // Add an OnClick listener to the button
        SelectionSortDemo.onClick.AddListener(LoadDemoMode);

        // Add an OnClick listener to the button
        InteractiveSelectionSort.onClick.AddListener(LoadInteractiveMode);

        Vector3 cameraPosition = new Vector3(0, 10, -10); //create location for the camera 
        MainMenuCamera.position = cameraPosition; //set the camera to this postion

    }

    // Function to be called when the Demo Mode button is clicked
    public void LoadDemoMode()
    {
        SceneManager.LoadScene("SelectionSortDemo"); // Replace with the name of your demo scene
    }

    // Function to be called when the Interactive Mode button is clicked
    public void LoadInteractiveMode()
    {
        SceneManager.LoadScene("InteractiveSelectionSort"); // Replace with the name of your interactive scene
    }

    // Optional: Function to quit the game (for standalone builds)
    public void QuitGame()
    {
        Application.Quit();
    }
}