using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Nox7atra.ApartmentEditor
{
    public class Room
    {
        #region properties
        public Color ContourColor
        {
            get
            {
                return _ContourColor;
            }
        }
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
        private Color _ContourColor;
        private List<Vector2> _ContourPoints;
        #endregion

        #region public methods
        public void Draw(Grid grid, bool isClosed = true)
        {
            int count = isClosed ? _ContourPoints.Count : _ContourPoints.Count - 1;
            for (int i = 0; i < count; i++)
            {
                var p1 = grid.GridToGUI(_ContourPoints[i]);
                var p2 = grid.GridToGUI(_ContourPoints[(i + 1) % _ContourPoints.Count]);

                Handles.color = _ContourColor;
                Handles.DrawLine(p1, p2);
             
                Handles.color = Color.white;
                Handles.DrawWireDisc(p1, Vector3.back, SNAPING_RAD);
                if (ApartmentConfigWindow.Config.IsDrawSizes)
                {
                    Handles.color = Color.white;
                    Handles.Label((p1 + p2) / 2, 
                        Vector2.Distance(
                            _ContourPoints[i], 
                            _ContourPoints[(i + 1) % _ContourPoints.Count]).ToString());
                }
                if (ApartmentConfigWindow.Config.IsDrawPositions)
                {
                    Handles.color = Color.white;
                    Handles.Label(p1, _ContourPoints[i].ToString());
                }
            }
        }
        public void Move(Vector2 dv)
        {
            for(int i = 0; i < _ContourPoints.Count; i++)
            {
                _ContourPoints[i] += dv;
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
            _ContourColor = new Color(Random.Range(0.5f, 1), Random.Range(0.5f, 1), Random.Range(0.5f, 1));
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