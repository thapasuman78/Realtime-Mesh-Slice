using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public struct Tri
{
    public Vector3[] vertices;

    public Tri(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        vertices = new Vector3[3] { v1, v2, v3 };
    }
}

public struct VertexData
{
    public List<Vector3> position;
    public List<Vector3> normal;
    public List<Vector2> uv;
}

public class MeshData
{
    public VertexData vertices;
    public List<int> tris;
    public MeshData()
    {
        vertices = new VertexData { position = new List<Vector3>(), normal = new List<Vector3>(), uv = new List<Vector2>() };
        tris = new List<int>();
    }
}
public class MeshSlice : MonoBehaviour
{
    public Transform Geometry;
    public Transform Slicer;
    public MeshFilter filter;
    public LayerMask GeoMask;
    public Material SlicedMat;
    public PhysicMaterial physicsMat;
    public Transform[] AllGeometry;

    private Mesh mesh;
    private Renderer rend;
    private Vector3 normal;

    private List<Vector3> intersectionPoints = new List<Vector3>();
    private List<Vector3> Source = new List<Vector3>();
    private List<Vector3> Target = new List<Vector3>();
    private List<Tri> triList = new List<Tri>();

    MeshData meshUpper = new MeshData();
    MeshData meshLower = new MeshData();

    private Material mU;
    private Material mL;
    private int index;
    private bool isMouseDown;

    void Start()
    {
        rend = Slicer.GetComponent<Renderer>();
        mesh = filter.sharedMesh;

        //FindIntersectionPoints();
    }

