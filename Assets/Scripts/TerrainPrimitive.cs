using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPrimitive : MonoBehaviour
{
    int[] newTriangles;
    Vector2[] newUv;
    // Start is called before the first frame update
    void Start()
    {
        BuildMesh();

    }

    // Update is called once per frame
    void Update()
    {

    }
    // void BuildMeshOld()
    // {
    //     var mesh = new Mesh();

    //     mesh.vertices = new Vector3[]
    //     {
    //         new Vector3(0, 0),
    //         new Vector3(0, 1),
    //         new Vector3(1, 1),
    //         new Vector3(1, 0)
    //     };

    //     // mesh.triangles = new int[] { 0, 1, 2, 3, 4, 5};
    //     mesh.triangles = newTriangles;
    //     GetComponent<MeshFilter>().mesh = mesh;
    // }

    void BuildMesh()
    {
        var mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        mesh.vertices = new Vector3[]
        {
            new Vector3(0, 0),
            new Vector3(0, 1),
            new Vector3(1, 1),
            new Vector3(1, 0)
        };
        mesh.uv = newUv;
        mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 }; ;
    }
}
