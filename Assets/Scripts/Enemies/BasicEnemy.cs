using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//first state, enemy follows the player
public class BasicEnemyFollowPlayer : State
{
    public BasicEnemy enemy;

    public BasicEnemyFollowPlayer(BasicEnemy myEnemy)
    {
        enemy = myEnemy;
    }

    public override void Enter()
    {
        //enemy.spriteRenderer.color = Color.white;
        enemy.animationStateIsLocked = false;
        enemy.ChangeAnimationState(enemy.BASICENEMY_SIDERUN);
    }

    public override void CalledInFixedUpdate()
    {
        // Calculate the direction and distance to the target
        Vector2 direction = enemy.target.transform.position - enemy.transform.position;
        float distance = direction.magnitude;

        //if we are far away from target, we go to target, if we reach target we change state
        if (distance > enemy.followDistance)
        {
            enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, enemy.target.transform.position, enemy.speed * Time.deltaTime);
        }
        else
        {
            parentFSM.SetCurrentState(new BasicEnemyChargeBeforeDash(enemy));
        }
    }
}

//second state, enemy is charging before the dash
public class BasicEnemyChargeBeforeDash : State
{
    public BasicEnemy enemy;
    private float currentTime = 0f;

    public BasicEnemyChargeBeforeDash(BasicEnemy myEnemy)
    {
        enemy = myEnemy;
    }

    public override void Enter()
    {
        
        //enemy.spriteRenderer.color = Color.red;
        enemy.animationStateIsLocked = true;
        enemy.ChangeAnimationState(enemy.BASICENEMY_CHARGE);
    }

    public override void CalledInFixedUpdate()
    {
        currentTime += Time.fixedDeltaTime;
        if (currentTime > enemy.chargeDuration)
        {
            parentFSM.SetCurrentState(new BasicEnemyDashToPlayer(enemy));
        }
    }
}

//3rd state enemy is dashing to player
public class BasicEnemyDashToPlayer : State
{
    public BasicEnemy enemy;
    private Vector3 dashDestination;

    public BasicEnemyDashToPlayer(BasicEnemy myEnemy)
    {
        enemy = myEnemy;
    }

    public override void Enter()
    {
        //enemy.spriteRenderer.color = Color.white;

        // Check if the object is moving left or right
        if (enemy.enemyDirection.x > 0)
        {
            
            
                //Debug.Log("we should turn");
                enemy.transform.eulerAngles = new Vector3(0, 180, 0);
            

        }
        else if (enemy.enemyDirection.x < 0)
        {
            
                Debug.Log("we are ok");
                enemy.transform.eulerAngles = new Vector3(0, 0, 0);
            
        }

        //enemy.animationStateIsLocked = true;
        enemy.ChangeAnimationState(enemy.BASICENEMY_SIDEDASH);

        enemy.isDashing = true;

        // Calculate direction towards the player
        Vector3 direction = (enemy.target.transform.position - enemy.transform.position).normalized;

        // find the dash destination
        dashDestination = enemy.transform.position + direction * enemy.dashDistance;
    }

    public override void CalledInFixedUpdate()
    {
        if (Vector3.Distance(enemy.transform.position, dashDestination) > 0.1f)
        {
            // Move towards the destination at dashSpeed
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, dashDestination, enemy.dashSpeed * Time.fixedDeltaTime);
        }
        else
        {
            parentFSM.SetCurrentState(new BasicEnemyCooldown(enemy));
        }
    }
}

//fourth state, enemy is in cooldown, not moving
public class BasicEnemyCooldown : State
{
    public BasicEnemy enemy;
    private float currentTime = 0f;
    public BasicEnemyCooldown(BasicEnemy myEnemy)
    {
        enemy = myEnemy;
    }

    public override void Enter()
    {
        enemy.isDashing = false;
    }

    public override void CalledInFixedUpdate()
    {
        currentTime += Time.fixedDeltaTime;
        if (currentTime > enemy.cooldownDuration)
        {
            parentFSM.SetCurrentState(new BasicEnemyFollowPlayer(enemy));
        }
    }
}

//fifth state, enemy is bouncing away of the player after a successful parry
public class BasicEnemyParryBounce : State
{
    public BasicEnemy enemy;
    private Vector3 bounceDestination;

    public BasicEnemyParryBounce(BasicEnemy myEnemy)
    {
        enemy = myEnemy;
    }

    public override void Enter()
    {
        
        
        enemy.ChangeAnimationState(enemy.BASICENEMY_CHARGE);

        // Calculate direction towards the player
        Vector3 direction = (enemy.target.transform.position - enemy.transform.position).normalized;

        // find the bounce destination
        bounceDestination = enemy.transform.position - direction * enemy.bounceDistance;
    }

    public override void CalledInFixedUpdate()
    {
        if (Vector3.Distance(enemy.transform.position, bounceDestination) > 0.1f)
        {
            // Move towards the destination at dashSpeed
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, bounceDestination, enemy.bounceSpeed * Time.fixedDeltaTime);
        }
        else
        {
            parentFSM.SetCurrentState(new BasicEnemyCooldown(enemy));
        }
    }
}


public class BasicEnemy : Enemy
{
    private FSM fsm;

    public float speed = 10f;
    //how close the enemy will go to the player
    public float followDistance = 3f;

    public float chargeDuration = 2f;
    
    public float dashSpeed = 10f;
    public float dashDistance = 5f;
   
    public float cooldownDuration = 2f;

    public float bounceDistance = 4f;
    public float bounceSpeed = 7f;

    public bool isDashing = false;
    //if the animation is locked then we will not change animation, it's used for when the enemy will be standing still and we dont want to change the side he looks
    public bool animationStateIsLocked = false;
    public Vector2 enemyDirection;


    //animation state
    private string currentState;

    //Animation states
    public string BASICENEMY_IDLE = "basicEnemy_idle";
    public string BASICENEMY_SIDERUN = "basicEnemy_sideRun";
    public string BASICENEMY_SIDEDASH = "basicEnemy_sideDash";
    public string BASICENEMY_CHARGE = "basicEnemy_charge";

    public GameObject target;

    public Rigidbody2D rb;
    private Animator anim;
    public SpriteRenderer spriteRenderer;

    public void Awake()
    {
        target = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = gameObject.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        fsm = new FSM(new BasicEnemyFollowPlayer(this));
        currentState = BASICENEMY_IDLE;
       
    }

    private void Update()
    {
       //where the sprite will look
        // Update the current position
        Vector2 currentPosition = transform.position;
        Vector2 playerPosition = target.transform.position;

        // Calculate the direction of movement
        enemyDirection = currentPosition - playerPosition;

        // Check if the object is moving left or right
        if (enemyDirection.x > 0)
        {
            if (!animationStateIsLocked)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            
        }
        else if (enemyDirection.x < 0)
        {
            if (!animationStateIsLocked)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
        } 
        
    }

    public void FixedUpdate()
    {
        fsm.CalledInFixedUpdate();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (isDashing)
        {
            if (col.gameObject.tag == "Player")
            {
                Player playerScript = target.GetComponent<Player>();

                if (playerScript.inDefence)
                {
                    playerScript.inCooldown = false;
                    fsm = new FSM(new BasicEnemyParryBounce(this));
                }
                else if (!playerScript.inAttack)
                {
                    GameMaster.Instance.KillPlayer(col.gameObject);
                }
            }
        }
    }

    public void ChangeAnimationState(string newState)
    {
        //stop the same animation from interrupting itself
        if (currentState == newState) return;

        //play the animation
        anim.Play(newState);

        //reassign the current state
        currentState = newState;
    }
}
