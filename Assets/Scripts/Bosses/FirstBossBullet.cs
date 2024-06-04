using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBossBullet : MonoBehaviour
{
    public GameObject player;
    public UpgradesControllerSO upgradeControllerSO;

    public float speed = 1.0f; // Speed of movement
    public Vector3 controlOffset = new Vector3(0, 2, 0); // Offset for the control point
    private float threshold = 0.1f; // Distance threshold to update target

    private Vector3 startPosition;
    private Vector3 currentTargetPosition;
    private Vector3 controlPoint;
    private float t;
    private bool isMoving = false;

   

    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        // Capture the target's position at the moment the bullet is enabled
        startPosition = transform.position;
        currentTargetPosition = player.transform.position;
        controlPoint = (startPosition + currentTargetPosition) / 2 + controlOffset;
        t = 0;
        isMoving = true;
    }

    void Update()
    {
        if (isMoving)
        {
            if (t < 1)
            {
                t += Time.deltaTime * speed;
                Vector3 newPosition = CalculateBezierPoint(t, startPosition, controlPoint, currentTargetPosition);
                transform.position = newPosition;
            }
            else
            {
                // Bullet has reached the current target position
                t = 0;
                startPosition = transform.position;
                currentTargetPosition = player.transform.position;
                controlPoint = (startPosition + currentTargetPosition) / 2 + controlOffset;
            }

            // Check if the bullet is close to the target position
            if (Vector3.Distance(transform.position, currentTargetPosition) < threshold)
            {
                // Update the target to the current position of the moving object
                startPosition = transform.position;
                currentTargetPosition = player.transform.position;
                controlPoint = (startPosition + currentTargetPosition) / 2 + controlOffset;
                t = 0;
            }
        }
    }

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0; // (1 - t)^2 * P0
        p += 2 * u * t * p1; // 2(1 - t)t * P1
        p += tt * p2; // t^2 * P2

        return p;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            Player playerScript = player.GetComponent<Player>();

            if (playerScript.inDefence && upgradeControllerSO.returnBullets)
            {
                playerScript.inCooldown = false;
                //GoBack();
                gameObject.SetActive(false);
            }
            else if (playerScript.inDefence)
            {
                gameObject.SetActive(false);
                playerScript.inCooldown = false;
            }
            else if (playerScript.inAttack && upgradeControllerSO.breakableBullets)
            {
                gameObject.SetActive(false);
                playerScript.inCooldown = false;
            }
            else
            {
                GameMaster.Instance.KillPlayer(col.gameObject);
            }
        }
        else
        {
          /*  if (friendlyFire)
            {
                friendlyFire = false;
                gm.KillEnemy(col.gameObject);
                gameObject.SetActive(false);

            } */
        }
    }
}
