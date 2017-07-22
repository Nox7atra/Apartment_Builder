using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    public class Apartment
    {
        #region properties
        public float Square
        {
            get
            {
                float result = 0;
                for (int i = 0; i < _Rooms.Count; i++)
                {
                    result += _Rooms[i].Square;
                }
                return result;
            }
        }
        #endregion
        #region attributes
        private List<Room> _Rooms;
        #endregion
        #region public methods
        public void Draw(Grid grid)
        {
            for(int i = 0; i < _Rooms.Count; i++)
            {
                _Rooms[i].Draw(grid);
            }
        }
        public void AddRoom(Room room)
        {
            _Rooms.Add(room);
        }
        #endregion
        #region constructor
        public Apartment()
        {
            _Rooms = new List<Room>();
        }
        #endregion
    }
    public class Room
    {
        #region properties
        public float Square
        {
            get
            {
                float result = 0;
                int pointsCount = _ContourPoints.Count;
                for (int i = 0; i < pointsCount; i++)
                {
                    result +=
                        0.5f * (_ContourPoints[i].x + _ContourPoints[(i + 1) % pointsCount].x)
                             * (_ContourPoints[i].y - _ContourPoints[(i + 1) % pointsCount].y);
                }
                return result;
            }
        }
        #endregion
        #region attributes
        private Type _Type;
        private List<Vector2> _ContourPoints;
        #endregion
        #region public methods
        public void Draw(Grid grid)
        {
            for(int i = 0; i < _ContourPoints.Count; i++)
            {
                Handles.color = Color.cyan;
                Handles.DrawLine(
                     grid.GUIToGrid(_ContourPoints[i]),
                     grid.GUIToGrid(_ContourPoints[(i + 1) % _ContourPoints.Count]));
            }
        }
        public void AddPoint(Vector2 point)
        {
            _ContourPoints.Add(point);
        }
        #endregion
        #region constructor
        public Room()
        {
            _ContourPoints = new List<Vector2>();
        }
        #endregion
        #region nested types
        public enum Type
        {
            Kitchen,
            Bathroom,
            Toilet,
            BathroomAndToilet
        }
        #endregion
    }

}
