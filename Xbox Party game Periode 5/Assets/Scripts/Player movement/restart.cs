using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class restart : MonoBehaviour
{
    [SerializeField] private  GameObject Player1, Player2, Player3, Player4;
    [SerializeField] private  Transform rsp1, rsp2, rsp3, rsp4;

    private void Update()
    {
        

    }
    public  void RespawnPlayer1()
    {
        Player1.transform.position = rsp1.transform.position;
    }
    public  void RespawnPlayer2()
    {
        Player2.transform.position = rsp2.transform.position;
    }
    public  void RespawnPlayer3()
    {
        Player3.transform.position = rsp3.transform.position;
    }
    public  void RespawnPlayer4()
    {
        Player4.transform.position = rsp4.transform.position;
    }
    
}