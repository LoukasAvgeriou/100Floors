using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyFollowPlayer : State
{
    public RangedEnemy enemy;

    public RangedEnemyFollowPlayer(RangedEnemy myEnemy)
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
            parentFSM.SetCurrentState(new RangedEnemyChargeBeforeShoot(enemy));
        }
    }
}

public class RangedEnemyChargeBeforeShoot : State
{
    public RangedEnemy enemy;
    private float currentTime = 0f;

    public RangedEnemyChargeBeforeShoot(RangedEnemy myEnemy)
    {
        enemy = myEnemy;
    }

    public override void CalledInFixedUpdate()
    {
        currentTime += Time.fixedDeltaTime;
        if (currentTime > enemy.chargeDuration)
        {

            parentFSM.SetCurrentState(new RangedEnemyShoot(enemy));
        }
    }
}

public class RangedEnemyShoot : State
{
    public RangedEnemy enemy;
    private float currentTime = 0f;

    public RangedEnemyShoot(RangedEnemy myEnemy)
    {
        enemy = myEnemy;
    }

    public override void Enter()
    {
        //shoot
        GameObject bullet = ObjectPooler.SharedInstance.GetPooledObject("Arrow");
        if (bullet != null)
        {
            bullet.transform.position = enemy.transform.position;
            bullet.transform.rotation = enemy.transform.rotation;
            bullet.SetActive(true);
        }
    }
    public override void CalledInFixedUpdate()
    {
        currentTime += Time.fixedDeltaTime;
        if (currentTime > enemy.chargeDuration)
        {

            parentFSM.SetCurrentState(new RangedEnemyFollowPlayer(enemy));
        }
    }
}

public class RangedEnemy : Enemy
{
    private FSM fsm; //finite state machine

    public float speed = 10f;
    public float chargeDuration = 2f;

    public GameObject target;
    public Rigidbody2D rb;

    //how close the enemy will go to the player
    public float followDistance = 3f;

    public void Awake()
    {
        fsm = new FSM(new RangedEnemyFollowPlayer(this));

        target = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
        fsm.CalledInFixedUpdate();
    }
}

