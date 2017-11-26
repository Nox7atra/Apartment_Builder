using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Foxsys.ApartmentEditor
{
    [CustomEditor(typeof(MeshFilter))]
    public class MeshFilterCustomEditor : Editor
    {
        private Color _GizmoColor = Color.green;
        private float _Offset = 0.3f;
        private MeshFilter _TargetMeshFilter;

        private void OnEnable()
        {
            _TargetMeshFilter = target as MeshFilter;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            _Offset = EditorGUILayout.FloatField("Offsets", _Offset);
            _GizmoColor = EditorGUILayout.ColorField("Gizmo Color", _GizmoColor);

        }
        private void OnSceneGUI()
        {
            Mesh mesh = _TargetMeshFilter.sharedMesh;
            var transform = _TargetMeshFilter.transform;
            mesh.RecalculateBounds();
            Bounds bounds = mesh.bounds;
            Handles.color = _GizmoColor;

            var pos = transform.position;

            DrawSizes(
                pos,
                bounds.GetCorner(CornerType.RightUpForward),
                bounds.GetCorner(CornerType.RightDownForward),
                transform.lossyScale,
                Vector3.right, 
                transform.rotation
            );

            DrawSizes(
                pos,
                bounds.GetCorner(CornerType.LeftUpForward),
                bounds.GetCorner(CornerType.RightUpForward),
                transform.lossyScale,
                Vector3.up, 
                transform.rotation
            );


            DrawSizes(
                pos,
                bounds.GetCorner(CornerType.LeftUpForward),
                bounds.GetCorner(CornerType.LeftUpBackward),
                transform.lossyScale,
                Vector3.left,
                transform.rotation
            );
        }

        private void DrawSizes(Vector3 position, Vector3 corner1, Vector3 corner2, Vector3 scale,
            Vector3 normalDirection, Quaternion rotation)
        {
            Vector3 p1 = position + rotation *
                         (Vector3.Scale(corner1, scale)),
                p2 = position + rotation *
                     (Vector3.Scale(corner1, scale) + normalDirection * _Offset),
                p3 = position + rotation *
                     (Vector3.Scale(corner2, scale) + normalDirection * _Offset),
                p4 = position + rotation *
                     (Vector3.Scale(corner2, scale));

            Handles.DrawLine(p1, p2);
            Handles.DrawLine(p2, p3);
            GUIStyle style = new GUIStyle();
            style.normal.textColor = _GizmoColor;
            style.fontStyle = FontStyle.Bold;
            Handles.Label((p2 + p3) / 2 + normalDirection * _Offset / 2, Vector3.Distance(p2, p3).ToString() + " meter", style);
            Handles.DrawLine(p3, p4);
        }
       
    }
}