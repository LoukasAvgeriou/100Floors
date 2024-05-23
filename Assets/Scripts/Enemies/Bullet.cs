using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject player;
    public Rigidbody2D rb;

    public float projectileVelocity = 5f;

    public UpgradesControllerSO upgradeControllerSO;
    private GameMaster gm;

    //if friendly fire is true, then the bullet will harm enemies
    public bool friendlyFire = false;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        gm = GameMaster.Instance;
    }

    private void OnEnable()
    {
        Vector3 difference = transform.position - player.transform.position;
        difference.Normalize();
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        //GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0f, 0f, rotZ + 90)) as GameObject;

        gameObject.transform.rotation = Quaternion.Euler(0f, 0f, rotZ + 90);

        //Rigidbody2D projectileRB = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = gameObject.transform.up * projectileVelocity;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            Player playerScript = player.GetComponent<Player>();

            if (playerScript.inDefence && upgradeControllerSO.returnBullets)
            {
                GoBack();
            }
            else if(playerScript.inDefence)
            {
                gameObject.SetActive(false);
                playerScript.inCooldown = false;
            }
            else
            {
                GameMaster.Instance.KillPlayer(col.gameObject);
            }
        }
        else
        {
            if (friendlyFire)
            {
                gm.KillEnemy(col.gameObject);
                gameObject.SetActive(false);
            }
        }
    }

    //we parry the bullet and send it back
    private void GoBack()
    {
        rb.velocity = -rb.velocity;
        friendlyFire = true;
    }
}