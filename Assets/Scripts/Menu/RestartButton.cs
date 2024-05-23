using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    public UpgradesControllerSO upgradeControllerSO;
    public LevelControllerSO levelControllerSO;

    public Player player;

    public void RestartGame()
    {

        levelControllerSO.currentLevel = 1;

        upgradeControllerSO.availableUpgradeButtons.Clear();

        upgradeControllerSO.availableUpgradeButtons = new List<UnityEngine.UI.Button>(upgradeControllerSO.upgradeButtons);

        player.stats.dashCooldown = upgradeControllerSO.startingCooldown;
        upgradeControllerSO.breakableBullets = false;
        upgradeControllerSO.returnBullets = false;
        upgradeControllerSO.secondLife = false;

        SceneManager.LoadScene("SampleScene");
    }
}
