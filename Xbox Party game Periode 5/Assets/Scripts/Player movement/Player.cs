using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
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

    private void Start()
    {
        rig = GetComponent<Rigidbody>();
        holdspeed = Speed;
    }

    private void Update()
    {
        _XAxis = XCI.GetAxisRaw(XboxAxis.LeftStickX, Player_Nummber);
        _YAxis = XCI.GetAxisRaw(XboxAxis.LeftStickY, Player_Nummber);

        M_Speed = new Vector3(_XAxis, 0f, _YAxis);

        if (M_Speed.magnitude < M_deadzone)
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
            gameObject.transform.localScale = new Vector3(0.012f, 0.007f, 0.01f);
            Speed = 2;
        }
        if (XCI.GetButtonUp(XboxButton.A, Player_Nummber))
        {
            gameObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            Speed = holdspeed;
        }
    }
}