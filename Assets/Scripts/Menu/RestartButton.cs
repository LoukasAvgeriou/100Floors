using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    public UpgradesControllerSO upgradeControllerSO;

    public void RestartGame()
    {
        upgradeControllerSO.availableUpgradeButtons.Clear();

        upgradeControllerSO.availableUpgradeButtons = new List<UnityEngine.UI.Button>(upgradeControllerSO.upgradeButtons);

        SceneManager.LoadScene("SampleScene");
    }
}
