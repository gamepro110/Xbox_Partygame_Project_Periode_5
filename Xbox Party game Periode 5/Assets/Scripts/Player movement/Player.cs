using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    private bool m_controlerControl = true;

    //speeds
    public Vector3 M_Speed;

    public float Speed = 5;
    private float holdspeed;

    [Range(0, 1)]
    public float M_deadzone;

    [SerializeField] private XboxController Player_Nummber;
    private float _XAxis;
    private float _YAxis;
    private Rigidbody rig;

    [SerializeField] private Transform m_respawnLocation;

    private Vector3 m_startScale = Vector3.zero;

    private void Start()
    {
        m_startScale = transform.localScale;

        rig = GetComponent<Rigidbody>();
        holdspeed = Speed;
    }

    private void Update()
    {
        if (!m_controlerControl)
        {
            return;
        }
        _XAxis = -XCI.GetAxis(XboxAxis.LeftStickX, Player_Nummber);
        _YAxis = -XCI.GetAxis(XboxAxis.LeftStickY, Player_Nummber);

        M_Speed = new Vector3(_XAxis, 0f, _YAxis);

        if (M_Speed.magnitude <= M_deadzone)
        {
            M_Speed = Vector3.zero;
        }
        else
        {
            M_Speed = M_Speed.normalized * ((M_Speed.magnitude - M_deadzone) / (1 / M_deadzone));
        }
        M_Speed.Normalize();

        rig.MovePosition(transform.position + (M_Speed * Speed) * Time.deltaTime);

        if (XCI.GetButtonDown(XboxButton.A, Player_Nummber))
        {
            gameObject.transform.localScale = new Vector3(m_startScale.x * 1.25f, m_startScale.y * 0.7f, m_startScale.z);
            Speed *= 0.45f;
        }
        if (XCI.GetButtonUp(XboxButton.A, Player_Nummber))
        {
            gameObject.transform.localScale = m_startScale;
            Speed = holdspeed;
        }
    }

    public void Hit()
    {
        m_controlerControl = false;

        StartCoroutine(PlayerLerpToRespawn());

        transform.rotation = new Quaternion(0, 0, 0, 0);
        //Debug.Log($"{gameObject.name} got hit");
        //transform.position = m_respawnLocation.position;
    }

    private IEnumerator PlayerLerpToRespawn()
    {
        if (transform.position != m_respawnLocation.position)
        {
            transform.position = Vector3.Slerp(transform.position, m_respawnLocation.position, 3 * Time.deltaTime);

            if (transform.position == m_respawnLocation.position)
            {
                new WaitForSeconds(1);
                m_controlerControl = true;
                yield return new WaitForSeconds(0);
            }
        }
        StopCoroutine(PlayerLerpToRespawn());
        yield return new WaitForSeconds(0);
        //TODO try to make this work
    }
}