using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeController : MonoBehaviour
{
    public Player player;
    public UpgradesControllerSO upgradeControllerSO;

    public void CooldownReductionUpgrade()
    {
        player.stats.dashCooldown = upgradeControllerSO.newCooldown;
    }
}
