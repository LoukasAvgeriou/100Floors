using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//first boss first state, the boss throws homming missiles
public class FirstBossFirstState : State
{
    public FirstBoss boss;

    public FirstBossFirstState(FirstBoss myBoss)
    {
        boss = myBoss;
    }

    //shoot
    GameObject bullet = ObjectPooler.SharedInstance.GetPooledObject("bullet");
     /*   if (bullet != null)
        {
            bullet.transform.position = enemy.transform.position;
            bullet.transform.rotation = enemy.transform.rotation;
            bullet.SetActive(true);
        } */
}

public class FirstBoss : MonoBehaviour
{
    private FSM fsm;

    public Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        //fsm = new FSM();
    }

    public void FixedUpdate()
    {
        fsm.CalledInFixedUpdate();
    }
}
