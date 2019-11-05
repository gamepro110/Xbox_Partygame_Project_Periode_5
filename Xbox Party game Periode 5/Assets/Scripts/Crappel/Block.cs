using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : Targetable
{
    private poolding poolding_;
    private float Destroytimer;

    public float SetTimer { get => Destroytimer; set => Destroytimer = value; }

    private void Start()
    {
        poolding_ = GetComponent<poolding>();
        Destroytimer = Random.Range(3, 4);
    }

    private void Update()
    {
        Destroytimer -= Time.deltaTime;
        if(Destroytimer <= 0)
        {
            poolding_.ReturnToPool();
        }
    }
    //public override void Hit()
    //{
    //    poolding_.ReturnToPool();
    //}
}
