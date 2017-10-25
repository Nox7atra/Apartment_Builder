using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    [CustomEditor(typeof(Room))]
    public class RoomCustomInspector : Editor
    {
        private Room _ThisRoom;
        public void OnEnable()
        {
            _ThisRoom = (Room) target;
        }
        public override void OnInspectorGUI()
        {
            _ThisRoom.ContourColor = EditorGUILayout.ColorField(_ThisRoom.ContourColor);
            DrawContourPositions();
        }

        public void DrawContourPositions()
        {
            for (int i = 0; i < _ThisRoom.Walls.Count; i++)
            {
                _ThisRoom.Walls[i].End = EditorGUILayout.Vector2Field("Point " + i,
                    _ThisRoom.Walls[i].End);
                _ThisRoom.Walls[(i + 1) % _ThisRoom.Walls.Count].Begin = _ThisRoom.Walls[i].End;
            }
        }
    }
}