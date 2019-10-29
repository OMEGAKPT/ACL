using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainInterfaceController : MonoBehaviour
{
    [SerializeField] Image bEye;
    [SerializeField] Image bHand;
    [SerializeField] Image bCrossedHand;
    [SerializeField] Image bSteps;

    [SerializeField] Image currentWeapon;
    [SerializeField] Image[] weapons;

    PlayerMovement pm;

    byte currentWeaponV;
    byte lastWeapon;
    string[] actions = { "asesinar","atacar","puñetazo","atacar"};

    // Start is called before the first frame update
    void Start()
    {
        pm = FindObjectOfType<PlayerMovement>();
        bHand.GetComponentInChildren<TMP_Text>().SetText("correr");
        currentWeaponV = 2;

        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWeaponV != 0)
        {
            bCrossedHand.enabled = true;
            bCrossedHand.GetComponentInChildren<TMP_Text>().enabled = true;
            bCrossedHand.GetComponentInChildren<TMP_Text>().SetText(actions[currentWeaponV]);
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

        currentWeaponV = (byte)(Input.GetKeyDown("1")? 0 : Input.GetKeyDown("2")? 1 : Input.GetKeyDown("3")? 2 : Input.GetKeyDown("4")? 3 : lastWeapon);

        if (currentWeaponV != lastWeapon)
        {
            lastWeapon = currentWeaponV;
            StartCoroutine(ChangeWeapon());
        }
    }

    IEnumerator ChangeWeapon()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].enabled = true;
        }

        currentWeapon.sprite = weapons[currentWeaponV].sprite;

        yield return new WaitForSeconds(1);

        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].enabled = false;
        }
    }
}
