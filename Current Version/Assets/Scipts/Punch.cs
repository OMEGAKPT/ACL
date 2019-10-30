using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : MonoBehaviour
{
    PlayerInputs pi;
    // Start is called before the first frame update
    void Start()
    {
        pi = GameObject.Find("Player").GetComponent<PlayerInputs>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (pi.Attacking)
            {
                if (other.GetComponent<EnemyController>().CanDamage)
                {
                    other.GetComponent<EnemyController>().TakeDamage(1);
                    Debug.Log("Acertado");
                }
            }
        }
    }
}
