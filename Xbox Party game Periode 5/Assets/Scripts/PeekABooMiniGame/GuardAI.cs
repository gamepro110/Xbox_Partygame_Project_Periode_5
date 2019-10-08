using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardAI : MonoBehaviour
{
    private float test;

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

        StartCoroutine(FindTargetWithDelay(m_viewMeshRefreshRate));
    }

    private void Update()
    {
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }

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

                    //if (target.GetComponentInParent<Player>())
                    //{
                    //    playerInSight = true;
                    //    //do things here
                    //}
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