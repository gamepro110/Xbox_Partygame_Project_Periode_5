using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float m_speed = 20.0f;

    private void Awake()
    {
        Destroy(gameObject, 5);
    }

    private void Update()
    {
        transform.position += transform.forward * m_speed * Time.deltaTime;
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