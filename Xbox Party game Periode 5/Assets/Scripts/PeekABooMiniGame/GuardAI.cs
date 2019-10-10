using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardAI : MonoBehaviour
{
    [SerializeField] private bool m_lookingTowardsPlayers = true;

    #region Field of View variables

    [SerializeField] private float m_rotationSpeed = 10.0f;
    [SerializeField] private RangedFloat m_minWaitTime;
    [SerializeField] private RangedFloat m_maxWaitTime;

    public float m_viewRadius = 10.0f;
    [Range(60.0f, 180.0f)] public float m_viewAngle = 90.0f;

    [SerializeField] private LayerMask m_obstacleMask;
    [SerializeField] private LayerMask m_targetMask;

    [SerializeField, Range(1.0f, 30.0f)] private float m_viewMeshResolution = 10.0f;
    [SerializeField, Range(0.01f, 1.0f)] private float m_viewMeshRefreshRate = 1;

    [SerializeField] private MeshFilter m_viewMeshFilter = null;
    private Mesh m_viewMesh;

    [SerializeField] private List<GameObject> PlayersInView = new List<GameObject>(5);

    #endregion Field of View variables

    [Space(10)]
    [SerializeField] private GameObject m_LookLocation = null;

    [SerializeField] private GameObject m_BackLookLocation = null;

    [SerializeField] private GameObject m_bulletPrefab = null;
    [SerializeField] private Vector3 m_lookTarget = Vector3.zero;

    private GameObject body = null;

    private void Awake()
    {
        body = GetComponentInChildren<Transform>().GetChild(0).gameObject;
    }

    private void Start()
    {
        m_viewMesh = new Mesh
        {
            name = "View Mesh"
        };
        m_viewMeshFilter.mesh = m_viewMesh;

        m_lookTarget = m_LookLocation.transform.position;

        StartCoroutine(FindTargetWithDelay(m_viewMeshRefreshRate));

        StartCoroutine(StartGuardBehavior());
    }

    private void Update()
    {
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

    #region behavior

    private void CharacterRotate()
    {
        Vector3 difference = m_lookTarget - gameObject.transform.position;
        transform.forward = Vector3.Slerp(transform.forward, difference, m_rotationSpeed * Time.deltaTime);

        Vector3.Slerp(transform.forward, difference, m_rotationSpeed * Time.deltaTime);

        body.transform.rotation = new Quaternion(0, transform.rotation.y, 0, 0);
    }

    private IEnumerator StartGuardBehavior()
    {
        PlayersInView.Clear();

        yield return new WaitForSeconds(m_maxWaitTime.maxValue);

        if (m_lookingTowardsPlayers)
        {
            Debug.Log($"{PlayersInView.Count} visable players");
        }

        while (PlayersInView.Count >= 1)
        {
            //TODO its not getting here
            yield return AimAndShootPlayer();
        }

        StartCoroutine(GuardRotating());
        StopCoroutine(StartGuardBehavior());
    }

    private IEnumerator GuardRotating()
    {
        yield return new WaitForSeconds(GetMinWaitTime());

        m_lookingTowardsPlayers = !m_lookingTowardsPlayers;

        m_lookTarget = m_lookingTowardsPlayers ? m_LookLocation.transform.position : m_BackLookLocation.transform.position;

        //Debug.Log($"looks in player direction = {m_lookingTowardsPlayers}");
        //Debug.Log(m_lookTarget = m_lookingTowardsPlayers ? m_LookLocation.transform.position : m_BackLookLocation.transform.position);

        if (ConeVisual(m_LookLocation.transform.position) || ConeVisual(m_BackLookLocation.transform.position))
        {
            yield return new WaitForSeconds(GetRandomWaitTime());

            yield return StartCoroutine(StartGuardBehavior());

            StopCoroutine(GuardRotating());
        }
    }

    private IEnumerator AimAndShootPlayer()
    {
        //TODO check why the bullet acts wierd
        for (int i = 0; i < PlayersInView.Count; i++)
        {
            //Debug.Log($"Found player ({PlayersInView[i]})");
            m_lookTarget = PlayersInView[i].transform.position;

            Vector3 offset = new Vector3(0, 0.5f);
            Instantiate(m_bulletPrefab, transform.position + offset, transform.rotation);

            PlayersInView.RemoveAt(i);

            yield return new WaitForSeconds(2.0f);
        }

        StopCoroutine(AimAndShootPlayer());
    }

    private float GetMinWaitTime()
    {
        return Random.Range(m_minWaitTime.minValue, m_minWaitTime.maxValue);
    }

    private float GetMaxWaitTime()
    {
        return Random.Range(m_maxWaitTime.minValue, m_maxWaitTime.maxValue);
    }

    private float GetRandomWaitTime()
    {
        return Random.Range(GetMinWaitTime(), GetMaxWaitTime());
    }

    #endregion behavior

    #region Field Of View

    protected IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
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
            if (Vector3.Angle(transform.forward, dirToTarget) < m_viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.transform.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, m_obstacleMask))
                {
                    //TODO use PlayersInView to find players

                    if (target.GetComponent<Player>())
                    {
                        if (!PlayersInView.Contains(target))
                        {
                            PlayersInView.Add(target);
                            //Debug.Log(target.gameObject);
                        }
                        //do things here
                    }
                }
            }
        }
    }

    private void DrawFieldOfView()
    {
        int StepCount = Mathf.RoundToInt(m_viewAngle * m_viewMeshResolution);
        float stepAngleSize = m_viewAngle / StepCount;

        List<Vector3> viewpoints = new List<Vector3>();

        for (int i = 0; i < StepCount; i++)
        {
            float angle = transform.eulerAngles.y - m_viewAngle / 2 + stepAngleSize * i;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(m_lookTarget, Vector3.one);
    }
}