using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Nox7atra.ApartmentEditor
{
    public class Room
    {
        #region properties
        public List<Vector2> Contour
        {
            get
            {
                return _ContourPoints;
            }
        }
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
        public void Draw(Grid grid, bool isClosed = true)
        {
            int count = isClosed ? _ContourPoints.Count : _ContourPoints.Count - 1;
            for (int i = 0; i < count; i++)
            {
                Handles.color = Color.cyan;
                Handles.DrawLine(grid.GridToGUI(_ContourPoints[i]),
                     grid.GridToGUI(_ContourPoints[(i + 1) % _ContourPoints.Count]));
                Handles.color = Color.yellow;
                Handles.DrawWireDisc(grid.GridToGUI(_ContourPoints[i]), Vector3.back, SNAPING_RAD);
            }
        }
        public bool IsLastPoint(Vector2 point)
        {
            return Vector2.Distance(point, _ContourPoints[0]) < SNAPING_RAD;
        }
        #endregion

        #region constructor
        public Room()
        {
            _ContourPoints = new List<Vector2>();
        }
        #endregion
        #region constants 
        public const float SNAPING_RAD = 10f;
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