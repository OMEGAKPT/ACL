using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    int damage;

    PlayerInputs pi;
    PlayerController pc;
    MainInterfaceController mc;

    void Start()
    {
        damage = 2;
        pi = GameObject.Find("Player").GetComponent<PlayerInputs>();
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
        mc = GameObject.Find("Canvas").GetComponent<MainInterfaceController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (pi.Attacking && mc.CurrentWeaponV == (byte)MainInterfaceController.AttackTypes.Sword)
            {
                if (other.gameObject.GetComponent<EnemyController>().CanDamage)
                {
                    other.gameObject.GetComponent<EnemyController>().TakeDamage(damage);
                    Debug.Log("Acertado");
                    other.gameObject.GetComponent<EnemyController>().Ani.Play("Hit");
                }
            }
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            if (gameObject.GetComponentInParent<EnemyController>().Attacking)
            {
                pc.TakeDamage(damage);
                pi.Ani.Play("Hit");
            }
        }
    }
}
