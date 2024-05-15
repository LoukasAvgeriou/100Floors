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
        enemy.spriteRenderer.color = Color.white;
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
        enemy.spriteRenderer.color = Color.red;
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
        enemy.spriteRenderer.color = Color.white;

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

    public GameObject target;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;

    public void Awake()
    {
        target = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        fsm = new FSM(new BasicEnemyFollowPlayer(this));
    }

    private void Update()
    {
        
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
                else
                {
                    GameMaster.Instance.KillPlayer(col.gameObject);
                }
            }
        }
    }
}
