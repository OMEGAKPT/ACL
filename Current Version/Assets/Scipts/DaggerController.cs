using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerController : MonoBehaviour
{
    int damage;
    PlayerInputs pi;
    MainInterfaceController mc;

    void Start()
    {
        damage = 3;
        pi = GameObject.Find("Player").GetComponent<PlayerInputs>();
        mc = GameObject.Find("Canvas").GetComponent<MainInterfaceController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Enemy"))
        {
            if (mc.CurrentWeaponV == (byte)MainInterfaceController.AttackTypes.Dagger)
            {
                if (other.gameObject.GetComponent<EnemyController>().CanDamage)
                {
                    other.gameObject.GetComponent<EnemyController>().TakeDamage(damage);
                    other.gameObject.GetComponent<EnemyController>().Ani.Play("Hit");
                }
            }
        }
        else if (other.gameObject.CompareTag("Chicken"))
        {
            other.gameObject.GetComponent<ChickenController>().TakeDamage(damage);
        }
    }
}
