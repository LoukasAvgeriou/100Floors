using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class FirstBossGameMaster : MonoBehaviour
{
    #region Singleton

    public static FirstBossGameMaster Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion

   
}
