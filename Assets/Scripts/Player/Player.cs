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

    //this is the wall layer, we use it to detect the walls
    public LayerMask obstacleLayer;
    
    //we will use this to calculate the correct animation to use
    private Vector2 previousPosition;

    public bool inCooldown = false;
    public bool inAttack = false;

    public bool inDefence = false;
    public float endDefence = 0f;

    private float dashTimeTaken = 0f;
    private float endStun = 0f;
    public float cooldownTimer = 0f;
    private float endAttack = 0f;

    //animation state
    private string currentState;

    //Animation states
    const string PLAYER_IDLE = "player_idle";
    const string PLAYER_RUN = "player_run";
    const string PLAYER_SIDEDASH = "player_sideDash";
    const string PLAYER_PARRY = "player_parry";

    public UpgradesControllerSO upgradesControllerSO;

    private Rigidbody2D rb;
    private Animator anim;
    private CircleCollider2D parryCollider;

    private Vector3 targetPosition;

    private GameMaster gm;



    private void Start()
    {
        gm = GameMaster.Instance;
        rb = GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        parryCollider = GetComponent<CircleCollider2D>();

        previousPosition = transform.position;
        currentState = PLAYER_IDLE;
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

        //we count the cooldown 
        if (inCooldown)
        {
            cooldownTimer += Time.deltaTime;

            //Debug.Log("cooldown = " + cooldownTimer);

            if (cooldownTimer >= stats.dashCooldown)
            {
                inCooldown = false;
            }
        }


        //animation control

        // Update the current position
        Vector2 currentPosition = transform.position;

        // Calculate the direction of movement
        Vector2 playerDirection = currentPosition - previousPosition;

        // Check if the object is moving left or right
        if (playerDirection.x > 0)
        {
            
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (playerDirection.x < 0)
        {
            
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

        // Update the previous position for the next frame
        previousPosition = currentPosition;

        // Check if both inputs are zero
        if (movement.x == 0 && movement.y == 0 && !inAttack && !inDefence)
        {
            //we need the idle animation
            ChangeAnimationState(PLAYER_IDLE);
        } 
        else if (inAttack)
        {
            //we need the attack animation
            ChangeAnimationState(PLAYER_SIDEDASH);
        }
        else if (inDefence)
        {
            //we need the attack animation
            ChangeAnimationState(PLAYER_PARRY);
        }

        else
        {
            //we need the run animation
            ChangeAnimationState(PLAYER_RUN);
        }
    }

    private void FixedUpdate()
    {
        if (!inAttack && !inDefence)
        {
            rb.MovePosition(rb.position + movement.normalized * stats.moveSpeed * Time.fixedDeltaTime);   
        }
    }

    IEnumerator DashTowards(Vector3 destination)
    {
        Vector3 initialPosition = transform.position;
        float startTime = Time.time;

        // Length of the ray
        float rayLength = 100f;

        // Calculate direction towards the mouse
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Cast a ray that only detects walls
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, stats.dashDistance, obstacleLayer);

        Debug.Log("Dashing time");

        // If it hits something the wall
        if (hit.collider != null)
        {
            Debug.Log("hit something");
            
            //because we will hit a wall, we change our destination, to the wall, so we dont pass through the wall
            destination = hit.point;
        }

        while (Vector3.Distance(transform.position, destination) > 0.1f)
        {
            // Move towards the destination at dashSpeed
            transform.position = Vector3.MoveTowards(transform.position, destination, stats.dashSpeed * Time.deltaTime);
            yield return null;
        }

        float dashDistanceTraveled = Vector3.Distance(initialPosition, transform.position);
        float dashTimeTaken = Time.time - startTime;
    }

    IEnumerator Parry()
    {
        parryCollider.enabled = true;

        yield return new WaitForSeconds(0.5f);

        parryCollider.enabled = false;
        inDefence = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //we collided with a basic enemy
        if (col.tag == "basicEnemy")
        {
            if (inAttack)
            {   // We are currently dashing, kill enemy.
                gm.KillEnemy(col.gameObject);
                inCooldown = false;
            }
        }
        else if(col.tag == "rangedEnemy")
        {
            if (inAttack)
            {   // We are currently dashing, kill enemy.
                gm.KillEnemy(col.gameObject);
                inCooldown = false;
            }
        } 
        else if(col.tag == "bullet")
        {
            if (inAttack && upgradesControllerSO.breakableBullets)
            {   // We are currently dashing and we have the kill bulets upgrade, so destroy the bullet
                gm.KillEnemy(col.gameObject);
                inCooldown = false;
            }
        }

        
    }

    private void ChangeAnimationState(string newState)
    {
        //stop the same animation from interrupting itself
        if (currentState == newState) return;

        //play the animation
        anim.Play(newState);

        //reassign the current state
        currentState = newState;
    }
}


