 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IHitable
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

        gameObject.transform.rotation = Quaternion.Euler(0f, 0f, rotZ + 90);

        rb.velocity = gameObject.transform.up * projectileVelocity;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            Player playerScript = player.GetComponent<Player>();

            if (playerScript.inDefence && upgradeControllerSO.returnBullets)
            {
                playerScript.inCooldown = false;
                GoBack();
            }
            else if(playerScript.inDefence)
            {
                gameObject.SetActive(false);
                playerScript.inCooldown = false;
            }
            else if (playerScript.inAttack && upgradeControllerSO.breakableBullets)
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
                friendlyFire = false;
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

    public void Hit()
    {
        Player playerScript = player.GetComponent<Player>();

        if (playerScript.inAttack && upgradeControllerSO.breakableBullets)
        {   // We are currently dashing and we have the kill bulets upgrade, so destroy the bullet
            gm.KillEnemy(gameObject);
        }
    }
}