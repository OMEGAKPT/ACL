﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class PlayerInputs : MonoBehaviour
{
    [SerializeField] GameObject fakeSword;
    [SerializeField] GameObject sword;
    Camera camera;
    GameObject focused;

    PlayerMovement pm;
    PlayerController pc;
    MainInterfaceController mic;
    public Animator Ani { get; set; }

    public bool Jumping { get; set; }
    public bool Attacking { get; set; }
    public bool ChangedDirection { get; set; }
    float speedJumping;
    bool moving;
    bool praying;
    bool highProfile;
    bool visionActived;

    int rangeVision;

    void Start()
    {
        focused = null;
        sword.SetActive(false);
        pc = GetComponent<PlayerController>();
        pm = GetComponent<PlayerMovement>();
        mic = GameObject.Find("Canvas").GetComponent<MainInterfaceController>();
        Ani = GetComponent<Animator>();
        camera = Camera.main;
        Jumping = false;
        speedJumping = 4;
        moving = false;
        praying = false;
        highProfile = false;
        Attacking = false;
        visionActived = false;
        pc.SearchStatus = (int)Status.Good;
        rangeVision = 30;
    }

    public bool IsActiveSword()
    {
        return sword.activeSelf;
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
        InputFocusEnemy();
        InputEagleVision();
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
        if (Input.GetButtonDown("Fire1") && !Attacking)
        {
            pc.SearchStatus = (int)Status.Alert;
            StartCoroutine("StopAlert");
            if (mic.CurrentWeaponV == (byte)MainInterfaceController.AttackTypes.Punch)
            {
                Attacking = true;
                Ani.SetBool("Punching", Attacking);
                DisableSword();
            }
            else if (mic.CurrentWeaponV == (byte)MainInterfaceController.AttackTypes.Sword)
            {
                Attacking = true;
                Ani.SetBool("AttackSword",Attacking);

                fakeSword.SetActive(false);
                sword.SetActive(true);
            }

            StartCoroutine("StopAttacking");
        }
    }

    IEnumerator StopAlert()
    {
        yield return new WaitForSeconds(5);
        if (pc.SearchStatus != (int)Status.Wanted)
        {
            pc.SearchStatus = (int)Status.Good;
        }
    }

    public void DisableSword()
    {
        fakeSword.SetActive(true);
        sword.SetActive(false);
    }

    IEnumerator StopAttacking()
    {
        yield return new WaitForSeconds(0.6f);
        Attacking = false;
        Ani.SetBool("Punching", Attacking);
        Ani.SetBool("AttackSword", Attacking);
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

    public void InputFocusEnemy()
    {
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.DrawRay(camera.transform.position, camera.transform.forward, Color.red);
            RaycastHit hit;
            
            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 100))
            {
                if (hit.collider.gameObject.CompareTag("Enemy"))
                {
                    if (focused == null)
                    {
                        focused = hit.collider.gameObject;
                        focused.GetComponent<EnemyController>().ChangeFocus(true);
                        camera.GetComponent<CameraController>().SetFocus(focused);
                    }
                    else if (focused != hit.collider.gameObject)
                    {
                        focused.GetComponent<EnemyController>().ChangeFocus(false);
                        focused = hit.collider.gameObject;
                        focused.GetComponent<EnemyController>().ChangeFocus(true);
                        camera.GetComponent<CameraController>().SetFocus(focused);
                    }
                    else
                    {
                        focused.GetComponent<EnemyController>().ChangeFocus(false);
                        focused = null;
                        camera.GetComponent<CameraController>().SetFocus(null);
                    }
                }
                else
                {
                    if (focused != null)
                    {
                        focused.GetComponent<EnemyController>().ChangeFocus(false);
                        focused = null;
                        camera.GetComponent<CameraController>().SetFocus(null);
                    }
                }
            }  
        }
    }

    public void InputEagleVision()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, rangeVision);

        if (!visionActived && Input.GetKeyDown(KeyCode.E) && (pm.InputC == Vector2.zero))
        {
            foreach(Collider c in colliders)
            {
                if (c.CompareTag("Enemy"))
                {
                    c.gameObject.GetComponent<EnemyController>().ChangeHighlight(true);
                }
                
            }

            camera.GetComponent<ColorCorrectionCurves>().enabled = true;
            visionActived = true;
        }

        if (pm.InputC != Vector2.zero)
        {
            camera.GetComponent<ColorCorrectionCurves>().enabled = false;
            StartCoroutine(DisableVisionEnemies(colliders));
            visionActived = false;
        }
    }

    IEnumerator DisableVisionEnemies(Collider[] colliders)
    {
        yield return new WaitForSeconds(6);

        if (!visionActived)
        {
            foreach (Collider c in colliders)
            {
                if (c.CompareTag("Enemy"))
                {
                    c.gameObject.GetComponent<EnemyController>().ChangeHighlight(false);
                }

            }
        }
    }

    public void SetVarAni()
    {
        Ani.SetInteger("Velocity", pm.RunSpeed);
        Ani.SetBool("Jumping", Jumping);
        Ani.SetBool("Praying", praying);
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
