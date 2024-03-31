using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode()]
public class ProgressBar : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("GameObject/UI/Linear Progress Bar")]
    public static void AddLinearProgressBar()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("UI/Linear Progress Bar"));
        obj.transform.SetParent(Selection.activeGameObject.transform, false);
    }
#endif

    public float maximum;

    public Image mask;
    public Image fill;
    public Color color;

    public GameObject player;
    public Player playerScript;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        playerScript = player.GetComponent<Player>();

        maximum = playerScript.stats.dashCooldown;
    }

    private void Update()
    {
        FillTheBar();
    }

    public void FillTheBar()
    {
        if (!playerScript.inCooldown)
        {
            mask.fillAmount = maximum;
        }
        else
        {
            mask.fillAmount = (float)playerScript.cooldownTimer / (float)maximum;
        }
    }
}
