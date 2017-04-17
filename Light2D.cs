//ORG: Ghostyii & MoonLight Game
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class Light2D : MonoBehaviour
{
    public bool reverseDiretion = false;
    [Range(0, 360)]
    public float angle = 360f;
    public float range = 5;
    public Color color = Color.white;
    public LayerMask cullingMask = -1;

    private int mask = -1;
    
    private int segments = 50;
    private float distance = 0;

    private Mesh mesh;
    private Material material;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private Vector3[] vertexs;
    private int[] triangles;

    private void Start()
    {
        mask = 0 | cullingMask;

        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        material = new Material(Shader.Find("Sprites/Default"));
        material.SetColor("_Color", color);
        meshRenderer.sharedMaterial = material;

    }

    private void Update()
    {
        mask = 0 | cullingMask;
        range = Mathf.Clamp(range, 0, range);
        material.SetColor("_Color", color);

        segments = Mathf.RoundToInt(angle * 10);
        vertexs = new Vector3[segments + 1];
        vertexs[0] = transform.InverseTransformPoint(transform.localPosition);

        int count = 1;
        for (float i = -transform.localEulerAngles.z; i <= -transform.localEulerAngles.z + angle; i ++)
        {
            Vector2 direction = reverseDiretion ?
                new Vector2(Mathf.Cos(Mathf.Deg2Rad * i), Mathf.Sin(Mathf.Deg2Rad * i))
              : new Vector2(Mathf.Sin(Mathf.Deg2Rad * i), Mathf.Cos(Mathf.Deg2Rad * i));

            RaycastHit2D hit = Physics2D.Raycast(transform.localPosition, direction, range, mask);

            distance = hit.collider == null ? range : hit.distance;
            Vector2 endPoint = new Vector2(transform.localPosition.x + distance * direction.x / direction.magnitude, transform.localPosition.y + distance * direction.y / direction.magnitude);
            endPoint = transform.InverseTransformPoint(endPoint);

            if (count <= segments)
                vertexs[count++] = endPoint;
        }

        triangles = new int[segments * 3];
        for (int i = 0, vi = 1; i < segments * 3 - 3; i += 3, vi++)
        {
            triangles[i] = 0;
            triangles[i + 1] = vi;
            triangles[i + 2] = vi + 1;
        }
        if (segments != 0)
        {
            triangles[segments * 3 - 3] = 0;
            triangles[segments * 3 - 2] = segments;
            triangles[segments * 3 - 1] = 1;
        }

        mesh = new Mesh();
        mesh.vertices = vertexs;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.sharedMesh = mesh;
    }

    private void OnDisable()
    {
        meshFilter.sharedMesh = null;
    }

}

