using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListOfUpgradeFunctions : MonoBehaviour
{
    public Player player;
    public UpgradesControllerSO upgradeControllerSO;

    public void CooldownReductionUpgrade()
    {
        player.stats.dashCooldown = upgradeControllerSO.newCooldown;
    }

    public void DashBreaksBulletsUpgrade()
    {
        upgradeControllerSO.breakableBullets = true;
    }

    public void ReturnBullets()
    {
        upgradeControllerSO.returnBullets = true;
    }

    public void SecondLife()
    {
        upgradeControllerSO.secondLife = true;
    }
}
