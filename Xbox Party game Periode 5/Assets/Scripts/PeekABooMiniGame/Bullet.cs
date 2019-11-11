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

        if (Physics.BoxCast(transform.position, Vector3.one / 2, transform.forward, out RaycastHit hit))
        {
            if (hit.collider.GetComponent<Targetable>())
            {
                hit.collider.GetComponent<Targetable>().Hit();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
}