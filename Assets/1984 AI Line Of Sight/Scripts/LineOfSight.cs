using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class LineOfSight : MonoBehaviour
{
    public float viewRadius;
    [Range(0, 360)] public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public List<Transform> visibleTargets = new List<Transform>();
    public Vector3 furtherFrontier;

    public float meshResolution;

    private Mesh _viewMesh;
    private MeshRenderer _meshR;

    public bool Use2DPhysics;
    [SerializeField] GameObject[] destroyWithSelfObj;
    void Start()
    {
        _viewMesh = new Mesh {name = "View Mesh"};
        GetComponent<MeshFilter>().mesh = _viewMesh;
        _meshR = GetComponent<MeshRenderer>();
        StartCoroutine(FindTargetsWithDelay(.2f));
        DFOVInit();
    }

    public void SetMaterial(Material m)
    {
        _meshR.material = m;
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }
    private void OnDisable()
    {
        foreach (var asd in destroyWithSelfObj)
        {
            Destroy(asd);
        }
    }
    void LateUpdate()
    {
        DrawFieldOfView();
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        if (!Use2DPhysics)
        {
            Collider[] targetsInViewRadius = new Collider[4];
            var size = Physics.OverlapSphereNonAlloc(transform.position, viewRadius, targetsInViewRadius, targetMask);
            for (int i = 0; i < size; i++)
            {
                Transform target = targetsInViewRadius[i].transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    float dstToTarget = Vector3.Distance(transform.position, target.position);
                    if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                    {
                        if (target.gameObject.GetComponent<AutoAttack>().isVisible)
                        {
                            visibleTargets.Add(target);
                        }
       
                    }
                }
            }
        }
        /*
        else
        {
            Collider2D[] targetsInViewRadius = new Collider2D[5];
            var size = Physics2D.OverlapCircleNonAlloc(transform.position, viewRadius, targetsInViewRadius, targetMask);
            for (int i = 0; i < size; i++)
            {
                Transform target = targetsInViewRadius[i].transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    float dstToTarget = Vector3.Distance(transform.position, target.position);
                    if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                    {
                        visibleTargets.Add(target);
                    }
                }
            }
        }
        */
    }
    public void AddTarget(Transform transform)
    {
        if(!visibleTargets.Contains(transform))
        visibleTargets.Add(transform);
    }

    int vertexCount;
    Vector3[] vertices;
    int[] triangles;
    List<Vector3> viewPoints = new List<Vector3>();
    float stepAngleSize;
    int stepCount;
    float angle;
    float maxDist;
    ViewCastInfo newViewCast;

    void DFOVInit()
    {
        stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        stepAngleSize = viewAngle / stepCount;

        vertexCount = stepCount + 1;
        vertices = new Vector3[vertexCount];
        triangles = new int[(vertexCount - 2) * 3];

    }

    private void FixedUpdate()
    {
        viewPoints.Clear();
        maxDist = -1;
        for (int i = 0; i <= stepCount; i++)
        {
            angle = -viewAngle * 0.5f + stepAngleSize * i;
            newViewCast = ViewCast(angle);
            if (newViewCast.dst > maxDist)
            {
                maxDist = newViewCast.dst;
                furtherFrontier = newViewCast.point;
            }

            viewPoints.Add(newViewCast.point);
        }
    }

    void DrawFieldOfView()
    {

        if (viewPoints.Count != vertexCount) return;

        vertices[0] = Vector3.zero;

        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewPoints.Clear();
        _viewMesh.Clear();
        _viewMesh.vertices = vertices;
        _viewMesh.triangles = triangles;
        _viewMesh.RecalculateNormals();
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle);
        dir = transform.rotation * dir;
        if (!Use2DPhysics)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
            {
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
            }
            else
            {
                return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
            }
        }else
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewRadius, obstacleMask);
            if (hit)
            {
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
            }
            else
            {
                return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees)
    {
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
}