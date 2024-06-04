using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStuff : MonoBehaviour
{
    public GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log("test script active");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("we are inside the onTriggerEnter2D");

        if (collision.tag == "Player")
        {
            Debug.Log("we have contact");
            Player playerScript = player.GetComponent<Player>();

            if (playerScript.inAttack)
            {
                playerScript.inCooldown = false;
                
            }

            
        }
    }


}
