using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstBossShootBigBullet : State
{
    public FirstBoss boss;
    private float currentTime = 0f;

    public FirstBossShootBigBullet(FirstBoss myBoss)
    {
        boss = myBoss;
    }

    public override void Enter()
    {
        Debug.Log("big boys state, we should not be moving");

        boss.isItTimeToRest = true;

        GameObject bullet = ObjectPooler.SharedInstance.GetPooledObject("coffee");

        if (bullet != null)
        {
            bullet.transform.position = boss.transform.position;
            bullet.transform.rotation = boss.transform.rotation;
            bullet.SetActive(true);
        }

        parentFSM.SetCurrentState(new FirstbossShootSmallBullets(boss));
    }
}


//in this state the boss is firing a baragge of small bullets
public class FirstbossShootSmallBullets : State
{
    public FirstBoss boss;
    //this counter will be used to count time between bullet firing
    private float currentTime = 0f;
    //this counter will be used to count when is time to change state
    private float timeCounter = 0f;

    public FirstbossShootSmallBullets(FirstBoss myBoss)
    {
        boss = myBoss;
    }

    public override void Enter()
    {
        boss.isItTimeToRest = false;

        //parentFSM.SetCurrentState(new FirstBossSecondState(boss));
    }

    public override void CalledInUpdate()
    {
        currentTime += Time.deltaTime;
        if (currentTime > boss.smallBulletInterval)
        {
            ShootBullet();
            currentTime = 0f;
        }

        timeCounter += Time.deltaTime;
        if (timeCounter > boss.timeShootingSmallBullets)
        {
            parentFSM.SetCurrentState(new FirstBossRestState(boss));
        }
    }

   public void ShootBullet()
    {
        GameObject bullet = ObjectPooler.SharedInstance.GetPooledObject("bullet");

        if (bullet != null)
        {
            bullet.transform.position = boss.transform.position;
            bullet.transform.rotation = boss.transform.rotation;
            bullet.SetActive(true);
        }
    }
}


public class FirstBossRestState : State
{
    public FirstBoss boss;
    private float currentTime = 0f;

    public FirstBossRestState(FirstBoss myBoss)
    {
        boss = myBoss;
    }

    public override void Enter()
    {
        boss.isItTimeToRest = true;

        boss.collider.enabled = true;
    }

    public override void CalledInUpdate()
    {
        currentTime += Time.deltaTime;
        if (currentTime > boss.secondsToRest)
        {
            parentFSM.SetCurrentState(new FirstBossShootBigBullet(boss));
        }
    }

    public override void Exit()
    {
        boss.collider.enabled = false;
    }

}

public class FirstBoss : MonoBehaviour
{
    private FSM fsm; //finite state machine
    public GameObject player;

    // Movement speed
    public float speed = 2f;
    public int health = 20;

    public float smallBulletInterval = 1f;
    public float bigBulletInterval = 2f;

    //how many big bullet are going to be shot in fire state
    public int bigBulletsToShoot = 3;
    public int bigBulletsShot = 0;

    //how much time the boss will move around shooting small bullets
    public float timeShootingSmallBullets = 5f;

    //how much time the boss will rest
    public float secondsToRest = 3f;

    //timeToRest means that 
    public bool isItTimeToRest = false;

    public Collider2D collider;
    private GameMaster gm;

    // List of transforms for waypoints
    public List<GameObject> waypoints;

    // Current target waypoint
    private Transform targetWaypoint;


    void Start()
    {
        //fsm = new FSM(new FirstBossShootBigBullet(this));
        fsm = new FSM(new FirstBossShootBigBullet(this));
        collider = GetComponent<Collider2D>();
        player = GameObject.FindWithTag("Player");
        gm = GameMaster.Instance;


        SetNextWaypoint();
    }


    private void Update()
    {
        fsm.CalledInUpdate();
    }

    public void FixedUpdate()
    {
        if (!isItTimeToRest)
        {
            //Debug.Log("we are in the fixedUpdate and the timeToRest = " + timeToRest.ToString());
            // Move towards the target waypoint
            MoveTowardsWaypoint();
        }
    }

    void MoveTowardsWaypoint()
    {
        

        if (targetWaypoint != null)
        {
            // Calculate the step size
            float step = speed * Time.deltaTime;

            // Move the game object towards the target waypoint
            transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, step);

            // Check if the position of the game object and target waypoint are approximately equal
            if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.1f)
            {
                // Set the next waypoint
                SetNextWaypoint();
            }
        }
    }

    void SetNextWaypoint()
    {
        // Choose a random waypoint from the list
        targetWaypoint = waypoints[Random.Range(0, waypoints.Count)].transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //Debug.Log("we have contact");
            Player playerScript = player.GetComponent<Player>();

            if (playerScript.inAttack)
            {
                playerScript.inCooldown = false;
                health--;
            }

            if (health <= 0)
            {
                gm.KillBoss();
                gameObject.SetActive(false);
                
            }
        }
    }

    
}

   

