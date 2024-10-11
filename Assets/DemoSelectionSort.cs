using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 
    Author: David Leon
*/

public class DemoSelectionSort : MonoBehaviour
{

    // Reference to the Cube prefab
    public GameObject Number_Rect;

    // List to store the dynamically created cubes
    private List<GameObject> cubesList = new List<GameObject>();

    //List to dynamically store each rectangles number
    private List<int> rect_Num_In_List = new List<int>();
  

    //Sort from least to greatest button
    public Button SortLeastGreatest;
    public bool startSortingLeastGreatest = false;

    //Sort from greatest to least button
    public Button SortGreatestLeast;
    public bool startSortingGreatestLeast = false;

    //Sort Alphabetically 
    public Button SortAlphabetically;
    public bool startSortingAlphabetically = false;
    
    //User input
    public TMP_InputField InputUserList;
    public bool userListEntered = false;
    public bool listNotProcessed = true;

    //check for a valid list 
    public bool validList = false;

    // the arrow reference 
    public GameObject currentPositionArrow;
    public GameObject runtime_currentPositionArrow;
    
    
    public int distanceBetweenRectangles = 0; //increments the distance between rectagles 
    public int middleX = 0; //the middle of sorting algorithm
    public Transform Camera_SA; //my camera object 
    public float TempHeight = -1.577f; //fixed height for temp square 
    public float TempDistance = 5;
    public float OppTempDistance = -15;
    public GameObject temp; //temp square object 
    public int currentListPosition = 0; //the current postion in list (to scan everything to the right of it )
    public bool cycle = false;
   
    //these are cycle checks 
    public bool temp_position_Not_Reached = true;
    public bool current_List_Position_Not_Reached = true;
    public bool phase_Not_Complete = true;
    public bool cycle_Not_Complete = true;

    //must check in the current index we are at is the smallest number
    bool check_If_Curr_Is_Min = false;

    //threshold value:
    public float THRESHOLD = 0.2f;


    //we must save the minimum index 
    public int minIndexToSave = 0;
   
   //speed of the blocks
    public float speed = 10.0f;

    int min = 0;
    int minIndex = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        
        // Add an OnClick listener to the button
        SortLeastGreatest.onClick.AddListener(StartAutoSortingLeastGreatest);

        // Add an OnClick listener to the button
        SortGreatestLeast.onClick.AddListener(StartAutoSortingGreatestLeast);

        // Add an OnClick listener to the button
        SortAlphabetically.onClick.AddListener(StartAutoSortingAlphabetically);

