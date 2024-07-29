using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private void OnEnable()
    {
        transform.parent = null;  
        StartCoroutine(ActivateCoroutine());
    }

    private IEnumerator ActivateCoroutine()
    {
        yield return new WaitForSeconds(1);  // Wait for 1 second
        gameObject.SetActive(false);  // Deactivate the game object      
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameMaster.Instance.KillPlayer(collision.gameObject);
        }
    }
}
