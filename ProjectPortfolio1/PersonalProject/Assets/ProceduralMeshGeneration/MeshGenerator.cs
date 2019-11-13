using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh _mesh;
    Vector3[] vertices;
    int[] triangles;

    Color[] colors;

    [SerializeField] float[] octaveFrequencies = new float[] { 7, 1.5f, 2, 8.5f };
    [SerializeField] float[] octaveAmplitudes = new float[] { 1, 2.9f, 0.7f, 5.1f };

    [SerializeField] private int xSize = 20;
    [SerializeField] private int zSize = 20;

    [SerializeField] private Gradient grandient;
    [SerializeField] private float minHeight;
         [SerializeField] private float maxHeight;

    // Start is called before the first frame update
    void Start()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;



    }

    private void Update()
    {
        CreateShape();
        UpdateMesh();
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;

        for (int index = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                if (octaveAmplitudes != null && octaveFrequencies != null)
                {
                    for (int i = 0; i < octaveFrequencies.Length; i++)
                    {
                        float y = octaveAmplitudes[i] * Mathf.PerlinNoise(octaveFrequencies[i] * x * 0.3f, octaveFrequencies[i] * z * 0.3f) * 2f;
                        vertices[index] = new Vector3(x, y, z);
                        if (y > maxHeight)
                        {
                            maxHeight = y;
                        }
                        if (y < minHeight)
                        {
                            minHeight = y;
                        }


                    }
                }
                else
                {
                    float y = Mathf.PerlinNoise(x * 0.3f, z * 0.3f) * 2f;
                    vertices[index] = new Vector3(x, y, z);
                    if (y > maxHeight)
                    {
                        maxHeight = y;
                    }
                    if (y < minHeight)
                    {
                        minHeight = y;
                    }

                }


                index++;

            }
        }


        for (int z = 0; z < zSize; z++)
        {

            for (int x = 0; x < xSize; x++)
            {



                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
                // yield return new WaitForSeconds(0.2f);

            }
            vert++;


        }


        colors = new Color[vertices.Length];


        for (int index = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height =Mathf.InverseLerp(minHeight,maxHeight, vertices[index].y);
                colors[index] = grandient.Evaluate(height);
                    index++;

            }
        }

    }

    private void UpdateMesh()
    {
        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.triangles = triangles;

        _mesh.colors =colors;
        _mesh.RecalculateNormals();
    }

    //private void OnDrawGizmos()
    //{
    //    if (vertices == null)
    //        return;

    //    for (int i = 0; i < vertices.Length; i++)
    //    {
    //        Gizmos.DrawSphere(vertices[i], 0.1f);
    //    }
    //}
}
