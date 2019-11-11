using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class Crappel : MonoBehaviour
{
    public List<GameObject> ObjDrop;

    private Rigidbody rig;

    [Space(10)] public Vector3 M_Speed;

    [SerializeField] private XboxController ForPlayerNumber = XboxController.Any;

    public float Speed = 5;

    [Range(0, 1)]
    public float M_deadzone;

    private float _XAxis;
    private float _YAxis;

    private float Cooldowntimer = 1f;

    private ObjectPooling m_pool = null;

    private void Start()
    {
        m_pool = GetComponent<ObjectPooling>();
        rig = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _XAxis = -XCI.GetAxis(XboxAxis.RightStickX, ForPlayerNumber);
        _YAxis = -XCI.GetAxis(XboxAxis.RightStickY, ForPlayerNumber);

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

        Cooldowntimer -= Time.deltaTime;
        if (Cooldowntimer <= 0)
        {
            if (XCI.GetButtonUp(XboxButton.X, ForPlayerNumber))
            {
                //Instantiate(ObjDrop[0], transform.position, Quaternion.identity);
                m_pool.InstantiateItem(transform.position, Quaternion.identity);
                Cooldowntimer = 1f;
            }
        }
    }
}