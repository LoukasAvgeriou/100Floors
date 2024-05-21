using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LevelControllerSO")]
public class LevelControllerSO : ScriptableObject
{
    public int currentLevel = 1;

    public List<string> levels = new List<string>();
}
