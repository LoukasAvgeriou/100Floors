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
        //Debug.Log("state 1");
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

    private bool setAttackPos = false;
    private Vector2 dashDirection;

    public BasicEnemyDashToPlayer(BasicEnemy myEnemy)
    {
        enemy = myEnemy;
    }

    public override void Enter()
    {
        enemy.dashTimeLeft = enemy.dashDuration;
        enemy.isDashing = true;
    }


    public override void CalledInFixedUpdate()
    {

        if (!setAttackPos)
        {
            setAttackPos = true;
            dashDirection = enemy.target.transform.position - enemy.transform.position;
            dashDirection = dashDirection.normalized;
        }
        if (enemy.isDashing)
        {
            // Move the player object in the dash direction at the dash speed
            enemy.rb.MovePosition(enemy.rb.position + dashDirection.normalized * enemy.dashSpeed * Time.fixedDeltaTime);
            enemy.dashTimeLeft -= Time.fixedDeltaTime;

            // Stop dashing after the dash duration has elapsed
            if (enemy.dashTimeLeft <= 0)
            {
                enemy.isDashing = false;
                parentFSM.SetCurrentState(new BasicEnemyFollowPlayer(enemy));
            }
        }
    }



}



public class BasicEnemy : Enemy
{
    private FSM fsm;

    public float speed = 10f;
    public float attackSpeedModifier = 3f;
    public float chargeDuration = 2f;

    public GameObject target;
    public Rigidbody2D rb;

    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    public float dashTimeLeft;
    public bool isDashing = false;

    //how close the enemy will go to the player
    public float followDistance = 3f;

    public void Awake()
    {

        target = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        fsm = new FSM(new BasicEnemyFollowPlayer(this));
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
                GameMaster.Instance.KillPlayer(col.gameObject);
            }
        }
    }
}
