using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float m_speed = 20.0f;

    private Rigidbody m_RB = null;

    private void Awake()
    {
        m_RB = GetComponent<Rigidbody>();
        Destroy(gameObject, 5);
    }

    private void Update()
    {
        m_RB.MovePosition(m_RB.transform.position += transform.forward * m_speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            Player player = collision.gameObject.GetComponent<Player>();
            player.Hit();
        }
        Destroy(gameObject);
    }
}