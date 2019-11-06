using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChickenController : MonoBehaviour
{
    [SerializeField] Material baseMaterial;
    [SerializeField] Material highlight;

    public NavMeshAgent Agent { get; set; }
    public bool CanDamage { get; set; }

    GameObject player;
    PlayerInputs pi;
    PlayerController pc;

    GameObject[] waypoints;
    int currentWaypoint;
    int speed;

    Animator ani;
    bool looking;
    bool inGroup;
    bool attacking;

    int lifes;
    int damage;

    void Start()
    {
        lifes = 4;
        damage = 1;
        CanDamage = true;
        inGroup = false;
        attacking = false;
        Agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        pi = GameObject.Find("Player").GetComponent<PlayerInputs>();
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        currentWaypoint = Random.Range(0, waypoints.Length);
        speed = (int)Speed.Walking;
        Agent.speed = speed;
        ani = GetComponent<Animator>();
        ani.SetBool("Walk",true);

        looking = false;
    }

    public void ChangeHighlight(bool c)
    {
        if (c)
            GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial = highlight;
        else
            GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial = baseMaterial;
    }

    void Update()
    {
        if (!inGroup)
        {
            if (Vector3.Distance(transform.position, waypoints[currentWaypoint].transform.position) >= 4)
            {
                if (!looking)
                {
                    Agent.SetDestination(waypoints[currentWaypoint].transform.position);
                    transform.LookAt(waypoints[currentWaypoint].transform.position);
                }
            }
            else if (Vector3.Distance(transform.position, waypoints[currentWaypoint].transform.position) <= 4)
            {
                currentWaypoint = Random.Range(0, waypoints.Length);
                looking = true;
                ani.SetBool("TurnHead", true);
                ani.SetBool("Walk", false);
                StartCoroutine(TurnHead());
            }
        }
        else
        {
            if (player.transform.position.y < 4 && !pc.IsDead)
            {
                Agent.SetDestination(player.transform.position);
                transform.LookAt(player.transform.position);
            }
            else
            {
                ChangeGroup(false);
                pc.SearchStatus = (int)PlayerInputs.Status.Good;
                FindObjectOfType<MainInterfaceController>().StatusColor();
            }
        }
       
    }

    public void TakeDamage(int damage)
    {
        if (!inGroup)
        {
            ChangeGroup(true);
            GameObject[] chickens = GameObject.FindGameObjectsWithTag("Chicken");
            pc.SearchStatus = (int)PlayerInputs.Status.Wanted;
            FindObjectOfType<MainInterfaceController>().StatusColor();
            foreach (GameObject c in chickens)
            {
                c.gameObject.GetComponent<ChickenController>().ChangeGroup(true);
            }
            
        }

        if (CanDamage)
        {
            CanDamage = false;
            lifes -= damage;
            
            if (lifes <= 0)
            {
                FindObjectOfType<SpawnController>().SubstractChicken();
                Destroy(gameObject);
            }
            StartCoroutine(WaitCanDamage());
        }
    }

    public void ChangeGroup(bool  v)
    {
        if (v)
        {
            inGroup = true;
            speed = (int)Speed.Running;
            Agent.speed = speed;
            ani.SetBool("Run", true);
            ani.SetBool("Walk", false);
        }
        else
        {
            inGroup = false;
            speed = (int)Speed.Walking;
            Agent.speed = speed;
            ani.SetBool("Run", false);
            ani.SetBool("Walk", true);
        }
    }

    IEnumerator WaitCanDamage()
    {
        yield return new WaitForSeconds(1);
        CanDamage = true;
    }

    IEnumerator TurnHead()
    {
        yield return new WaitForSeconds(6);
        looking = false;
        ani.SetBool("TurnHead", false);
        ani.SetBool("Walk", true);
    }

    IEnumerator StopAttacking()
    {
        yield return new WaitForSeconds(3);
        attacking = false;
    }

    public enum Speed
    {
        Idle = 0,
        Walking = 3,
        Running = 6
    }

    private void OnTriggerEnter(Collider other)
    {
        if (inGroup)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (!attacking)
                {
                    attacking = true;
                    
                    pc.TakeDamage(damage);
                    pi.Ani.Play("Hit");

                    StartCoroutine(StopAttacking());
                }
            }
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (inGroup)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (!attacking)
                {
                    attacking = true;

                    pc.TakeDamage(damage);
                    pi.Ani.Play("Hit");

                    StartCoroutine(StopAttacking());
                }
            }
        }
    }


}
