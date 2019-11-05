using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int ActualLifes { get; set; }
    public int MaxLifes { get; set; }
    public int SearchStatus { get; set; }

    MainInterfaceController mic;

    void Start()
    {
        MaxLifes = 5;
        ActualLifes = MaxLifes;
        SearchStatus = 0;
        mic = GameObject.Find("Canvas").GetComponent<MainInterfaceController>();
    }

    void Update()
    {
        Debug.Log(SearchStatus);
    }

    public void TakeDamage(int i)
    {
        Debug.Log("uuuhhhg");
        ActualLifes -= i;
        mic.PrintLifes();

        if (ActualLifes <= 0)
        {
            Debug.Log("Muerto");
        }
    }
}
