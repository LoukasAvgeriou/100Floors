using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UpgradeController : MonoBehaviour
{
    public LevelControllerSO levelControllerSO;
    public UpgradesControllerSO upgradeControllerSO;

    public GameObject canvas;

    // Number of upgrades to pick
    public int numberOfUpgradesToPick = 2;

    //the list of our upgrades
    public List<Button> allUpgradesList = new List<Button>();

    //we create a list to store the 2 random indexes
    public List<int> randomIndexes = new List<int>();

    //the list of the transforms that we will place the upgrade buttons
    public List<Transform> buttonsTransforms = new List<Transform>();
    
    private void Start()
    {
        // Ensure there are enough objects to pick
        if (upgradeControllerSO.availableUpgradeButtons.Count >= numberOfUpgradesToPick)
        {
            PickRandomUpgrades();
        }
        else
        {
            Debug.LogError("Not enough game objects in the list to pick from!");
        }
    }

    public void PickRandomUpgrades()
    {
        // Create a temporary list to avoid modifying the original list while iterating
        List<Button> tempList = new List<Button>(upgradeControllerSO.availableUpgradeButtons);
        
        Debug.Log("our list is ready! The length of the tempList = " + tempList.Count);

        PickTwoDifferentRandomNumbers();

        for (int i = 0; i < numberOfUpgradesToPick; i++)
        {

            var tempIDX = i;
            Debug.Log("try number:" + i.ToString() + "the random index we picked = " + randomIndexes[i].ToString());

            // Get the game object at the random index
            Button selectedButton = tempList[randomIndexes[i]];

            //tempList.Remove(selectedButton);

            selectedButton = Instantiate(selectedButton, buttonsTransforms[i].transform.position, transform.rotation);

            //we need to have the button be child of the canvas so it will be visible
            selectedButton.transform.parent = canvas.transform;

            selectedButton.onClick.AddListener(() => OnButtonClick(selectedButton, randomIndexes[tempIDX]));   
            
        }
    }

    void OnButtonClick(Button clickedButton, int prefabIndex)
    {
        Debug.Log("we are inside the button function");

        upgradeControllerSO.availableUpgradeButtons.RemoveAt(prefabIndex);

        //we increase the level we are on
        levelControllerSO.currentLevel += 1;
    
        //we load the next level
        SceneManager.LoadScene(levelControllerSO.levels[levelControllerSO.currentLevel - 1].ToString());  
    }

    private void PickTwoDifferentRandomNumbers()
    {
        Debug.Log("time to pick the numbers");

        // Create a list of possible numbers.
        List<int> numbers = new List<int>();
        for (int i = 0; i < upgradeControllerSO.availableUpgradeButtons.Count; i++)
        {
            numbers.Add(i);
        }

        Debug.Log("numbers count = " + numbers.Count.ToString());

        // Shuffle the list using Fisher-Yates algorithm.
        for (int i = numbers.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = numbers[i];
            numbers[i] = numbers[randomIndex];
            numbers[randomIndex] = temp;
        }

        for (int i = 0; i < upgradeControllerSO.availableUpgradeButtons.Count; i++)
        {
            randomIndexes.Add(numbers[i]);
        }

        Debug.Log("the two random numbers are " + randomIndexes[0].ToString() + " and " + randomIndexes[1].ToString());
    }
}
