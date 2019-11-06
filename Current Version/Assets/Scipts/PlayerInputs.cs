using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;

public class PlayerInputs : MonoBehaviour
{
    [SerializeField] GameObject fakeSword;
    [SerializeField] GameObject sword;
    [SerializeField] GameObject dagger;
    [SerializeField] GameObject pointDagger;
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
    bool canDagger;

    int rangeVision;
    int distanceDager;
    int damageHiddenBlade;

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
        canDagger = true;
        pc.SearchStatus = (int)Status.Good;
        rangeVision = 30;
        distanceDager = 2000;
        damageHiddenBlade = 10;
    }

    public bool IsActiveSword()
    {
        return sword.activeSelf;
    }

    void Update()
    {
        if (!pc.IsDead && !mic.GetPanel())
        {
            Inputs();
        }
    }

    public void Inputs()
    {
        InputProfile();
        InputRun();
        InputSpace();
        InputAttack();
        InputFocusEnemy();
        InputEagleVision();
        CheckSword();
        InputOptions();
        SetVarAni();
    }

    public void CheckSword()
    {
        if (mic.CurrentWeaponV != (byte)MainInterfaceController.AttackTypes.Sword)
        {

            sword.SetActive(false);
            fakeSword.SetActive(true);
        }
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

    public void InputOptions()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            mic.ChangePanel();
        }
    }

    public void InputAttack()
    {
        if (Input.GetButtonDown("Fire1") && mic.CurrentWeaponV == (byte)MainInterfaceController.AttackTypes.Dagger)
        {
            if (canDagger && pc.Daggers > 0)
            {
                ChangeStatusAlert();
                canDagger = false;
                Vector3 q = new Vector3();

                if (transform.rotation.eulerAngles.y >= 230 && transform.rotation.eulerAngles.y < 330)
                {
                    q = Vector3.forward;
                }
                else if (transform.rotation.eulerAngles.y >= 130 && transform.rotation.eulerAngles.y < 230)
                {
                    q = Vector3.left;
                }
                else if (transform.rotation.eulerAngles.y >= 40 && transform.rotation.eulerAngles.y < 130)
                {
                    q = Vector3.back;
                }
                else
                {
                    q = Vector3.right;
                }

                GameObject dag = Instantiate(dagger, pointDagger.transform.position, Quaternion.AngleAxis(90, q));
                Rigidbody rg = dag.GetComponent<Rigidbody>();
                rg.AddForce(transform.forward * distanceDager, ForceMode.Force);

                Destroy(dag.gameObject, 5);
                StartCoroutine(WaitNextDagger());
                pc.UpdateDaggers(-1);
            }
        }
        else if (Input.GetButtonDown("Fire1") && mic.CurrentWeaponV == (byte)MainInterfaceController.AttackTypes.HiddenBlade)
        {
            if (focused != null && Vector3.Distance(transform.position,focused.transform.position) < 2)
            {
                focused.GetComponent<EnemyController>().TakeDamage(damageHiddenBlade);
            }
        }
        else if (Input.GetButtonDown("Fire1") && !Attacking)
        {
            ChangeStatusAlert();
            if (mic.CurrentWeaponV == (byte)MainInterfaceController.AttackTypes.Punch)
            {
                Attacking = true;
                Ani.SetBool("Punching", Attacking);
                DisableSword();
            }
            else if (mic.CurrentWeaponV == (byte)MainInterfaceController.AttackTypes.Sword)
            {
                ChangeStatusAlert();
                Attacking = true;
                Ani.SetBool("AttackSword", Attacking);

                fakeSword.SetActive(false);
                sword.SetActive(true);
            }

            StartCoroutine("StopAttacking");
        }
    }

    public void ChangeStatusAlert()
    {
        if (pc.SearchStatus != (int)Status.Wanted)
        {
            pc.SearchStatus = (int)Status.Alert;
            StartCoroutine("StopAlert");
            mic.StatusColor();
        }
    }

    public void RemoveFocus()
    {
        if (focused != null)
        {
            focused.GetComponent<EnemyController>().ChangeFocus(false);
            focused = null;
            camera.GetComponent<CameraController>().SetFocus(null);
        }
    }

    IEnumerator WaitNextDagger()
    {
        yield return new WaitForSeconds(1);
        canDagger = true;
    }

    IEnumerator StopAlert()
    {
        yield return new WaitForSeconds(5);
        if (pc.SearchStatus != (int)Status.Wanted)
        {
            pc.SearchStatus = (int)Status.Good;
            mic.StatusColor();
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
            mic.StatusColor();

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
            if (praying)
            {
                praying = false;
                pc.SearchStatus = (int)Status.Good;
                mic.StatusColor();
            }
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
                        RemoveFocus();
                    }
                }
                else
                {
                    if (focused != null)
                    {
                        RemoveFocus();
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
            StopAllCoroutines();
            foreach (Collider c in colliders)
            {
                if (c != null && c.CompareTag("Enemy"))
                {
                    c.gameObject.GetComponent<EnemyController>().ChangeHighlight(true);
                }
                else if (c != null && c.CompareTag("Chicken"))
                {
                    c.gameObject.GetComponent<ChickenController>().ChangeHighlight(true);
                }

            }

            camera.GetComponent<ColorCorrectionCurves>().enabled = true;
            visionActived = true;
        }

        if (pm.InputC != Vector2.zero)
        {
            camera.GetComponent<ColorCorrectionCurves>().enabled = false;
            StartCoroutine(DisableVision());
            visionActived = false;
        }
    }

    IEnumerator DisableVision()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, rangeVision * 100);
        yield return new WaitForSeconds(6);

        if (!visionActived)
        {
            foreach (Collider c in colliders)
            {
                if (c != null && c.CompareTag("Enemy"))
                {
                    c.gameObject.GetComponent<EnemyController>().ChangeHighlight(false);
                }
                else if (c != null && c.CompareTag("Chicken"))
                {
                    c.gameObject.GetComponent<ChickenController>().ChangeHighlight(false);
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
