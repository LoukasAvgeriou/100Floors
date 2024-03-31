using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [System.Serializable]
    public class Stats
    {
        public float Speed = 5f;
        public float AttackTime = 0.3f;
        public float AttackStun = 0.4f;
        public float AttackSpeedModifier = 3f;
        public float dashCooldown = 3f;
    }

    public Stats stats = new Stats();

    public bool inCooldown = false;
    public float cooldownTimer = 0f;

    public bool inDefence = false;
    public float endDefence = 0f;

    public bool inAttack = false;
    private bool deadly = false;
    private float endAttack = 0f;
    private float endStun = 0f;
    private Vector3 targetPos;

    private Vector2 movement;
    public bool moveWithMouse = true;
    public float moveSpeed = 5f;


    private Rigidbody2D rb;
    //private Animator anim;
    //private TrailRenderer trail;

    private GameMaster gm;

    private SpriteRenderer spriteRenderer;


    // [SerializeField] private GameObject pivot;

    public void ChangeMoveStyle()
    {
        if (moveWithMouse)
        {
            moveWithMouse = false;
        }
        else
        {
            moveWithMouse = true;
        }
    }


    // Use this for initialization
    private void Start()
    {
        gm = GameMaster.Instance;
        rb = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        //anim = gameObject.GetComponent<Animator>();
        //trail = gameObject.GetComponent<TrailRenderer>();

        // Calculate mouse position
        targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPos.z = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        //newstuff
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (inAttack && Time.time >= endStun)
        {
            inAttack = false;
            deadly = false;
        }

        if (inDefence && Time.time >= endDefence)
        {
            inDefence = false;
            //Debug.Log("cooldown = " + cooldownTimer);
        }

        if (Input.GetMouseButtonDown(0) && Time.time >= endStun && !inCooldown)
        {

            inAttack = true;
            endAttack = Time.time + stats.AttackTime;
            endStun = endAttack + stats.AttackStun;

            inCooldown = true;
            cooldownTimer = 0;
        }

        if (Input.GetMouseButtonDown(1) && Time.time >= endStun && !inCooldown)
        {
            //Debug.Log("mouse is working");
            inDefence = true;
            endDefence = Time.time + stats.dashCooldown;
            //endStun = endDefence + stats.AttackStun;

            inCooldown = true;
            cooldownTimer = 0;

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
        //where the character looks
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0;

        if (!inAttack)
        {
            if (mouse.x < transform.position.x)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else if (mouse.x > transform.position.x)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }

            if (mouse.y > transform.position.y)
            {
                //spriteRenderer.sprite = behindSprite;
            }
            else if (mouse.y < transform.position.y)
            {
                // spriteRenderer.sprite = frontSprite;
            }
        }

        // Debug.Log(rb.velocity.ToString());


    }

    void FixedUpdate()
    {
        if (rb.drag == 50)
            rb.drag = 20;

        if (inDefence)
        {
            rb.velocity = Vector2.zero;
        }

        if (!inAttack && !inDefence)
        {
            if (moveWithMouse)
            {


                //deactivate the trail effect
                //trail.enabled = false;

                // Calculate mouse position
                targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                targetPos.z = transform.position.z;

                if (Vector3.Distance(targetPos, transform.position) > 0.2)
                {
                    var direction = targetPos - transform.position;
                    rb.AddRelativeForce(direction.normalized * stats.Speed, ForceMode2D.Force);
                    Debug.DrawLine(targetPos, transform.position, Color.green);

                   


                    

                }
                else
                {
                    rb.drag = 50;


                }

            }
            else
            {
                rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
            }
        }
        else if (inAttack)
        {
            if (Time.time <= endAttack)
            {
                targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                targetPos.z = transform.position.z;

                targetPos.x += targetPos.x - transform.position.x;
                targetPos.y += targetPos.y - transform.position.y;
                var direction = targetPos - transform.position;
                rb.AddRelativeForce(direction.normalized * (stats.Speed * stats.AttackSpeedModifier), ForceMode2D.Force);
                Debug.DrawLine(targetPos, transform.position, Color.red);


            }

        }
    }



    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "mainEnemy")
        {   // We collided with a main enemy.
            //SwordEnemy swordEnemy = col.gameObject.GetComponent<SwordEnemy>();

            Debug.Log("hitted sword enemy");

            if (inAttack)
            {   // We are currently dashing, kill enemy.
                gm.KillmainEnemy(col.gameObject);
                ScreenShakeController.instance.StartShake(.1f, .1f);
                inCooldown = false;
            }
            /*  else if (swordEnemy.deadly)
              {   // The enemy is dashing and we are vulnerable, die.
                  gm.KillPlayer(this);
              } */
        }
        else if (col.gameObject.tag == "rangedEnemy")
        {   // We collided with a ranged enemy.
            //RangedEnemy rangedEnemy = col.gameObject.GetComponent<RangedEnemy>();

            Debug.Log("hitted ranged enemy");

            if (inAttack)
            {   // We are currently dashing, kill enemy.
                gm.KillEnemy(col.gameObject);
                ScreenShakeController.instance.StartShake(.1f, .1f);
                inCooldown = false;
            }
        }
        else if (col.gameObject.tag == "Arrow")
        {   // We collided with an arrow.
            Debug.Log("hitted arrow");

            if (inAttack)
            {   // We are currently dashing, kill enemy.
                gm.BreakArrow(col.gameObject);
                ScreenShakeController.instance.StartShake(.1f, .1f);
                inCooldown = false;
            }
        }
    }
}


