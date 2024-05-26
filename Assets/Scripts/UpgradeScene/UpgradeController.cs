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
    public  List<Button> allUpgradesList = new List<Button>();
    
    // A working list that will be modified during runtime
    //public List<Button> workingList = new List<Button>();

    //the list of the transforms that we will place the upgrade buttons
    public List<Transform> buttonsTransforms = new List<Transform>();
    
    private void Start()
    {
        //workingList = new List<Button>(upgradeControllerSO.availableUpgradeButtons);
        //upgradeControllerSO.availableUpgradeButtons = new List<Button>(workingList);

        // Ensure there are enough objects to pick
        if (upgradeControllerSO.availableUpgradeButtons.Count >= numberOfUpgradesToPick)
        {
            PickRandomUpgrades();
        }
        else
        {
            Debug.LogError("Not enough game objects in the list to pick from!");
        }

       /* foreach (Button button in workingList)
        {
            button.onClick.AddListener(() => OnButtonClick(button));
        } */ 
    }

    public void PickRandomUpgrades()
    {
        // Create a temporary list to avoid modifying the original list while iterating
        List<Button> tempList = new List<Button>(upgradeControllerSO.availableUpgradeButtons);

        for (int i = 0; i < numberOfUpgradesToPick; i++)
        {
            // Pick a random index
            int randomIndex = Random.Range(0, tempList.Count);

            // Get the game object at the random index
            Button selectedButton = tempList[randomIndex];

            tempList.Remove(selectedButton);

            selectedButton = Instantiate(selectedButton, buttonsTransforms[i].transform.position, transform.rotation);

            //we need to have the button be child of the canvas so it will be visible
            selectedButton.transform.parent = canvas.transform;

            selectedButton.onClick.AddListener(() => OnButtonClick(selectedButton, randomIndex));
        }
    }

    void OnButtonClick(Button clickedButton, int prefabIndex)
    {
        

        upgradeControllerSO.availableUpgradeButtons.RemoveAt(prefabIndex);

        

        //we increase the level we are on
        levelControllerSO.currentLevel += 1;
       
       
        
        //we load the next level
        SceneManager.LoadScene(levelControllerSO.levels[levelControllerSO.currentLevel - 1].ToString());  
    } 
}
