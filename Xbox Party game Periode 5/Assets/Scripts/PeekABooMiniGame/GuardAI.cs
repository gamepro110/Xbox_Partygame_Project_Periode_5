using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardAI : MonoBehaviour
{
    [SerializeField] private Quaternion m_targetRotation;

    [SerializeField] private float m_rotationSpeed;
    [SerializeField] private RangedFloat m_minWaitTime;
    [SerializeField] private RangedFloat m_maxWaitTime;

    #region Field of View variables

    public float m_viewRadius = 10.0f;
    [Range(60.0f, 180.0f)] public float m_viewAngle = 90.0f;

    [SerializeField] private LayerMask m_obstacleMask;
    [SerializeField] private LayerMask m_targetMask;

    [SerializeField, Range(1.0f, 30.0f)] private float m_viewMeshResolution = 10.0f;
    [SerializeField, Range(0.1f, 1.0f)] private float m_viewMeshRefreshRate = 1;

    [SerializeField] private MeshFilter m_viewMeshFilter = null;
    private Mesh m_viewMesh;

    [SerializeField] private List<GameObject> PlayersInView = new List<GameObject>(5);

    #endregion Field of View variables

    private void Start()
    {
        m_viewMesh = new Mesh
        {
            name = "Mesh View"
        };
        m_viewMeshFilter.mesh = m_viewMesh;

        StartCoroutine(StartGuardBehavior());
    }

    //TODO add rotation behavoir (spawner timer iEnumarator)

    private void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, m_targetRotation, m_rotationSpeed * Time.deltaTime);
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    #region behavior

    private IEnumerator StartGuardBehavior()
    {
        yield return new WaitForSeconds(m_maxWaitTime.maxValue);

        yield return StartCoroutine(GuardRotating());
        StopCoroutine(StartGuardBehavior());
    }

    private IEnumerator GuardRotating()
    {
        yield return new WaitForSeconds(GetMinWaitTime());

        m_targetRotation = Quaternion.LookRotation(-transform.forward, Vector3.up);

        if (transform.rotation.eulerAngles == new Vector3() || transform.rotation.eulerAngles == new Vector3(0, 180, 0))
        {
            PlayersInView.Clear();

            StartCoroutine(FindTargetWithDelay(m_viewMeshRefreshRate));

            if (PlayersInView.Count > 0)
            {
                Debug.Log("in");//TODO its not getting here
                for (int i = 0; i < PlayersInView.Count; i++)
                {
                    transform.rotation = Quaternion.LookRotation(PlayersInView[i].gameObject.transform.position, transform.up);

                    GameObject go = new GameObject();
                    {
                        name = "test";
                        transform.position = Vector3.MoveTowards(transform.position, PlayersInView[i].transform.position, 5 * Time.deltaTime);
                    };
                    go.AddComponent<BoxCollider>();

                    Instantiate(go, transform.position, Quaternion.identity);
                    //look at and shoot player
                }
            }
            else
            {
                Debug.Log("out");

                yield return new WaitForSeconds(GetRandomWaitTime());

                yield return StartCoroutine(StartGuardBehavior());

                StopCoroutine(GuardRotating());
            }

            StopCoroutine(FindTargetWithDelay(m_viewMeshRefreshRate));
        }
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
}