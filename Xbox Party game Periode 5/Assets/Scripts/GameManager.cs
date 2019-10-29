using UnityEngine;
using UnityEngine.UI;
using XboxCtrlrInput;

public class GameManager : MonoBehaviour
{
    public static GameManager Access;
    private Player[] m_players = new Player[4];

    [SerializeField] private Canvas m_pauseMenu = null;

    [SerializeField] private Button m_continue = null;
    [SerializeField] private Button m_quit = null;

    [SerializeField] private RectTransform m_cursorTransform = null;

    private bool m_gameIsPaused = false;
    public bool GameIsPaused { get => m_gameIsPaused; }

    [Range(0, 1)] private float m_gameSpeed = 1.0f;

    private float m_xCanvasAxis;
    private float m_yCanvasAxis;
    [SerializeField] private float m_cursorMovementSpeed = 5.0f;

    private void Awake()
    {
        if (Access)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(this);
            Access = this;
        }
    }

    private void Start()
    {
        m_continue.onClick.AddListener(ContinueButton);
        m_quit.onClick.AddListener(QuitButton);

        m_players = FindObjectsOfType<Player>();
    }

    private void Update()
    {
        if (XCI.GetButtonUp(XboxButton.Start, XboxController.Any))
        {
            m_gameIsPaused = !m_gameIsPaused;
        }

        if (m_gameIsPaused)
        {
            PauseOrUnpause();

            m_xCanvasAxis = XCI.GetAxis(XboxAxis.LeftStickX, XboxController.First);
            m_yCanvasAxis = XCI.GetAxis(XboxAxis.LeftStickY, XboxController.First);

            MovePauseScreenCursor();
        }
        else
        {
            PauseOrUnpause();
        }
    }

    private void PauseOrUnpause()
    {
        for (int i = 0; i < m_players.Length; i++)
        {
            m_players[i].PausePlayer(!m_gameIsPaused);
        }

        m_pauseMenu.enabled = m_gameIsPaused;
        Time.timeScale = m_gameIsPaused ? 0 : 1;
    }

    private void ContinueButton()
    {
        Debug.Log("Continue");
    }

    private void QuitButton()
    {
        Debug.Log("Quit");
    }

    private void MovePauseScreenCursor()
    {
        m_cursorTransform.localPosition += Vector3.right * m_xCanvasAxis * m_cursorMovementSpeed;
        m_cursorTransform.localPosition += Vector3.up * m_yCanvasAxis * m_cursorMovementSpeed;

        //TODO ask anne for help with UI Cursor
    }
}