using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : Targetable
{
    private float Destroytimer;

    private void Start()
    {
        Destroytimer = Random.Range(4, 6);
    }
    private void Update()
    {
        Destroytimer -= Time.deltaTime;
        if(Destroytimer <= 0)
        {
            Destroy(gameObject);
        }
    }
}
