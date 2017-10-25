using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    [CustomEditor(typeof(TriangulatorTest))]
    public class Test : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var targ = target as TriangulatorTest;
            if (GUILayout.Button("Triangulate"))
            {
                EarCuttingTriangulator triang = new EarCuttingTriangulator();;
                targ.Mesh = triang.CreateMesh(targ.TestContour);
            }
        }
    }

}

