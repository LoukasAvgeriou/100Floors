using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UpgradeControllerSO")]
public class UpgradesControllerSO : ScriptableObject
{
    //this is the upgrade for cooldown reduction, it reduces the cooldown of the dash
    public bool cooldownReduction = false;
    public float newCooldown = 2f;
}