    public void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            isMouseDown = true;
            Slicer.gameObject.SetActive(true);
            Slicer.transform.position = AllGeometry[index].transform.position;
            Initialize(AllGeometry[index], AllGeometry[index].GetComponent<MeshFilter>());
        }
        else if(Input.GetMouseButtonUp(0) && isMouseDown)
        {
            isMouseDown = false;
            Slicer.gameObject.SetActive(false);
            FindIntersectionPoints();
            index++;
        }
    }

    public void Initialize(Transform toCut, MeshFilter toCutFilter)
    {
        Geometry = toCut;
        filter = toCutFilter;
        mesh = toCutFilter.sharedMesh;
    }

    //public void Update()
    //{
    //    intersectionPoints.Clear();
    //    triList.Clear();
    //    Source.Clear();
    //    Target.Clear();
    //    FindIntersectionPoints();
    //}

    public void FindIntersectionPoints()
    {
        intersectionPoints.Clear();
        Source.Clear();
        Target.Clear();
        meshUpper = null;
        meshLower = null;

        Vector3 pos_wrt_Geom = Geometry.InverseTransformPoint(Slicer.position);
        normal = Geometry.InverseTransformDirection(Slicer.up.normalized);

        meshUpper = new MeshData();
        meshLower = new MeshData();

        float d_GP = pos_wrt_Geom.magnitude;
        //Debug.Log(pos_wrt_Geom);
        int length = mesh.triangles.Length / 3;
        for (int i = 0; i < length; i++)
        {
            int index = i * 3;

            Vector3 v0 = mesh.vertices[mesh.triangles[index]];
            Vector3 v1 = mesh.vertices[mesh.triangles[index + 1]];
            Vector3 v2 = mesh.vertices[mesh.triangles[index + 2]];

            Vector2 u0 = mesh.uv[mesh.triangles[index]];
            Vector2 u1 = mesh.uv[mesh.triangles[index + 1]];
            Vector2 u2 = mesh.uv[mesh.triangles[index + 2]];

            Vector3 n_0 = mesh.normals[mesh.triangles[index]];
            Vector3 n_1 = mesh.normals[mesh.triangles[index + 1]];
            Vector3 n_2 = mesh.normals[mesh.triangles[index + 2]];

            float d_AP = Vector3.Dot((v0 - pos_wrt_Geom).normalized, normal);
            float d_BP = Vector3.Dot((v1 - pos_wrt_Geom).normalized, normal);
            float d_CP = Vector3.Dot((v2 - pos_wrt_Geom).normalized, normal);



            //if (d_AP * d_BP < 0)
            //{
            //    Debug.Log(1);
            //    SliceTriangle(mesh.vertices[mesh.triangles[index]], mesh.vertices[mesh.triangles[index + 1]], mesh.vertices[mesh.triangles[index + 2]], d_AP, d_BP, d_CP);
            //}
            //else if (d_BP * d_CP < 0)
            //{
            //    Debug.Log(2);
            //    SliceTriangle(mesh.vertices[mesh.triangles[index + 1]], mesh.vertices[mesh.triangles[index + 2]], mesh.vertices[mesh.triangles[index]], d_BP, d_CP, d_AP);
            //}
            //else if (d_CP * d_AP < 0)
            //{
            //    Debug.Log(3);
            //    SliceTriangle(mesh.vertices[mesh.triangles[index + 2]], mesh.vertices[mesh.triangles[index]], mesh.vertices[mesh.triangles[index + 1]], d_CP, d_AP, d_BP);
            //}


            //if (d_AP * d_BP < 0 && d_AP * d_CP < 0)
            //{
            //    Debug.Log(1);
            //    intersectionPoints.Add(GetIntersectionPoint(mesh.vertices[mesh.triangles[index]], mesh.vertices[mesh.triangles[index + 1]]));
            //    intersectionPoints.Add(GetIntersectionPoint(mesh.vertices[mesh.triangles[index]], mesh.vertices[mesh.triangles[index + 2]]));
            //}
            //else if (d_BP * d_CP < 0 && d_BP * d_AP < 0)
            //{
            //    Debug.Log(2);
            //    intersectionPoints.Add(GetIntersectionPoint(mesh.vertices[mesh.triangles[index + 1]], mesh.vertices[mesh.triangles[index + 2]]));
            //    intersectionPoints.Add(GetIntersectionPoint(mesh.vertices[mesh.triangles[index + 1]], mesh.vertices[mesh.triangles[index]]));
            //}
            //else if (d_CP * d_AP < 0 && d_CP * d_BP < 0)
            //{
            //    Debug.Log(3);
            //    intersectionPoints.Add(GetIntersectionPoint(mesh.vertices[mesh.triangles[index + 2]], mesh.vertices[mesh.triangles[index]]));
            //    intersectionPoints.Add(GetIntersectionPoint(mesh.vertices[mesh.triangles[index + 2]], mesh.vertices[mesh.triangles[index + 1]]));
            //}

            int s = meshLower.vertices.position.Count;
            int f = meshUpper.vertices.position.Count;

            if (d_AP * d_BP < 0 && d_AP * d_CP < 0)
            {
                Debug.Log(1);
                ManualIntersection(v0, v1, pos_wrt_Geom, normal, u0, u1, n_0, n_1, out Vector3 p1, out Vector2 uv1, out Vector3 n1);
                ManualIntersection(v0, v2, pos_wrt_Geom, normal, u0, u2, n_0, n_2, out Vector3 p2, out Vector2 uv2, out Vector3 n2);
                intersectionPoints.Add(p1);
                intersectionPoints.Add(p2);

                triList.Add(new Tri(v0, p1, p2));
                triList.Add(new Tri(p2, p1, v1));
                triList.Add(new Tri(p2, v1, v2));

                if (d_AP < 0)
                {
                    AddVertices(meshLower.vertices, new Vector3[3] { v0, p1, p2 }, new Vector3[3] { n_0, n1, n2 }, new Vector2[3] { u0, uv1, uv2 });

                    AddVertices(meshUpper.vertices, new Vector3[4] { p2, p1, v1, v2 }, new Vector3[4] { n2, n1, n_1, n_2 }, new Vector2[4] { uv2, uv1, u1, u2 });

                    MakeTriangle(meshLower.tris, s, false);

                    MakeTriangle(meshUpper.tris, f, true);
                }
                else
                {
                    AddVertices(meshUpper.vertices, new Vector3[3] { v0, p1, p2 }, new Vector3[3] { n_0, n1, n2 }, new Vector2[3] { u0, uv1, uv2 });

                    AddVertices(meshLower.vertices, new Vector3[4] { p2, p1, v1, v2 }, new Vector3[4] { n2, n1, n_1, n_2 }, new Vector2[4] { uv2, uv1, u1, u2 });

                    MakeTriangle(meshUpper.tris, f, false);
                    MakeTriangle(meshLower.tris, s, true);
                }
            }
            else if (d_BP * d_CP < 0 && d_BP * d_AP < 0)
            {
                Debug.Log(2);

                ManualIntersection(v1, v2, pos_wrt_Geom, normal, u1, u2, n_1, n_2, out Vector3 p1, out Vector2 uv1, out Vector3 n1);
                ManualIntersection(v1, v0, pos_wrt_Geom, normal, u1, u0, n_1, n_0, out Vector3 p2, out Vector2 uv2, out Vector3 n2);
                intersectionPoints.Add(p1);
                intersectionPoints.Add(p2);


                triList.Add(new Tri(v1, p1, p2));
                triList.Add(new Tri(p2, p1, v2));
                triList.Add(new Tri(p2, v2, v0));


                if (d_BP < 0)
                {
                    AddVertices(meshLower.vertices, new Vector3[3] { v1, p1, p2 }, new Vector3[3] { n_1, n1, n2 }, new Vector2[3] { u1, uv1, uv2 });

                    AddVertices(meshUpper.vertices, new Vector3[4] { p2, p1, v2, v0 }, new Vector3[4] { n2, n1, n_2, n_0 }, new Vector2[4] { uv2, uv1, u2, u0 });

                    MakeTriangle(meshLower.tris, s, false);

                    MakeTriangle(meshUpper.tris, f, true);

                }
                else
                {
                    AddVertices(meshUpper.vertices, new Vector3[3] { v1, p1, p2 }, new Vector3[3] { n_1, n1, n2 }, new Vector2[3] { u1, uv1, uv2 });

                    AddVertices(meshLower.vertices, new Vector3[4] { p2, p1, v2, v0 }, new Vector3[4] { n2, n1, n_2, n_0 }, new Vector2[4] { uv2, uv1, u2, u0 });

                    MakeTriangle(meshUpper.tris, f, false);

                    MakeTriangle(meshLower.tris, s, true);
                }
            }
            else if (d_CP * d_AP < 0 && d_CP * d_BP < 0)
            {
                Debug.Log(3);

                ManualIntersection(v2, v0, pos_wrt_Geom, normal, u2, u0, n_2, n_0, out Vector3 p1, out Vector2 uv1, out Vector3 n1);
                ManualIntersection(v2, v1, pos_wrt_Geom, normal, u2, u1, n_2, n_1, out Vector3 p2, out Vector2 uv2, out Vector3 n2);
                intersectionPoints.Add(p1);
                intersectionPoints.Add(p2);

                triList.Add(new Tri(v2, p1, p2));
                triList.Add(new Tri(p2, p1, v0));
                triList.Add(new Tri(p2, v0, v1));

                if (d_CP < 0)
                {

                    AddVertices(meshLower.vertices, new Vector3[3] { v2, p1, p2 }, new Vector3[3] { n_2, n1, n2 }, new Vector2[3] { u2, uv1, uv2 });

                    AddVertices(meshUpper.vertices, new Vector3[4] { p2, p1, v0, v1 }, new Vector3[4] { n2, n1, n_0, n_1 }, new Vector2[4] { uv2, uv1, u0, u1 });

                    MakeTriangle(meshLower.tris, s, false);

                    MakeTriangle(meshUpper.tris, f, true);
                }
                else
                {

                    AddVertices(meshUpper.vertices, new Vector3[3] { v2, p1, p2 }, new Vector3[3] { n_2, n1, n2 }, new Vector2[3] { u2, uv1, uv2 });

                    AddVertices(meshLower.vertices, new Vector3[4] { p2, p1, v0, v1 }, new Vector3[4] { n2, n1, n_0, n_1 }, new Vector2[4] { uv2, uv1, u0, u1 });

                    MakeTriangle(meshUpper.tris, f, false);

                    MakeTriangle(meshLower.tris, s, true);
                }
            }

            else
            {
                if (d_AP < 0)
                {
                    AddVertices(meshLower.vertices, new Vector3[3] { v0, v1, v2 }, new Vector3[3] { n_0, n_1, n_2 }, new Vector2[3] { u0, u1, u2 });

                    MakeTriangle(meshLower.tris, s, false);

                }
                else
                {
                    AddVertices(meshUpper.vertices, new Vector3[3] { v0, v1, v2 }, new Vector3[3] { n_0, n_1, n_2 }, new Vector2[3] { u0, u1, u2 });
                    MakeTriangle(meshUpper.tris, f, false);
                }
            }
        }
        Geometry.gameObject.SetActive(false);

        MakeMesh();

    }

    public void MakeMesh()
    {
        Mesh mesh1 = new Mesh { vertices = meshUpper.vertices.position.ToArray(), triangles = meshUpper.tris.ToArray(), uv = meshUpper.vertices.uv.ToArray(), normals = meshUpper.vertices.normal.ToArray() };
        Mesh mesh2 = new Mesh { vertices = meshLower.vertices.position.ToArray(), triangles = meshLower.tris.ToArray(), uv = meshLower.vertices.uv.ToArray(), normals = meshLower.vertices.normal.ToArray() };

        CreateGameObject(mesh1);
        CreateGameObject(mesh2);
    }

    public void CreateGameObject(Mesh mesh)
    {
        GameObject g = new GameObject("G1");
        MeshFilter mf = g.AddComponent<MeshFilter>();
        MeshRenderer mr = g.AddComponent<MeshRenderer>();
        MeshCollider mc = g.AddComponent<MeshCollider>();
        g.transform.position = Geometry.transform.position;
        g.transform.localScale = Geometry.transform.localScale;
        g.layer = Geometry.gameObject.layer;
        mf.mesh = mesh;
        mr.material = new Material(SlicedMat);
        mc.material = physicsMat;
        mc.sharedMesh = mesh;
        mc.convex = true;

        Rigidbody rb = g.AddComponent<Rigidbody>();

        rb.interpolation = RigidbodyInterpolation.Interpolate;

        rb.AddExplosionForce(5, g.transform.position, 10, 0, ForceMode.Impulse);

        mr.material.DOFloat(-2f, "_Diss", 2f).SetDelay(1.5f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            g.gameObject.SetActive(false);
        });
    }

    public void AddVertices(VertexData vertData, Vector3[] vertices, Vector3[] normal, Vector2[] uv)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            vertData.position.Add(vertices[i]);
            vertData.normal.Add(normal[i]);
            vertData.uv.Add(uv[i]);
        }
    }

    public void MakeTriangle(List<int> triArray, int index, bool isTwoTri)
    {
        triArray.Add(index);
        triArray.Add(index + 1);
        triArray.Add(index + 2);

        if (!isTwoTri) return;

        triArray.Add(index);
        triArray.Add(index + 2);
        triArray.Add(index + 3);
    }

    public Vector3 GetIntersectionPoint(Vector3 s, Vector3 t)
    {
        //Debug.Log("S : " + s);
        //Debug.Log("T : " + t);
        Vector3 s1 = Geometry.TransformPoint(s);
        Vector3 t1 = Geometry.TransformPoint(t);
        Source.Add(s1);
        Target.Add(t1);
        Physics.Raycast(s1, t1 - s1, out RaycastHit hitinfo, (s - t).magnitude, GeoMask);
        //Debug.Log(hitinfo.point);
        return hitinfo.point;
    }

    public void ManualIntersection(Vector3 s, Vector3 t, Vector3 plane_origin, Vector3 normal, Vector2 uv1, Vector2 uv2, Vector3 n1, Vector3 n2, out Vector3 p, out Vector2 uv, out Vector3 n)
    {
        Vector3 direction = (t - s).normalized;
        Vector3 po_s = plane_origin - s;
        float distance = Vector3.Dot(po_s.normalized, normal) / Vector3.Dot(direction, normal);
        //Debug.Log(distance);

        Vector3 s1 = Geometry.TransformPoint(s);
        Vector3 t1 = Geometry.TransformPoint(t);
        Source.Add(s1);
        Target.Add(t1);

        p = s + distance * po_s.magnitude * direction;

        float ratio = (p - s).magnitude / (t - s).magnitude;

        uv = Vector2.Lerp(uv1, uv2, ratio);
        n = Vector3.Lerp(n1, n2, ratio);
    }

    void SliceTriangle(Vector3 v0, Vector3 v1, Vector3 v2, float d1, float d2, float d3)
    {
        Vector3 p1 = v0 + (d1 / (d1 - d2)) * (v1 - v0).normalized;
        intersectionPoints.Add(p1);
        if (d1 < 0)
        {
            if (d3 < 0)
            {
                Vector3 p2 = v1 + (d2 / (d2 - d3)) * (v2 - v1).normalized;
                intersectionPoints.Add(p2);
            }
            else
            {
                Vector3 p3 = v0 + (d1 / (d1 - d3)) * (v2 - v0).normalized;
                intersectionPoints.Add(p3);
            }
        }
        else
        {
            if (d3 < 0)
            {
                Vector3 p2 = v0 + (d1 / (d1 - d3)) * (v2 - v0).normalized;
                intersectionPoints.Add(p2);
            }
            else
            {
                Vector3 p3 = v1 + (d2 / (d2 - d3)) * (v2 - v1).normalized;
                intersectionPoints.Add(p3);
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    if (intersectionPoints.Count > 0)
    //    {
    //        Gizmos.color = Color.red;
    //        for (int i = 0; i < intersectionPoints.Count; i++)
    //        {
    //            Gizmos.DrawSphere(intersectionPoints[i], 0.01f);
    //            Gizmos.DrawRay(Source[i], Target[i] - Source[i]);
    //        }

    //        Gizmos.color = Color.green;
    //        for (int i = 0; i < triList.Count; i++)
    //        {
    //            Gizmos.DrawLine(triList[i].vertices[0], triList[i].vertices[1]);
    //            Gizmos.DrawLine(triList[i].vertices[1], triList[i].vertices[2]);
    //            Gizmos.DrawLine(triList[i].vertices[2], triList[i].vertices[0]);
    //        }
    //    }
    //}
}
