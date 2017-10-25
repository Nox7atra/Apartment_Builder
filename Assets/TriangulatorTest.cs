using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TriangulatorTest : MonoBehaviour
{

    public List<Vector2> TestContour;
    public Mesh Mesh;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        for (int i = 0; i < TestContour.Count; i++)
        {
            Gizmos.DrawLine(TestContour[i], TestContour[(i+1) % TestContour.Count]);
            Handles.Label(TestContour[i], i.ToString());
        }
        Gizmos.color = Color.red;
        if (Mesh != null)
        {
            Gizmos.DrawWireMesh(Mesh);
        }

    }
}

