using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using UnityEngine.UI;
using XInputDotNetPure;

[RequireComponent(typeof(Rigidbody))]
public class Player : Targetable
{
    [SerializeField] private bool m_controlerHasControl = true;
    [SerializeField] private float m_lerpToRespawnTimer = 10f;
    [SerializeField, Range(0.1f, 1.0f)] private float m_distanceToRegainControll = 0.2f;

    //speeds
    [Space(10)] public Vector3 M_Speed;

    public float Speed = 5;
    //private float holdspeed;

    [Range(0, 1)]
    public float M_deadzone;

    [SerializeField] private XboxController Player_Nummber = XboxController.Any;
    private float _XAxis;
    private float _YAxis;
    private Rigidbody rig;

    [Space(10)]
    [SerializeField] private Transform m_respawnLocation;

    //private Vector3 m_startScale = Vector3.zero;

    [SerializeField] private Text playerScoreText;
    private int playerScore;

    private void Start()
    {
        if (Player_Nummber == XboxController.Any)
        {
            Debug.LogWarning("Assign Players Player number.");
            UnityEditor.EditorApplication.isPlaying = false;
        }
        rig = GetComponent<Rigidbody>();

        //m_startScale = transform.localScale;
        //holdspeed = Speed;

        playerScore = 0;
        UpdateUI();
    }

    private void Update()
    {
        if (m_controlerHasControl)
        {
            #region Movement

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

            #endregion Movement

            //if (XCI.GetButtonDown(XboxButton.A, Player_Nummber))
            //{
            //    gameObject.transform.localScale = new Vector3(m_startScale.x * 1.25f, m_startScale.y * 0.7f, m_startScale.z);
            //    Speed *= 0.45f;
            //}
            //if (XCI.GetButtonUp(XboxButton.A, Player_Nummber))
            //{
            //    gameObject.transform.localScale = m_startScale;
            //    Speed = holdspeed;
            //}
        }
        else
        {
            if ((transform.position - m_respawnLocation.position).magnitude <= m_distanceToRegainControll)
            {
                m_controlerHasControl = true;
                return;
            }
            M_Speed = Vector3.zero;

            //slowly moves the player from the position where he get shot to his respawn position
            transform.position = Vector3.Lerp(transform.position, m_respawnLocation.position, (m_lerpToRespawnTimer * Vector3.Distance(transform.position, m_respawnLocation.position)) * Time.deltaTime);

            //slowly rotates the character from any rotation to standing upright
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, m_distanceToRegainControll * 20 * Time.deltaTime);
        }
    }

    public override void Hit()
    {
        m_controlerHasControl = false;
        PlayerIndex index = PlayerIndex.One;
        switch (Player_Nummber)
        {
            case XboxController.First:
                {
                    index = PlayerIndex.One;
                    break;
                }

            case XboxController.Second:
                {
                    index = PlayerIndex.Two;
                    break;
                }

            case XboxController.Third:
                {
                    index = PlayerIndex.Three;
                    break;
                }

            case XboxController.Fourth:
                {
                    index = PlayerIndex.Four;
                    break;
                }
        }
        StartCoroutine(StartRumble(index, 0.5f, 4));
    }

    private IEnumerator StartRumble(PlayerIndex playerIndex, float time, float strength)
    {
        GamePad.SetVibration(playerIndex, strength, strength);
        yield return new WaitForSeconds(time);
        GamePad.SetVibration(playerIndex, 0, 0);
    }

    public override Vector3 GetMovementSpeed()
    {
        return M_Speed;
    }

    public void AtFinish()
    {
        m_controlerHasControl = false;

        playerScore++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        string color = "";
        int playerNum = 0;

        switch (Player_Nummber)
        {
            case XboxController.First:
                {
                    playerNum = 1;
                    color = "red";
                    break;
                }
            case XboxController.Second:
                {
                    playerNum = 2;
                    color = "blue";
                    break;
                }
            case XboxController.Third:
                {
                    playerNum = 3;
                    color = "yellow";
                    break;
                }
            case XboxController.Fourth:
                {
                    playerNum = 4;
                    color = "magenta";
                    break;
                }
        }
        playerScoreText.text = $"<color={color}>Player{playerNum}: {playerScore} </color>";
    }

    public override float GetDeadzone()
    {
        return M_deadzone;
    }
}