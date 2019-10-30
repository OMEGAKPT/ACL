using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int Lifes { get; set; }
    public bool CanDamage { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        Lifes = Random.Range(2,4);
        CanDamage = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        if (CanDamage)
        {
            CanDamage = false;
            Lifes -= damage;

            if (Lifes <= 0)
            {
                Destroy(gameObject);
            }
            StartCoroutine(WaitCanDamage());
        }
    }

    IEnumerator WaitCanDamage()
    {
        yield return new WaitForSeconds(1f);
        CanDamage = true;
    }
}
