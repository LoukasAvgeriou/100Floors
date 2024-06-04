using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode()]
public class FirstBossHelthBar : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("GameObject/UI/BossHealthBar")]
    public static void AddLinearProgressBar()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("UI/BossHealthBar"));
        obj.transform.SetParent(Selection.activeGameObject.transform, false);
    }
#endif

    public float maximum;

    public Image mask;
    public Image fill;
    //public Color color;

    //public GameObject player;
    //public Player playerScript;
    
    public GameObject firstBoss;
    public FirstBoss fBScript;

    private void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player");
        firstBoss = GameObject.FindGameObjectWithTag("FirstBoss");



        fBScript = firstBoss.GetComponent<FirstBoss>();

        maximum = fBScript.health;
    }

    private void Update()
    {
        FillTheBar();
    }

    public void FillTheBar()
    {
        mask.fillAmount = fBScript.health / maximum;
        Debug.Log(fBScript.health);
        
    }
}
