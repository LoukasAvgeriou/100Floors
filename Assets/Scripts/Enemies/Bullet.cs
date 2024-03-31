using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject player;
    public Rigidbody2D rb;

    public float projectileVelocity = 5f;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        Vector3 difference = transform.position - player.transform.position;
        difference.Normalize();
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        //GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0f, 0f, rotZ + 90)) as GameObject;

        gameObject.transform.rotation = Quaternion.Euler(0f, 0f, rotZ + 90);

        Rigidbody2D projectileRB = gameObject.GetComponent<Rigidbody2D>();
        projectileRB.velocity = gameObject.transform.up * projectileVelocity;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            GameMaster.Instance.KillPlayer(col.gameObject);
        }
    }
}