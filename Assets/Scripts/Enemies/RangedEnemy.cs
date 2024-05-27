using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//first state, we are following the player if we are far from him
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
        enemy.animationStateIsLocked = false;
        enemy.ChangeAnimationState(enemy.RANGEDENEMY_SIDERUN);
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

//second state, we charge the shot
public class RangedEnemyChargeBeforeShoot : State
{
    public RangedEnemy enemy;
    private float currentTime = 0f;

    public RangedEnemyChargeBeforeShoot(RangedEnemy myEnemy)
    {
        enemy = myEnemy;
    }

    public override void Enter()
    {
        //enemy.spriteRenderer.color = Color.red;
        
        enemy.ChangeAnimationState(enemy.RANGEDENEMY_SIDECHARGE);
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

//third state, we shoot
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
        enemy.ChangeAnimationState(enemy.RANGEDENEMY_IDLE);

        //shoot
        GameObject bullet = ObjectPooler.SharedInstance.GetPooledObject("bullet");
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

//fourth state, enemy is in cooldown, not moving
public class RangedEnemyCooldown : State
{
    public RangedEnemy enemy;
    private float currentTime = 0f;
    public RangedEnemyCooldown(RangedEnemy myEnemy)
    {
        enemy = myEnemy;
    }

    public override void Enter()
    {
        //Debug.Log("state 4");
        //enemy.spriteRenderer.sprite = enemy.number4;
        enemy.animationStateIsLocked = true;
    }

    public override void CalledInFixedUpdate()
    {
        currentTime += Time.fixedDeltaTime;
        if (currentTime > enemy.cooldownDuration)
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
    public float cooldownDuration = 2f;

    public GameObject target;
    public Rigidbody2D rb;
    private Animator anim;

    //if the animation is locked then we will not change animation, it's used for when the enemy will be standing still and we dont want to change the side he looks
    public bool animationStateIsLocked = false;
    public Vector2 enemyDirection;


    //animation state
    private string currentState;

    //Animation states
    public string RANGEDENEMY_IDLE = "rangedEnemy_idle";
    public string RANGEDENEMY_SIDERUN = "rangedEnemy_sideRun";
    public string RANGEDENEMY_SIDECHARGE = "rangedEnemy_sideCharge";

    //how close the enemy will go to the player
    public float followDistance = 3f;

    private void OnEnable()
    {
        fsm = new FSM(new RangedEnemyFollowPlayer(this));
    }

    public void Awake()
    {
        target = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
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

