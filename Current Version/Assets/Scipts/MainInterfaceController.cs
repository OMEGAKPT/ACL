using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainInterfaceController : MonoBehaviour
{
    [SerializeField] Image bHand;
    [SerializeField] Image bCrossedHand;
    [SerializeField] Image bSteps;
    [SerializeField] Image helpMenu;
    [SerializeField] Image status;
    [SerializeField] TMP_Text helpText;
    [SerializeField] TMP_Text daggers;
    [SerializeField] GameObject panel;

    [SerializeField] Image currentWeapon;
    [SerializeField] Image[] actions;
    [SerializeField] Image[] weapons;

    [SerializeField] GameObject life;
    [SerializeField] GameObject lifeBar;
    
    PlayerMovement pm;
    PlayerController pc;

    public byte CurrentWeaponV { get; set; }
    byte lastWeapon;
    string[] actionsC = { "asesinar","atacar","puñetazo","atacar"};

    void Start()
    {
        pm = GameObject.Find("Player").GetComponent<PlayerMovement>();
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
        panel.SetActive(false);
        bHand.GetComponentInChildren<TMP_Text>().SetText("correr");
        CurrentWeaponV = (byte)AttackTypes.Punch;
        lastWeapon = (byte)AttackTypes.Punch;

        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].enabled = false;
            weapons[i].color = Color.black;
            actions[i].color = Color.black;
        }
        currentWeapon.color = Color.black;
        PrintLifes();

        daggers.text = "" + pc.Daggers;

        StatusColor();
    }

    public bool GetPanel() { return panel.activeSelf; }

    public void ChangePanel()
    {
        panel.active = !panel.active;
    }
    
    void Update()
    {
        if (!panel.activeSelf)
        {
            HelpMenu();
            if (lastWeapon != 0)
            {
                bCrossedHand.enabled = true;
                bCrossedHand.GetComponentInChildren<TMP_Text>().enabled = true;
                bCrossedHand.GetComponentInChildren<TMP_Text>().SetText(actionsC[CurrentWeaponV]);
            }
            else
            {
                bCrossedHand.enabled = false;
                bCrossedHand.GetComponentInChildren<TMP_Text>().enabled = false;
            }

            if (Input.GetButton("Fire2"))
            {
                bHand.enabled = true;
                bHand.GetComponentInChildren<TMP_Text>().enabled = true;
                bHand.GetComponentInChildren<TMP_Text>().SetText("correr");
                bSteps.GetComponentInChildren<TMP_Text>().SetText("saltar");
            }
            else
            {
                bSteps.GetComponentInChildren<TMP_Text>().SetText("mezclarse");
                bHand.enabled = false;
                bHand.GetComponentInChildren<TMP_Text>().enabled = false;
            }

            InputsColor();

            CurrentWeaponV = (Input.GetKeyDown("1") ? (byte)AttackTypes.HiddenBlade :
                    Input.GetKeyDown("2") ? (byte)AttackTypes.Sword :
                    Input.GetKeyDown("3") ? (byte)AttackTypes.Punch :
                    Input.GetKeyDown("4") ? (byte)AttackTypes.Dagger :
                    lastWeapon);

            if (CurrentWeaponV != lastWeapon)
            {
                lastWeapon = CurrentWeaponV;
                StartCoroutine(ChangeWeapon());
            }
        }
    }

    public void HelpMenu()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            helpMenu.enabled = !helpMenu.enabled;
            helpText.enabled = !helpText.enabled;
        }
    }

    public void InputsColor()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            actions[0].color = Color.yellow;
            StartCoroutine(SetBlackActions());
        }
        if (Input.GetButtonDown("Jump"))
        {
            actions[3].color = Color.green;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            StartCoroutine(SetBlackActions());
        }
        if (Input.GetButtonDown("Fire1"))
        {
            actions[2].color = Color.red;
            StartCoroutine(SetBlackActions());
        }
        if (Input.GetButtonDown("Fire3"))
        {
            actions[1].color = Color.blue;
        }
        else if (Input.GetButtonUp("Fire3"))
        {
            StartCoroutine(SetBlackActions());
        }
    }

    public void StatusColor()
    {
        if (pc.SearchStatus == (int)PlayerInputs.Status.Good)
        {
            status.color = Color.cyan;
        }
        else if (pc.SearchStatus == (int)PlayerInputs.Status.Alert)
        {
            status.color = Color.yellow;
        }
        else if (pc.SearchStatus == (int)PlayerInputs.Status.Wanted)
        {
            status.color = Color.red;
        }
        else if (pc.SearchStatus == (int)PlayerInputs.Status.Hide)
        {
            status.color = Color.white;
        }
    }

    public void PrintLifes()
    {
        Destroy(GameObject.Find("GroupLifes").gameObject);
        float x = -75;
        float actualX = x;
        float widthLife = ((RectTransform)life.transform).sizeDelta.x;
        float space = 5;
        float y = 10;
        GameObject c = new GameObject();
        c.name = "GroupLifes";        
        c.transform.SetParent(lifeBar.transform);
        c.transform.localPosition = Vector3.zero;

        for (int i = 0; i < pc.MaxLifes; i++)
        {            
            GameObject l = Instantiate(life, Vector3.zero, Quaternion.identity, c.transform);
            l.transform.localPosition = new Vector3(actualX, y, 0);
            if (i >= pc.ActualLifes)
                l.GetComponent<Image>().color = Color.cyan;
            l.name = "Life";
            actualX += widthLife + space;

            if (i % 10 == 9)
            {
                y -= 15;
                actualX = x;
            }
        }
    }

    public void UpdateDaggers(int n)
    {
        daggers.text = "" + n;
    }

    IEnumerator SetBlackActions()
    {
        yield return new WaitForSeconds(1);

        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].color = Color.black;
        }
    }

    IEnumerator ChangeWeapon()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].enabled = true;
        }

        currentWeapon.sprite = weapons[CurrentWeaponV].sprite;

        yield return new WaitForSeconds(1);

        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].enabled = false;
        }
    }

    public enum AttackTypes
    {
        HiddenBlade,
        Sword,
        Punch,
        Dagger
    }
}
