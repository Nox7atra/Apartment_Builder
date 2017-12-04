using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Foxsys.ApartmentEditor
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
            DrawRoomTypeField();
            _ThisRoom.WallThickness = EditorGUILayout.FloatField("WallThikness (cm)", _ThisRoom.WallThickness);
            DrawContourPositions();
            _ThisRoom.IsShowPositions = EditorGUILayout.ToggleLeft("Show Positions", _ThisRoom.IsShowPositions);
            _ThisRoom.IsShowSizes = EditorGUILayout.ToggleLeft("Show Sizes", _ThisRoom.IsShowSizes);
            _ThisRoom.IsShowSquare = EditorGUILayout.ToggleLeft("Show Square", _ThisRoom.IsShowSquare);
        }
        private void DrawRoomTypeField()
        {
            var prevType = _ThisRoom.CurrentType;
            _ThisRoom.CurrentType = (Room.Type)EditorGUILayout.EnumPopup("Room type", _ThisRoom.CurrentType);

            if (prevType != _ThisRoom.CurrentType)
            {
                _ThisRoom.name = _ThisRoom.ParentApartment.GetRoomName(_ThisRoom.CurrentType);
                AssetDatabase.SaveAssets();
            }
        }
        private void DrawContourPositions()
        {
            var dimension = _ThisRoom.ParentApartment.Dimensions;
            var contour = _ThisRoom.Contour;
            for (int i = 0, count = contour.Count; i < count; i++)
            {
                var newPosition = EditorGUILayout.Vector2Field("Point " + i,
                    contour[i].Position);

                if (dimension.Contains(newPosition))
                {
                    contour[i].MoveTo(newPosition);
                }

            }
        }
    }
}