using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] GameObject fakeSword;
    [SerializeField] GameObject sword;
    [SerializeField] GameObject focus;

    public int Lifes { get; set; }
    public bool CanDamage { get; set; }
    public NavMeshAgent Agent { get; set; }
    GameObject player;

    public int StateEnemy { get; set; }
    public bool Attacking { get; set; }

    GameObject[] waypoints;
    int currentWaypoint;
    GameObject objetive;

    float timer;
    float timeWithoutSee;
    float timeWaitAttack;
    PlayerInputs pi;

    public Animator Ani { get; set; }

    void Start()
    {
        focus.SetActive(false);
        sword.SetActive(false);
        Agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        Ani = GetComponent<Animator>();
        Lifes = Random.Range(2,4);
        CanDamage = true;
        Agent.speed = (int)Speed.Walking;
        Ani.SetInteger("Velocity", (int)Agent.speed);
        StateEnemy = (int)State.Patrol;
        Attacking = false;

        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        currentWaypoint = Random.Range(0,waypoints.Length);
        objetive = null;
        Agent.updateRotation = false;
        timer = 0;
        timeWithoutSee = 5;
        timeWaitAttack = 2;
        pi = GameObject.Find("Player").GetComponent<PlayerInputs>();
    }

    void Update()
    {
        switch (StateEnemy)
        {
            case (int)State.Idle:
                Sight();
                break;
            case (int)State.Patrol:
                Patrol();
                Sight();
                break;
            case (int)State.Attack:
                Attack();
                break;
            case (int)State.Die:
                Die();
                break;
        }       
    }

    public void Die()
    {
        Ani.Play("Die");

        objetive = null;
        Agent.isStopped = true;
    }

    public void Patrol()
    {
        if (Vector3.Distance(transform.position,waypoints[currentWaypoint].transform.position) >= 4)
        {
            Agent.SetDestination(waypoints[currentWaypoint].transform.position);
            transform.LookAt(waypoints[currentWaypoint].transform.position);

        }
        else if(Vector3.Distance(transform.position, waypoints[currentWaypoint].transform.position) <= 4)
        {
            currentWaypoint = Random.Range(0, waypoints.Length);
        }
    }

    public void Attack()
    {
        fakeSword.SetActive(false);
        sword.SetActive(true);

        if (Vector3.Distance(transform.position, objetive.transform.position) >= 2f)
        {
            Agent.isStopped = false;
            Agent.speed = (int)Speed.Running;
            Ani.SetInteger("Velocity", (int)Agent.speed);
            Agent.SetDestination(objetive.transform.position);
            transform.LookAt(objetive.transform);

            if (Vector3.Distance(transform.position, objetive.transform.position) > 10)
            {
                timer += Time.deltaTime;
            }
            else
                timer = 0;

            if (timer >= timeWithoutSee)
            {
                fakeSword.SetActive(true);
                sword.SetActive(false);
                timer = 0;
                StateEnemy = (int)State.Patrol;
                Agent.speed = (int)Speed.Walking;
                Ani.SetInteger("Velocity", (int)Agent.speed);
                transform.LookAt(Vector3.zero);
            }
        }
        else
        {
            Agent.speed = (int)Speed.Idle;
            Ani.SetInteger("Velocity", (int)Agent.speed);
            Agent.velocity = Vector3.zero;
            Agent.isStopped = true;

            timer += Time.deltaTime;

            if (timer >= timeWaitAttack)
            {
                transform.LookAt(objetive.transform);
                timer = 0;
                Attacking = true;
                Ani.SetBool("AttackSword", Attacking);
                StartCoroutine(StopAttacking());
                timeWaitAttack = Random.Range(2,6);
            }
        }
    }

    IEnumerator StopAttacking()
    {
        yield return new WaitForSeconds(0.6f);
        Attacking = false;
        Ani.SetBool("AttackSword", Attacking);
    }


    public void Sight()
    {
        RaycastHit hit;

        Debug.DrawRay(transform.position + Vector3.up * 1.7f, (transform.forward + transform.right).normalized * 50, Color.green);
        Debug.DrawRay(transform.position + Vector3.up * 1.7f, (transform.forward - transform.right).normalized * 50, Color.green);
        Debug.DrawRay(transform.position + Vector3.up * 1.7f, (transform.forward).normalized * 50, Color.green);

        if (Physics.Raycast(transform.position + Vector3.up * 1.7f, transform.forward.normalized * 50, out hit, 50) ||
            Physics.Raycast(transform.position + Vector3.up * 1.7f, (transform.forward + transform.right).normalized * 50, out hit, 50) ||
            Physics.Raycast(transform.position + Vector3.up * 1.7f, (transform.forward - transform.right).normalized * 50, out hit, 50))
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                if (pi.IsActiveSword() ||  pi.Attacking)
                {
                    StateEnemy = (int)State.Attack;
                    objetive = hit.collider.gameObject;
                    Agent.speed = (int)Speed.Running;
                    Ani.SetInteger("Velocity", (int)Agent.speed);

                    Debug.Log("YOU");
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (CanDamage)
        {
            CanDamage = false;
            Lifes -= damage;

            if (Lifes <= 0)
            {
                transform.LookAt(objetive.transform);
                StateEnemy = (int)State.Die;
                StartCoroutine(RemoveBody());
            }
            StartCoroutine(WaitCanDamage());
        }
    }

    public void ChaneFocus(bool v)
    {
        focus.SetActive(v);
    }

    IEnumerator RemoveBody()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }

    IEnumerator WaitCanDamage()
    {
        yield return new WaitForSeconds(0.6f);
        if (StateEnemy != (int)State.Die)
        {
            CanDamage = true;
        }
    }

    public enum State
    {
        Idle,
        Patrol,
        Attack,
        Die
    }

    public enum Speed
    {
        Idle = 0,
        Walking = 3,
        Running = 6
    }
}
