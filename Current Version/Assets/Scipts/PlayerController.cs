using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public int ActualLifes { get; set; }
    public int MaxLifes { get; set; }
    public int SearchStatus { get; set; }
    public int Daggers { get; set; }

    MainInterfaceController mic;
    Animator ani;

    public bool IsDead { get; set; }

    void Start()
    {
        ani = GetComponent<Animator>();
        IsDead = false;
        MaxLifes = 20;
        ActualLifes = MaxLifes;
        SearchStatus = 0;
        Daggers = 10;
        mic = GameObject.Find("Canvas").GetComponent<MainInterfaceController>();
        InvokeRepeating("RegenerateLife",1,3);
        mic.PrintLifes();
        mic.UpdateDaggers(Daggers);
    }

    public void TakeDamage(int i)
    {
        ActualLifes -= i;
        mic.PrintLifes();

        if (ActualLifes <= 0)
        {
            IsDead = true;
            ani.SetBool("Dead", IsDead);
            StartCoroutine(Dead());
        }
    }

    public void UpdateDaggers(int n)
    {
        if ((Daggers + n) < 0)
            Daggers = 0;
        else
            Daggers += n;

        mic.UpdateDaggers(Daggers);
    }

    public void RegenerateLife()
    {
        if (SearchStatus == (int)PlayerInputs.Status.Good && ActualLifes < MaxLifes)
        {
            ActualLifes++;
            mic.PrintLifes();
        }
    }

    IEnumerator Dead()
    {
        ani.Play("Die");
        Debug.Log("animado");
        yield return new WaitForSeconds(10);
        SceneManager.LoadScene("Welcome");
    }
}
