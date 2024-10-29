using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class InteractiveSelectionSort : MonoBehaviour
{
    public Button GenerateList;
    public GameObject rectangle;
    public Transform InteractiveCamera;
    bool GenerateListAlreadyClicked = false;

    // List to store the dynamically created cubes
    private List<GameObject> RectangleList = new List<GameObject>();

    //List to dynamically store each rectangles number
    private List<int> rect_Num_In_List = new List<int>();
    
    void Start()
    {
        // Add an OnClick listener to the button
        GenerateList.onClick.AddListener(GenerateTheList);

        
    }

    public void GenerateTheList()
    {
        if (GenerateListAlreadyClicked)
        {
            foreach (GameObject rect in RectangleList) //get rid of every cube in the scene
            {
                Destroy(rect); // Destroy the GameObject in the scene
            }

            RectangleList.Clear();
            GenerateListAlreadyClicked = false;
        }
        else
        {
    
            // Create an instance of the Random class
            System.Random random = new System.Random();
            int distanceBetweenRectangles = 0; 

            // Generate a random number between 3 (inclusive) and 11 (exclusive)
            int randomListSize = random.Next(3, 8);

            for (int i = 0; i < randomListSize; i++)
            {
                Vector3 rectanglePosition = new Vector3(distanceBetweenRectangles, 0, 0); //get a position for a rectangle 
                GameObject newRect = Instantiate(rectangle, rectanglePosition, Quaternion.identity); //create the rectangle

                RectangleList.Add(newRect);
                
                TextMeshPro tmp = newRect.GetComponentInChildren<TextMeshPro>(); //get the rectangle's text box object
                tmp.text = random.Next(0,99).ToString(); //give it its text 

                distanceBetweenRectangles += 5; 
            }

            //calculate camera postion based on total list objects 
            float cameraHeight = (float) randomListSize;
            float cameraDistanceFromList = (float) randomListSize * -3f;
            float middleX = distanceBetweenRectangles / 2;

            Vector3 cameraPosition = new Vector3(middleX, cameraHeight, cameraDistanceFromList); //create location for the camera 
            InteractiveCamera.position = cameraPosition; //set the camera to this postion

            GenerateListAlreadyClicked = true;

        }
    }

}