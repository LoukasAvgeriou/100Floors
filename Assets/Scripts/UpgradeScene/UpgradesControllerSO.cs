using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UpgradeControllerSO")]
public class UpgradesControllerSO : ScriptableObject
{
    //list of all updates
    public List<Button> upgradeButtons = new List<Button>();

    //list of the available updates
    public List<Button> availableUpgradeButtons = new List<Button>();

    //this is the upgrade for cooldown reduction, it reduces the cooldown of the dash
    public bool cooldownReduction;
    public float newCooldown = 2f;
    public float startingCooldown = 3f;

    //this is the upgrade for breakable bullets
    public bool breakableBullets;

    //this the upgrade for returning bullets by parrying them
    public bool returnBullets;

    //this is the upgrade for second life
    public bool secondLife;
}
