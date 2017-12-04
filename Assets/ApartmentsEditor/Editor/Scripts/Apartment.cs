using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace Foxsys.ApartmentEditor
{
    public class Apartment : ScriptableObject
    {
        #region fields

        public float Height;

        public List<RoomMaterials> RoomsMaterialses;

        public bool IsGenerateOutside;

        [SerializeField]
        private List<Room> _Rooms;
        [SerializeField]
        private Rect _Dimensions;

        private Vector2[] _DimensionsPoints = new Vector2[4];

        #endregion

        #region properties
        public List<Room> Rooms
        {
            get
            {
                return _Rooms;
            }
        }

        public float Square
        {
            get
            {
                float result = 0;
                foreach (Room room in _Rooms)
                {
                    result += room.Square;
                }
                return result;
            }
        }

        public Rect Dimensions
        {
            get { return _Dimensions; }
            set
            {
                _Dimensions = value;
                _DimensionsPoints[0] = new Vector3(Dimensions.width / 2, Dimensions.height / 2);
                _DimensionsPoints[1] = new Vector3(-Dimensions.width / 2, Dimensions.height / 2);
                _DimensionsPoints[2] = new Vector3(-Dimensions.width / 2, -Dimensions.height / 2);
                _DimensionsPoints[3] = new Vector3(Dimensions.width / 2, -Dimensions.height / 2);
            }
        }

        #endregion

        public void Draw(Grid grid)
        {
            DrawDimensions(grid);
            foreach (Room room in _Rooms)
            {
                room.Draw(grid);
            }
        }
        private void DrawDimensions(Grid grid)
        {
            Handles.color = SkinManager.Instance.CurrentSkin.RoomDimensionsColor;
            for(int i = 0; i < _DimensionsPoints.Length; i++)
                Handles.DrawLine(
                    grid.GridToGUI((_DimensionsPoints[i])),
                    grid.GridToGUI(_DimensionsPoints[(i + 1) % _DimensionsPoints.Length]));
        }

        public List<Wall> GetWallsWithPoint(Vector2 point)
        {
            List<Wall> result = new List<Wall>();
            foreach (var room in Rooms)
            {
                var contour = room.Contour;
                for (int i = 0, count = contour.Count; i < count; i++)
                {
                    RoomVert vert1 = contour[i],
                        vert2 = contour[(i + 1) % count];
                    var wall = new Wall(vert1, vert2);
                    if (wall.IsPointOnWall(point))
                        result.Add(wall);
                }
            }
            return result;
        }

        public RoomVert GetVertInPos(Vector2 position, out Room inRoom)
        {
            foreach (var room in _Rooms)
            {
                foreach (var roomVert in room.Contour)
                {
                    if (!(Vector2.Distance(roomVert.Position, position) < Room.SnapingRad)) continue;

                    inRoom = room;
                    return roomVert;
                }
            }
            inRoom = null;
            return null;
        }
        public Room GetRoomWithInsidePoint(Vector2 point, Room excludeRoom = null)
        {
            foreach (var room in _Rooms)
            {
                if(excludeRoom != room && room.IsPointInside(point))
                {
                    return room;
                }
            }
            return null;
        }

        public bool IsApartmentInRect(Rect rect)
        {
            foreach (var room in _Rooms)
            {
                if (!room.IsInsideRect(rect))
                    return false;
            }
            return true;
        }

        public string GetRoomName(Room.Type type)
        {
            var typeName = type.ToString();
            var room = _Rooms.FindLast(x => x.name.Split('_')[0] == typeName);
            var num = room == null ? 0 : (int.Parse(room.name.Split('_')[1]) + 1);
            return typeName + "_" + num;
        }
        private Apartment()
        {
            _Rooms = new List<Room>();
            RoomsMaterialses = new List<RoomMaterials>(4);
            for (int i = 0; i < 4; i++)
            {
                RoomsMaterialses.Add(new RoomMaterials());
            }
        }

        [Serializable]
        public class RoomMaterials
        {
            public Material FloorMat;
            public Material RoofMat;
            public Material WallMat;
        }
    }
}
