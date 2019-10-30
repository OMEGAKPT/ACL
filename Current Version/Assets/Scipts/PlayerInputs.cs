using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    [SerializeField] GameObject fakeSword;
    [SerializeField] GameObject sword;

    PlayerMovement pm;
    PlayerController pc;
    MainInterfaceController mic;
    Animator ani;

    public bool Jumping { get; set; }
    public bool Attacking { get; set; }
    public bool ChangedDirection { get; set; }
    float speedJumping;
    bool moving;
    bool praying;
    bool highProfile;

    void Start()
    {
        sword.SetActive(false);
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
        pm = GameObject.Find("Player").GetComponent<PlayerMovement>();
        mic = GameObject.Find("Canvas").GetComponent<MainInterfaceController>();
        ani = GetComponent<Animator>();

        Jumping = false;
        speedJumping = 4;
        moving = false;
        praying = false;
        highProfile = false;
        Attacking = false;
        pc.SearchStatus = (int)Status.Good;
    }


    void Update()
    {
        Inputs();
    }

    public void Inputs()
    {
        InputProfile();
        InputRun();
        InputSpace();
        InputAttack();
        SetVarAni();
    }

    public void InputProfile()
    {
        if (Input.GetButton("Fire2"))
            highProfile = true;
        else
            highProfile = false;
    }

    public bool CanJump()
    {
        if (Jumping)
        {
            return false;
        }
        if (!highProfile)
        {
            return false;
        }
        if (!Input.GetButtonDown("Jump"))
        {
            return false;
        }

        return true;
    }

    public void InputAttack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (mic.CurrentWeaponV == (byte)MainInterfaceController.AttackTypes.Punch)
            {
                Attacking = true;
                ani.SetBool("Punching", Attacking);
            }
            else if (mic.CurrentWeaponV == (byte)MainInterfaceController.AttackTypes.Sword)
            {
                Attacking = true;
                ani.SetBool("AttackSword",Attacking);

                fakeSword.SetActive(false);
                sword.SetActive(true);
            }

            StartCoroutine("StopAttacking");
        }
    }

    IEnumerator StopAttacking()
    {
        yield return new WaitForSeconds(1f);
        Attacking = false;
        ani.SetBool("Punching", Attacking);
        ani.SetBool("AttackSword", Attacking);
    }


    public void InputRun()
    {
        if (highProfile && Input.GetButton("Fire3") && pm.InputC != Vector2.zero)
        {
            pm.RunSpeed = (int)PlayerMovement.Speed.Running;
            pm.JumpSpeed = (int)PlayerMovement.Jump.Running;
        }
        else
        {
            if (pm.InputC != Vector2.zero)
            {
                pm.RunSpeed = (int)PlayerMovement.Speed.Walking;
            }
            else
            {
                pm.RunSpeed = (int)PlayerMovement.Speed.Idle;
                pm.JumpSpeed = (int)PlayerMovement.Jump.Idle;
            }
        }
    }

    public void InputSpace()
    {
        if (Input.GetButton("Jump") && pm.RunSpeed <= (int)PlayerMovement.Speed.Walking && !highProfile)
        {
            praying = true;
            pc.SearchStatus = (int)Status.Hide;

            if (pm.RunSpeed == (int)PlayerMovement.Speed.Walking)
            {
                pm.RunSpeed = (int)PlayerMovement.Speed.Praying;
            }
        }
        else if (CanJump())
        {
            Jumping = true;
            gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * pm.JumpSpeed, ForceMode.Impulse);
        }
        else
        {
            pc.SearchStatus = (int)Status.Good;
            praying = false;
        }

        if (Jumping && (moving || pm.InputC != Vector2.zero))
        {
            moving = true;
            transform.Translate(transform.forward * speedJumping * Time.deltaTime, Space.World);
        }
        else
        {
            moving = false;
        }
    }

    public void SetVarAni()
    {
        ani.SetInteger("Velocity", pm.RunSpeed);
        ani.SetBool("Jumping", Jumping);
        ani.SetBool("Praying", praying);
    }


    public bool IsGrounded(Transform trans)
    {
        return Physics.Raycast(trans.position, Vector3.down, 0.1f);
    }

    void OnCollisionEnter(Collision other)
    {
        if (Jumping)
        {
            Jumping = false;
        }
    }

    public enum Status
    {
        Good,
        Alert,
        Wanted,
        Hide
    }
}
