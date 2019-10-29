using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard_AI : MonoBehaviour
{
    #region Field of View variables

    [Header("Field Of View Variables")]
    [SerializeField] private RangedFloat m_minWaitTime;

    [SerializeField] private RangedFloat m_maxWaitTime;

    public float m_viewRadius = 10.0f;
    [SerializeField, MinMaxRange(20.0f, 60.0f)] private RangedFloat m_viewAngle;
    public float m_currentViewAngle = 60.0f;

    [SerializeField] private LayerMask m_obstacleMask;
    [SerializeField] private LayerMask m_targetMask;

    [SerializeField, Range(1.0f, 30.0f)] private float m_viewMeshResolution = 10.0f;
    [SerializeField, Range(0.01f, 1.0f)] private float m_viewMeshRefreshRate = 1;

    [SerializeField] private MeshFilter m_viewMeshFilter = null;
    private Mesh m_viewMesh;

    [SerializeField] private List<GameObject> m_targetsInView;

    #endregion Field of View variables

    [Header("Rotation variables")]
    [SerializeField] private float m_rotationSpeed = 10.0f;

    [SerializeField] private bool m_lookingTowardsPlayers = true;

    [SerializeField] private Transform m_LookLocation = null;
    [SerializeField] private Transform m_BackLookLocation = null;

    private Vector3 m_lookTarget;

    private float m_waitingTimer = 0;
    private float m_targetWaitTime = 1;

    [SerializeField, Range(0.01f, 2.0f)] private float m_fireRate = 1.0f;
    private float m_nextFire = 0;

    [SerializeField] private GameObject m_bulletPrefab = null;
    private bool PlayerVisable = false;

    [SerializeField] private SimpleAudioEvent simpleAudio = null;
    private AudioSource audioSource = null;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        m_viewMesh = new Mesh
        {
            name = "View Mesh"
        };
        m_viewMeshFilter.mesh = m_viewMesh;
    }

    private void Start()
    {
        m_targetsInView = new List<GameObject>();

        m_lookTarget = m_LookLocation.position;

        m_currentViewAngle = m_viewAngle.maxValue;

        StartCoroutine(FindTargetWithDelay(m_viewMeshRefreshRate));
    }

    private void Update()
    {
        //rotation
        if (!PlayerVisable)
        {
            m_currentViewAngle = Mathf.Clamp(m_currentViewAngle += 200 * Time.deltaTime, m_viewAngle.minValue, m_viewAngle.maxValue);

            //rotation direction
            if (m_lookingTowardsPlayers)
            {
                m_lookTarget = m_LookLocation.position;
            }
            else
            {
                m_lookTarget = m_BackLookLocation.position;
            }

            m_waitingTimer += Time.deltaTime / 2;
            if (m_waitingTimer > m_targetWaitTime)
            {
                m_lookingTowardsPlayers = !m_lookingTowardsPlayers;
                m_waitingTimer = 0;

                if (m_lookingTowardsPlayers)
                {
                    m_targetWaitTime = GetMaxWaitTime();
                }
                else
                {
                    m_targetWaitTime = GetMinWaitTime();
                }
            }
        }
        else
        {
            List<Targetable> targets = new List<Targetable>();
            for (int i = 0; i < PlayersInView.Count; i++)
            {
                targets[i] = PlayersInView[i].GetComponent<Targetable>();
            }

            if (targets[0].GetMovementSpeed().magnitude > targets[0].GetDeadzone())
            {
                m_currentViewAngle = Mathf.Clamp(m_currentViewAngle -= 200 * Time.deltaTime, m_viewAngle.minValue, m_viewAngle.maxValue);
                m_lookTarget = PlayersInView[0].transform.position;
                transform.LookAt(PlayersInView[0].transform.position);

                while (Time.time > m_nextFire)
                {
                    simpleAudio.Play(audioSource);

                    Instantiate(m_bulletPrefab, transform.position, transform.rotation);

                    PlayersInView.RemoveAt(0);

                    m_nextFire = Time.time + m_fireRate;
                }
            }
            else
            {
                PlayersInView.RemoveAt(0);
            }
        }

        if (PlayersInView.Count >= 1)
        {
            PlayerVisable = true;
        }
        else
        {
            PlayerVisable = false;
        }

        CharacterRotate();
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    /// <summary>
    /// 1 if parallel, 0 if perpendicular, -1 if reverse parallel
    /// </summary>
    /// <param name="objectLocation"></param>
    /// <returns>the object you are looking at</returns>
    public bool ConeVisual(Vector3 objectLocation)
    {
        float cosAngle = Vector3.Dot((objectLocation - transform.position).normalized, transform.forward);
        float angle = Mathf.Acos(cosAngle) * Mathf.Rad2Deg;
        return angle < m_viewRadius;
    }

    private void CharacterRotate()
    {
        Vector3 difference = m_lookTarget - gameObject.transform.position;
        transform.forward = Vector3.Slerp(transform.forward, difference, m_rotationSpeed * Time.deltaTime);
    }

    #region Waiting time values

    private float GetMinWaitTime()
    {
        return Random.Range(m_minWaitTime.minValue, m_minWaitTime.maxValue);
    }

    private float GetMaxWaitTime()
    {
        return Random.Range(m_maxWaitTime.minValue, m_maxWaitTime.maxValue);
    }

    //private float GetRandomWaitTime()
    //{
    //    return Random.Range(GetMinWaitTime(), GetMaxWaitTime());
    //}

    #endregion Waiting time values

    #region Field Of View

    protected IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            //remove this line to remove delay
            yield return new WaitForSeconds(delay);
            FindVisableTarget();
        }
    }

    private void FindVisableTarget()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, m_viewRadius, m_targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            GameObject target = targetsInViewRadius[i].gameObject;
            Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < m_currentViewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.transform.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, m_obstacleMask))
                {
                    if (target.GetComponent<Targetable>())
                    {
                        if (!m_targetsInView.Contains(target))
                        {
                            m_targetsInView.Add(target);
                        }
                    }
                }
            }
        }
    }

    private void DrawFieldOfView()
    {
        int StepCount = Mathf.RoundToInt(m_currentViewAngle * m_viewMeshResolution);
        float stepAngleSize = m_currentViewAngle / StepCount;

        List<Vector3> viewpoints = new List<Vector3>();

        for (int i = 0; i < StepCount; i++)
        {
            float angle = transform.eulerAngles.y - m_currentViewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);
            viewpoints.Add(newViewCast.point);
            //Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * m_viewRadius, Color.red);
        }

        int vertexCount = viewpoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewpoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }
        m_viewMesh.Clear();
        m_viewMesh.vertices = vertices;
        m_viewMesh.triangles = triangles;
        m_viewMesh.RecalculateNormals();
    }

    private ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, m_viewRadius, m_obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * m_viewRadius, m_viewRadius, globalAngle);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

    #endregion Field Of View
}