        if (InputUserList == null)
        {
            Debug.LogError("InputUserList is not found!");
        }
        else
        {
            // Add listener to detect when the user has finished editing
            InputUserList.onEndEdit.AddListener(OnTextSubmitted);
        }


    }





    // Update is called once per frame
    void Update()
    {


        if (userListEntered) //if the list has been entered then we go in here 
        {

            if (listNotProcessed) //if the list has not been processed yet then we must process it 
            {
                if(ProcessInput()) //process the list 
                { //if have a successfull process of list 
                    //move the list up (to make it "disapear)
                    Vector3 currentInputListPosition = InputUserList.transform.position;
                    currentInputListPosition.y += 300; // Increase the y position by 300
                    InputUserList.transform.position = currentInputListPosition; // Assign the new position

                    //create a new current position arrow 
                    CreateArrowAtPosition(getCurrPosition(rect_Num_In_List));
                    Vector3 currentArrowPos = runtime_currentPositionArrow.transform.position;
                    currentArrowPos.y += 5;
                    runtime_currentPositionArrow.transform.position = currentArrowPos; //I want the arrow above the rectangle 

                    validList = true;
                    listNotProcessed = false;
                }
                else
                { //make the user insert a valid list 
                    userListEntered = false;

                }

            }
            else if (startSortingLeastGreatest || startSortingGreatestLeast || startSortingAlphabetically)  // when the user presses auto sort we go in here and start sorting the list 
            {
                if (currentListPosition < rect_Num_In_List.Count) //we dont want to go out of bounds
                {

            
                    GameObject rect = cubesList[currentListPosition]; //get the rectangle in the current position

                    Vector3 arrowPointPosition = rect.transform.position; //get the rectangles position

                    min = rect_Num_In_List[currentListPosition]; //get the minimum value
                    minIndex = currentListPosition; // get the minimum index 

                    int minimum_Val_Index = currentListPosition;
                    int minimum_Val_Check = rect_Num_In_List[currentListPosition];

                    if (startSortingLeastGreatest || startSortingAlphabetically) //if we are sorting from least to greatest 
                    {
                        /* I want to find the smallest number in the list to see if its the current postion arrow */
                        for (int i = currentListPosition; i < rect_Num_In_List.Count; i++)
                        {
                            if (minimum_Val_Check > rect_Num_In_List[i])
                            {
                                minimum_Val_Index = i;
                            }
                        }

                        if (minimum_Val_Index == currentListPosition) //if nothing has changed then currentListPosition index is the smallest value in list
                        {
                            check_If_Curr_Is_Min = true;
                        }
                    }
                    else if (startSortingGreatestLeast) //if we are sorting from greatest to least 
                    {
                        /* I want to find the greatest number in the list to see if its the current postion arrow */
                        for (int i = currentListPosition; i < rect_Num_In_List.Count; i++)
                        {
                            if (minimum_Val_Check < rect_Num_In_List[i])
                            {
                                minimum_Val_Index = i;
                            }
                        }

                        if (minimum_Val_Index == currentListPosition) //if nothing has changed then currentListPosition index is the smallest value in list
                        {
                            check_If_Curr_Is_Min = true;
                        }
                    }
                    
                    /* STARTING INDEX MOVING TO THE TEMP SPOT */
                    if(temp_position_Not_Reached) //we go in here unitl we have reached the temp spot 
                    {
                        
                    
                        if (Mathf.Abs(rect.transform.position.z - TempDistance) > THRESHOLD) //GOING FOWARD TOWARDS TEMP
                        {
                            // DEBUG INFO --------------------------------------------------------------------
                            Debug.Log("curr min position going to temp: " + rect.transform.position.z);
                            Debug.Log("temp: " + TempDistance);
                            Debug.Log("DIFFERENCE: " + Mathf.Abs(rect.transform.position.z - TempDistance));
                            Debug.Log("IN TEMP GOING FORWARD");
                            // DEBUG INFO --------------------------------------------------------------------

                            rect.transform.Translate(Vector3.forward * speed * Time.deltaTime); //move up (+ z-axis)

                        }
                        else if (Mathf.Abs(rect.transform.position.x - middleX) > THRESHOLD) //CHECK TO GO LEFT (+ result) OR RIGHT (- result)
                        {
                            if (rect.transform.position.x > middleX) //GOING LEFT TOWARDS TEMP
                            {   
                                // DEBUG INFO --------------------------------------------------------------------
                                Debug.Log("curr min position going to temp (left): " + rect.transform.position.x);
                                Debug.Log("middleX: " + middleX);
                                Debug.Log("DIFFERENCE: " + Mathf.Abs(rect.transform.position.x - middleX));
                                Debug.Log("IN TEMP GOING LEFT");
                                // DEBUG INFO --------------------------------------------------------------------

                                rect.transform.Translate(Vector3.left  * speed * Time.deltaTime); //move left (- x-axis)
                            }
                            else //GOING RIGHT TOWARDS TEMP
                            {   
                                // DEBUG INFO --------------------------------------------------------------------
                                Debug.Log("curr min position going to temp (right): " + rect.transform.position.x);
                                Debug.Log("middleX: " + middleX);
                                Debug.Log("DIFFERENCE: " + Mathf.Abs(rect.transform.position.x - middleX));
                                Debug.Log("IN TEMP GOING RIGHT");
                                // DEBUG INFO --------------------------------------------------------------------

                                rect.transform.Translate(Vector3.right  * speed *  Time.deltaTime); //move right (+ x-axis)
                            }
                        }
                        /* check if we can move on to moving the smallest value in current list postion */
                        if (Mathf.Abs(rect.transform.position.x - middleX) <= THRESHOLD && Mathf.Abs(rect.transform.position.z - TempDistance) <= THRESHOLD)
                        {
                            temp_position_Not_Reached = false;
                    
                        }
                    }
                    else if (check_If_Curr_Is_Min) //if the current list postion is the  smallest value I want to move it to temp and then right back where it was 
                    {
                        
                        
                        Vector3 minIndexPreviousPosition = getCurrPosition(rect_Num_In_List); //safely get curr position 

                        if (Mathf.Abs(rect.transform.position.x - minIndexPreviousPosition.x) > THRESHOLD) //move rect in temp to its original location 
                        {
                            if (minIndexPreviousPosition.x > rect.transform.position.x) //if arrow's current postion is to the left of us  { location .  < . temp (rect.transform) . < . location }
                            {
                                // DEBUG INFO --------------------------------------------------------------------
                                Debug.Log("curr min position from temp to its original position (moving right): " + rect.transform.position.x);
                                Debug.Log("previous pos: " + minIndexPreviousPosition.x);
                                Debug.Log("DIFFERENCE: " + Mathf.Abs(rect.transform.position.x - minIndexPreviousPosition.x));
                                Debug.Log("IN CHECK_IF_CURR_IS_MIN GOING RIGHT BACK TO ORIGINAL POSITION FROM TEMP");
                                // DEBUG INFO --------------------------------------------------------------------

                                rect.transform.Translate(Vector3.right  * speed * Time.deltaTime); //move right (+ x-axis)
                            }
                            else
                            {
                                // DEBUG INFO --------------------------------------------------------------------
                                Debug.Log("curr min position from temp to its original position (moving left): " + rect.transform.position.x);
                                Debug.Log("previous pos: " + minIndexPreviousPosition.x);
                                Debug.Log("DIFFERENCE: " + Mathf.Abs(rect.transform.position.x - minIndexPreviousPosition.x));
                                Debug.Log("IN CHECK_IF_CURR_IS_MIN GOING LEFT BACK TO ORIGINAL POSITION FROM TEMP");
                                // DEBUG INFO --------------------------------------------------------------------

                                rect.transform.Translate(Vector3.left  * speed *  Time.deltaTime); //move left (- x-axis) 
                            }
                        }
                        else if (Mathf.Abs(rect.transform.position.z - minIndexPreviousPosition.z) > THRESHOLD)
                        {
                            // DEBUG INFO --------------------------------------------------------------------
                            Debug.Log("curr min position from temp to its original position going down: " + rect.transform.position.z);
                            Debug.Log("previous pos z: " + minIndexPreviousPosition.z);
                            Debug.Log("DIFFERENCE: " + Mathf.Abs(rect.transform.position.z - minIndexPreviousPosition.z));
                            Debug.Log("IN CHECK_IF_CURR_IS_MIN GOING DOWN");
                            // DEBUG INFO --------------------------------------------------------------------

                            rect.transform.Translate(Vector3.back * speed * Time.deltaTime); //move down (- z-axis)
                        }

                        

                        //check if the cycle has been complete 
                        if (Mathf.Abs(rect.transform.position.x - minIndexPreviousPosition.x) <= THRESHOLD && Mathf.Abs(rect.transform.position.z - minIndexPreviousPosition.z) <= THRESHOLD)
                        {
                            //we want to reset the cycle 
                            current_List_Position_Not_Reached = true;
                            temp_position_Not_Reached = true;
                            check_If_Curr_Is_Min = false;
                            currentListPosition++;

                            if (currentListPosition != rect_Num_In_List.Count) //we dont want to go in here when we are at the end of the list 
                            {
                                //update the arrow position 
                                UpdateArrowPosition(getCurrPosition(rect_Num_In_List));

                                //I want the arrow above the rectangle  
                                Vector3 currentArrowPos = runtime_currentPositionArrow.transform.position;
                                currentArrowPos.y += 5;
                                runtime_currentPositionArrow.transform.position = currentArrowPos; 
                            }
                            
                            //WHEN THE LIST HAS BEEN COMPLETELY SORTED WE GO IN HERE 
                            if (currentListPosition == rect_Num_In_List.Count) //if the list has been completely sorted 
                            {
                                //reset these for the next list the usŅr wants to enter 
                                userListEntered = false;
                                listNotProcessed = true;
                                startSortingLeastGreatest = false;
                                startSortingGreatestLeast = false;
                                startSortingAlphabetically = false;
                                validList = false;
                                currentListPosition = 0;
                                distanceBetweenRectangles = 0; //reset this again so the cubes are spaced out correctly 

                                foreach (GameObject cube in cubesList) //get rid of every cube in the scene
                                {
                                    Destroy(cube); // Destroy the GameObject in the scene
                                }

                                Destroy(runtime_currentPositionArrow); //destory the arrow 


                                //clear both lists
                                cubesList.Clear();
                                rect_Num_In_List.Clear();

                                
                                InputUserList.text = ""; // Clear the input field

                                /*make the input list visible again  */
                                Vector3 currentInputListPosition = InputUserList.transform.position;
                                currentInputListPosition.y -= 300; // Increase the y position by 300
                                InputUserList.transform.position = currentInputListPosition; // Assign the new position

                                moveButtonsDown(); //bring the buttons back down 
                            }
            

                        }
                      

                        
                    }
                    else 
                    {
                    /* MINIMUM INDEX HEADS TOWARD THE CURRENT POSITION IN THIS IF STATEMENT BELOW */
                        if (current_List_Position_Not_Reached)
                        {

                            if (startSortingLeastGreatest || startSortingAlphabetically) //if we are sorting from least to greatest go in here 
                            {
                                
                                for (int i = currentListPosition + 1; i < rect_Num_In_List.Count; i++)
                                {
                                
                                    if (min > rect_Num_In_List[i])
                                    {
                                        min = rect_Num_In_List[i];
                                        minIndex = i;
                                    }

                                } //when we exit this for loop we should have the min value

                            }
                            else if (startSortingGreatestLeast) //if we are sorting from greatest to least go in here
                            {
                                for (int i = currentListPosition + 1; i < rect_Num_In_List.Count; i++)
                                { //substitute min for max 
                                
                                    if (min < rect_Num_In_List[i]) // if (max < rect_Num_In_List[i])
                                    {
                                        min = rect_Num_In_List[i]; //max = rect_Num_In_List[i]
                                        minIndex = i; //maxIndex = i
                                    }

                                } //when we exit this for loop we should have the max value
                            }

                        Vector3 currPosVector = getCurrPosition(rect_Num_In_List);
                        

                            if ((Mathf.Abs(cubesList[minIndex].transform.position.z + TempDistance) > THRESHOLD) && phase_Not_Complete) //move away from the line 
                            {
                                // DEBUG INFO --------------------------------------------------------------------
                                Debug.Log("(move away from line:) cube min index z position: " + cubesList[minIndex].transform.position.z);
                                Debug.Log("(move away from line:) temp distance: " + TempDistance);
                                Debug.Log("DIFFERENCE: " + Mathf.Abs(cubesList[minIndex].transform.position.z + TempDistance));
                                Debug.Log("IN CUBE GOING BACK TO CURRENT POSTION AND GOING AWAY");
                                // DEBUG INFO --------------------------------------------------------------------

                                cubesList[minIndex].transform.Translate(Vector3.back * speed *  Time.deltaTime); //move down (- z-axis)
                                
                            }
                            else if (Mathf.Abs(cubesList[minIndex].transform.position.x - currPosVector.x) > THRESHOLD)
                            {
                                // DEBUG INFO --------------------------------------------------------------------
                                Debug.Log(" cube min index x position: " + cubesList[minIndex].transform.position.x);
                                Debug.Log("current position index x position: " + currPosVector.x);
                                Debug.Log("DIFFERENCE: " + Mathf.Abs(cubesList[minIndex].transform.position.x - currPosVector.x));
                                Debug.Log("IN CUBE GOING BACK TO CURRENT POSTION (LEFT)");
                                // DEBUG INFO --------------------------------------------------------------------

                                cubesList[minIndex].transform.Translate(Vector3.left  * speed * Time.deltaTime); //move left (- x-axis)
                                phase_Not_Complete = false;
                                
                            }
                            else if (Mathf.Abs(cubesList[minIndex].transform.position.z - currPosVector.z) > THRESHOLD) //move up toward currListPosition ( + z-axis)
                            {
                                // DEBUG INFO --------------------------------------------------------------------
                                Debug.Log("cube min index z position: " + cubesList[minIndex].transform.position.z);
                                Debug.Log("current position index z position: " + currPosVector.z);
                                Debug.Log("DIFFERENCE: " + Mathf.Abs(cubesList[minIndex].transform.position.z - currPosVector.z));
                                Debug.Log("IN CUBE GOING UP TO CURRENT POSTION");
                                // DEBUG INFO --------------------------------------------------------------------

                                cubesList[minIndex].transform.Translate(Vector3.forward  * speed * Time.deltaTime);

                            }

                            //check each criteria 
                            bool condition2 = Mathf.Abs(cubesList[minIndex].transform.position.x - currPosVector.x) <= THRESHOLD;
                            bool condition3 = Mathf.Abs(cubesList[minIndex].transform.position.z - (arrowPointPosition.z - TempDistance)) <= THRESHOLD;


                            if (condition2 && condition3) //if both conditions are meet the minimum value has taken the place of current position 
                            {
                                current_List_Position_Not_Reached = false; //we do not want to be here anymore 
                                phase_Not_Complete = true; //reset this for the next cycle
                                minIndexToSave = minIndex; //we want to save the minimum index for the next part 
                            }
                        }
                        else 
                        { /* CURRENT POSITION HEADS TOWARD MINIMUM INDEX POSITION (BEFORE ITS SWAPPED LOCATION) */
                        
                            int minPosX = 0;
                            int minPosY = 0;
                            int minPosZ = 0; 

                            /* the point of this is too find the position of where the minimum value was at */
                            for (int i = 0; i < rect_Num_In_List.Count; i++)
                            {
                                if (i == minIndexToSave) //once we find current position we can get out
                                {
                                    i = rect_Num_In_List.Count;
                                }
                                else
                                {
                                    minPosX += 5; 
                                }
                            }

                            Vector3 minIndexPreviousPosition = new Vector3(minPosX, minPosY, minPosZ); //position of minimum index 

                            if (Mathf.Abs(rect.transform.position.x - minIndexPreviousPosition.x) > THRESHOLD) //move curr position in temp to where minimum value was previously at
                            {
                                if (minIndexPreviousPosition.x > rect.transform.position.x) //we move right if previous minimum location was to the right of curr pos in temp
                                {
                                    // DEBUG INFO --------------------------------------------------------------------
                                    Debug.Log("Cur going to min pos x (moving right): " + rect.transform.position.x);
                                    Debug.Log("min previous position x: " + minIndexPreviousPosition.x);
                                    Debug.Log("DIFFERENCE: " + Mathf.Abs(rect.transform.position.x - minIndexPreviousPosition.x));
                                    Debug.Log("IN TEMP GOING TO MIN POS RIGHT");
                                    // DEBUG INFO --------------------------------------------------------------------

                                    rect.transform.Translate(Vector3.right  * speed * Time.deltaTime); //move right (+ x-axis)
                                }
                                else //we move left if previous minimum location was to the left of curr pos in temp
                                {
                                    // DEBUG INFO --------------------------------------------------------------------
                                    Debug.Log("Cur going to min pos x (moving left): " + rect.transform.position.x);
                                    Debug.Log("min previous position x: " + minIndexPreviousPosition.x);
                                    Debug.Log("DIFFERENCE: " + Mathf.Abs(rect.transform.position.x - minIndexPreviousPosition.x));
                                    Debug.Log("IN TEMP GOING TO MIN POS LEFT");
                                    // DEBUG INFO --------------------------------------------------------------------

                                    rect.transform.Translate(Vector3.left  * speed *  Time.deltaTime); //move left (- x-axis)
                                }
                            }
                            else if (Mathf.Abs(rect.transform.position.z - minIndexPreviousPosition.z) > THRESHOLD) //move back up 
                            {
                                // DEBUG INFO --------------------------------------------------------------------
                                Debug.Log("Cur going to min pos z (moving up): " + rect.transform.position.z);
                                Debug.Log("min previous position z: " + minIndexPreviousPosition.z);
                                Debug.Log("DIFFERENCE: " + Mathf.Abs(rect.transform.position.z - minIndexPreviousPosition.z));
                                Debug.Log("IN TEMP GOING TO MIN POS DOWN");
                                // DEBUG INFO --------------------------------------------------------------------

                                rect.transform.Translate(Vector3.back * speed * Time.deltaTime); //move down (- z-axis)
                            }

                            //check if the cycle has been complete 
                            if (Mathf.Abs(rect.transform.position.x - minIndexPreviousPosition.x) <= THRESHOLD && Mathf.Abs(rect.transform.position.z - minIndexPreviousPosition.z) <= THRESHOLD)
                            {
                                temp_position_Not_Reached = true;
                                current_List_Position_Not_Reached = true;

                                //need to update list with numbers that have been swaped 
                                int temp = rect_Num_In_List[currentListPosition];
                                rect_Num_In_List[currentListPosition] = rect_Num_In_List[minIndexToSave]; 
                                rect_Num_In_List[minIndexToSave] = temp;

                                //need to swap tempRect objects 
                                GameObject tempRect = cubesList[currentListPosition];
                                cubesList[currentListPosition] = cubesList[minIndexToSave];
                                cubesList[minIndexToSave] = tempRect;



                                currentListPosition++;

                                //update the arrow position 
                                UpdateArrowPosition(getCurrPosition(rect_Num_In_List));

                                //I want the arrow above the rectangle  
                                Vector3 currentArrowPos = runtime_currentPositionArrow.transform.position;
                                currentArrowPos.y += 5;
                                runtime_currentPositionArrow.transform.position = currentArrowPos; 
                                

                            }


                        }
                    }
                

                
                }
            }
        }
    }



    /* this helper method gets current position */
    public Vector3 getCurrPosition(List<int> rect_Num_In_List)
    {
        int currPosX = 0;
        int currPosY = 0;
        int currPosZ = 0; 

        for (int i = 0; i < rect_Num_In_List.Count; i++)
            {
                if (i == currentListPosition) //once we find current position we can get out
                {
                    i = rect_Num_In_List.Count;
                }
                else
                {
                currPosX += 5; 
                }
            }
            return new Vector3(currPosX, currPosY, currPosZ);
    }

    //when we click the button we start sorting the list 
    public void StartAutoSortingLeastGreatest()
    {
        if (validList) //list has to be entered first and valid 
        {
            startSortingLeastGreatest = true;
            moveButtonsUp(); //move the buttons up 
        }

        

    }

    //when we click the button we start sorting the list 
    public void StartAutoSortingGreatestLeast()
    {
        if (validList) //list has to be entered first and valid 
        {
            startSortingGreatestLeast = true;
            moveButtonsUp(); //move the buttons up 
        }

        


    }

    //when we click the button we start sorting the list 
    public void StartAutoSortingAlphabetically()
    {
        if (validList) //list has to be entered first and valid 
        {
            startSortingAlphabetically = true;
            moveButtonsUp(); //move the buttons up 
        }

    }

    //helper method to move buttons up
    public void moveButtonsUp()
    {
        //move the button up (to make it "disapear) so the user cannot click on it 
        Vector3 currentPosition = SortLeastGreatest.transform.position;
        currentPosition.y += 600; // Increase the y position by 300
        SortLeastGreatest.transform.position = currentPosition; // Assign the new position

        //move the button up (to make it "disapear) so the user cannot click on it 
        currentPosition = SortGreatestLeast.transform.position;
        currentPosition.y += 600; // Increase the y position by 300
        SortGreatestLeast.transform.position = currentPosition; // Assign the new position

        //move the button up (to make it "disapear) so the user cannot click on it 
        currentPosition = SortAlphabetically.transform.position;
        currentPosition.y += 600; // Increase the y position by 300
        SortAlphabetically.transform.position = currentPosition; // Assign the new position

    }

    //helper method to move buttons down 
    public void moveButtonsDown()
    {
        //move the button up (to make it "disapear) so the user cannot click on it 
        Vector3 currentPosition = SortLeastGreatest.transform.position;
        currentPosition.y -= 600; // Increase the y position by 300
        SortLeastGreatest.transform.position = currentPosition; // Assign the new position

        //move the button up (to make it "disapear) so the user cannot click on it 
        currentPosition = SortGreatestLeast.transform.position;
        currentPosition.y -= 600; // Increase the y position by 300
        SortGreatestLeast.transform.position = currentPosition; // Assign the new position

        //move the button up (to make it "disapear) so the user cannot click on it 
        currentPosition = SortAlphabetically.transform.position;
        currentPosition.y -= 600; // Increase the y position by 300
        SortAlphabetically.transform.position = currentPosition; // Assign the new position

    }

    //when we enter the list we show the start button
    public void OnTextSubmitted(string userList)
    {
        userListEntered = true;
    }

    //this method processes the list the user enters if it is valid 
    public bool ProcessInput()
    {
        // Get the input text from the input field
        string inputText = InputUserList.text;
        bool SortAlpha = false;
        

        // Split the text into parts using ',' or space as a delimiter
        string[] numberStrings = inputText.Split(new char[] { ',', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

        if (numberStrings.Length <= 1) //we must have more than 1 element to sort in the list or something is wrong
        {
            Debug.Log("Please enter a valid list ");
            return false; //something went wrong we must leave
        }

        if (checkListIsNumberList(numberStrings)) //check if the list is made of only numbers 
        {
            // Convert each part to an integer
            for (int i = 0; i < numberStrings.Length; i++)
            {
                if (int.TryParse(numberStrings[i], out int parsedNumber))
                {
                    rect_Num_In_List.Add(parsedNumber);
                }
                else //we have a word to sort 
                {
                    Debug.Log("Oh no something went really wrong");
                }
            }
        }
        else if (checkListIsLettersOnly(numberStrings)) //otherwise check if the list is made of letters only
        {
            SortAlpha = true;

            for (int i = 0; i < numberStrings.Length; i++) //convert each letter into a number so we can still sort the list in terms of number value 
            {
                rect_Num_In_List.Add(CalculateAsciiValueSum(numberStrings[i]));
            }

        }
        else //if we are in here the user entered an incorrect list format 
        {
            Debug.Log("Please enter a valid list ");
            return false; //something went wrong we must leave

        }

        // Print the numbers to check
        foreach (int number in rect_Num_In_List)
        {
            Debug.Log("Number: " + number);
        }

        
        if (Camera_SA == null)
        {
            // If the cameraTransform is not set, find the child camera
            Camera_SA = GetComponentInChildren<Camera>().transform;
        }

        
        for (int i = 0; i < numberStrings.Length; i++) //we are initializing rectangle list
        {
            Vector3 rectanglePosition = new Vector3(distanceBetweenRectangles, 0, 0); //get a position for a rectangle 
            GameObject newRect = Instantiate(Number_Rect, rectanglePosition, Quaternion.identity); //create the rectangle

            
            TextMeshPro tmp = newRect.GetComponentInChildren<TextMeshPro>(); //get the rectangle's text box object 

            if (tmp != null)
            {
                if (SortAlpha) //if we are going to sort alphabetically 
                {
                    tmp.text = numberStrings[i]; //give it the word
                }
                else 
                {
                    tmp.text = rect_Num_In_List[i].ToString(); //give it this number
                }
            }
            

            cubesList.Add(newRect); //add rectangle to the list 

            distanceBetweenRectangles += 5; //increment the space 

        }

        middleX = distanceBetweenRectangles / 2; //calculate the middle of the soring alogirithm 

        //calculate camera postion based on total list objects 
        float cameraHeight = (float) cubesList.Count;
        float cameraDistanceFromList = (float) cubesList.Count * -3f;

        
        Vector3 cameraPosition = new Vector3(middleX, cameraHeight, cameraDistanceFromList); //create location for the camera 
        Camera_SA.position = cameraPosition; //set the camera to this postion

        //set the arrows position
     //   Vector3 arrowPosition = new Vector3(0, 5, 0);
      //  currentPositionArrow.transform.position = arrowPosition;


        //put the temp in its fixed position 
        if (temp != null)
        {
            temp.transform.position = new Vector3(middleX, TempHeight, TempDistance); //it should be in the middle of the list
        }

        return true; //if we made it here to the end there was a successful process of the list 

    }

    //a helper method to check if entire list is made of numbers from alphabet 
    public bool checkListIsNumberList(string [] list)
    {

        foreach (string str in list)
        {
            if (int.TryParse(str, out int result))
            {
                Debug.Log($"'{str}' is a valid integer: {result}");
            }
            else
            {
                Debug.Log($"'{str}' is not a valid integer");
                return false;
            }
        }

        return true;

    }

    //a helper method to check if entire list is made of letters from alphabet 
    public bool checkListIsLettersOnly(string [] list)
    {
        foreach (string str in list)
        {

            foreach (char c in str) //loop through the string
            {
                if (!char.IsLetter(c))
                {
                    return false;
                }
            }
        }

        return true; //the entire list is made of alphabet letters
    }

    public int CalculateAsciiValueSum(string input)
    {
        int sum = 0;

        foreach (char c in input)
        {
            sum += (int)c; // Casting the character to int gives the ASCII value
        }

        return sum;
    }

    //create the current position arrow
    void CreateArrowAtPosition(Vector3 position)
    {
        runtime_currentPositionArrow = Instantiate(currentPositionArrow, position, currentPositionArrow.transform.rotation);
        // You can also store a reference to the new arrow if needed
    }

    //update the arrow's position 
    void UpdateArrowPosition(Vector3 newPosition)
    {
        runtime_currentPositionArrow.transform.position = newPosition;
    }
    
    
}
