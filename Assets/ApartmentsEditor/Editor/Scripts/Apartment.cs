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

        public Texture PlanImage;

        [SerializeField] private List<Room> _Rooms;
        [SerializeField] private Rect _Dimensions;

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

        public void Draw(ApartmentEditorGrid grid)
        {
            DrawDimensions(grid);

            if(PlanImage != null)
                DrawPlanImage();
               
            foreach (Room room in _Rooms)
            {
                room.Draw(grid);
               
            }
            DrawWallObjects(grid);
        }

        private void DrawPlanImage()
        {
            WindowObjectDrawer.DrawTexture(_Dimensions, PlanImage);
        }
        private void DrawWallObjects(ApartmentEditorGrid grid)
        {
            foreach (Room room in _Rooms)
            {
                var wallObjects = room.WallObjects;
                if (wallObjects.Count > 0)
                {
                    for (int i = 0, count = room.WallObjects.Count; i < count; i++)
                    {
                        wallObjects[i].Object.Draw(wallObjects[i].GetVector2Position());
                        if (room.IsShowPositions)
                        {
                            var position = wallObjects[i].GetVector2Position();
                            WindowObjectDrawer.DrawLabel(position, position.RoundCoordsToInt().ToString());
                        }
                    }
                }
            }
        }
        private void DrawDimensions(ApartmentEditorGrid grid)
        {
            Handles.color = SkinManager.Instance.CurrentSkin.RoomDimensionsColor;
            for(int i = 0; i < _DimensionsPoints.Length; i++)
                WindowObjectDrawer.DrawLine(_DimensionsPoints[i],_DimensionsPoints[(i + 1) % _DimensionsPoints.Length]);
        }

        public RoomVert GetVertInPos(Vector2 point, Room excludeRoom = null)
        {
            RoomVert vert = null;
            foreach (var room in _Rooms)
            {
                if(room == excludeRoom)
                    continue;

                vert = room.GetVertInPos(point);
            }
            return vert;
        }
        public ISelectable GetSelectableInPos(ApartmentEditorGrid grid, Vector2 position, out Room inRoom)
        {
            ISelectable selectable = null;
            inRoom = null;

            var rad = SkinManager.Instance.CurrentSkin.CirclesRad;
            foreach (var room in _Rooms)
            { 
                foreach (var wallObject in room.WallObjects)
                {
                    if (!(Vector2.Distance(wallObject.GetVector2Position(), position) <
                          rad)) continue;
                    inRoom = room;
                    return wallObject;
                }
                foreach (var roomVert in room.Contour)
                {
                    if (!(Vector2.Distance(roomVert.Position, position) < rad)) continue;
                    inRoom = room;
                    selectable = roomVert;
                }
            }
            return selectable;
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

        public Room GetNearestRoom(Vector2 point)
        {
            Room nearestRoom = null;
            var minDistance = float.MaxValue;
            for (int i = 0, count = _Rooms.Count; i < count; i++)
            {
                var room = _Rooms[i];
                var projection = room.GetNearestPointOnContour(point);

                if (!projection.HasValue)
                    continue;
                
                var distance = Vector2.Distance(point, projection.Value);
                if (minDistance > distance)
                {
                    nearestRoom = room;
                    minDistance = distance;
                }
            }
            return nearestRoom;
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
