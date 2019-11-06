using UnityEngine;

public class Punch : MonoBehaviour
{
    int damage;
    PlayerInputs pi;
    MainInterfaceController mc;

    void Start()
    {
        damage = 1;
        pi = GameObject.Find("Player").GetComponent<PlayerInputs>();
        mc = GameObject.Find("Canvas").GetComponent<MainInterfaceController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (pi.Attacking && mc.CurrentWeaponV == (byte)MainInterfaceController.AttackTypes.Punch)
            {
                if (other.gameObject.GetComponent<EnemyController>().CanDamage)
                {
                    other.gameObject.GetComponent<EnemyController>().TakeDamage(damage);
                    other.gameObject.GetComponent<EnemyController>().Ani.Play("Hit");
                }
            }
        }
    }
}
