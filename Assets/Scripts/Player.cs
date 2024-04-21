using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [System.Serializable]
    public class Stats
    {
        public float moveSpeed = 5f;

        public float dashSpeed = 10f; // The speed of the dash
        public float dashDistance = 5f;// The length of the dash
        public float dashCooldown = 3f;

        public float AttackStun = 0.3f;
    }

    public Stats stats = new Stats();

    private Vector2 movement;

    public bool inCooldown = false;
    public bool inAttack = false;

    public bool inDefence = false;
    public float endDefence = 0f;

    private float dashTimeTaken = 0f;
    private float endStun = 0f;
    public float cooldownTimer = 0f;
    private float endAttack = 0f;

    private Rigidbody2D rb;

    private CircleCollider2D parryCollider;

    private Vector3 targetPosition;

    private GameMaster gm;



    private void Start()
    {
        gm = GameMaster.Instance;
        rb = GetComponent<Rigidbody2D>();
        parryCollider = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (inAttack && Time.time >= endStun)
        {
            inAttack = false;
        }

        if (inDefence && Time.time >= endDefence)
        {
            inDefence = false;
            //Debug.Log("cooldown = " + cooldownTimer);
        }

        // Check for left mouse button click and enter attack mode
        if (Input.GetMouseButtonDown(0) && !inCooldown && Time.time >= endStun)
        {
            inAttack = true;
            inCooldown = true;
            cooldownTimer = 0;

            float estimatedAttackDuration = stats.dashDistance / stats.dashSpeed;

            endAttack = Time.time + estimatedAttackDuration;
            endStun = endAttack + stats.AttackStun;

            //Debug.Log("estimated attack duration = " + estimatedAttackDuration);

            // Get the mouse position in world coordinates
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = transform.position.z; // Ensure the same z-coordinate as the object

            // Calculate direction towards the mouse
            Vector3 direction = (targetPosition - transform.position).normalized;

            // Move the object by dashDistance in that direction
            Vector3 dashDestination = transform.position + direction * stats.dashDistance;

            // Start dashing towards the calculated destination
            StartCoroutine(DashTowards(dashDestination));
        }

        //check for right click and enter parry mode
        if (Input.GetMouseButtonDown(1) && Time.time >= endStun && !inCooldown)
        {

            inDefence = true;
            endDefence = Time.time + stats.dashCooldown;
            //endStun = endDefence + stats.AttackStun;

            inCooldown = true;
            cooldownTimer = 0;

            StartCoroutine(Parry());

        }

        if (inCooldown)
        {
            cooldownTimer += Time.deltaTime;

            //Debug.Log("cooldown = " + cooldownTimer);

            if (cooldownTimer >= stats.dashCooldown)
            {
                inCooldown = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!inAttack)
        {
            rb.MovePosition(rb.position + movement.normalized * stats.moveSpeed * Time.fixedDeltaTime);
        }
    }

    IEnumerator DashTowards(Vector3 destination)
    {
        Vector3 initialPosition = transform.position;
        float startTime = Time.time;

        while (Vector3.Distance(transform.position, destination) > 0.1f)
        {
            // Move towards the destination at dashSpeed
            transform.position = Vector3.MoveTowards(transform.position, destination, stats.dashSpeed * Time.deltaTime);
            yield return null;
        }

        float dashDistanceTraveled = Vector3.Distance(initialPosition, transform.position);
        float dashTimeTaken = Time.time - startTime;

        Debug.Log("Dash Distance Traveled: " + dashDistanceTraveled);
        Debug.Log("Dash Time Taken: " + dashTimeTaken);
    }

    IEnumerator Parry()
    {
        Debug.Log("parry");
        parryCollider.enabled = true;


        yield return new WaitForSeconds(1);

        parryCollider.enabled = false;
        inDefence = false;

        Debug.Log("stop parry");
    }

}


