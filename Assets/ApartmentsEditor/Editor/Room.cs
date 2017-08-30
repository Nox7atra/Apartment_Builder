using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    public class Room
    {
        public const float SNAPING_RAD = 10f;

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
                        0.5f * (_ContourPoints[i].x * _ContourPoints[(i + 1) % pointsCount].y 
                        - _ContourPoints[i].y *_ContourPoints[(i + 1) % pointsCount].x);
                }
                return Mathf.Abs(result);
            }
        }
        public Vector2 Centroid
        {
            get
            {
                Vector2 centroid = new Vector2();
                float signedArea = 0;
                for(int i = 0; i < _ContourPoints.Count; i++)
                {
                    float x0 = _ContourPoints[i].x;
                    float y0 = _ContourPoints[i].y;
                    float x1 = _ContourPoints[(i + 1) % _ContourPoints.Count].x;
                    float y1 = _ContourPoints[(i + 1) % _ContourPoints.Count].y;
                    float a = x0 * y1 - x1 * y0;
                    signedArea += a;
                    centroid.x += (x0 + x1) * a;
                    centroid.y += (y0 + y1) * a;
                }
                return centroid / (3 * signedArea);
            }
        }
   
        private Type _Type;
        private Color _ContourColor;
        private List<Vector2> _ContourPoints;
        private List<WallObject> _WallObjects;
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
                float rad = SNAPING_RAD / grid.Zoom;
                Handles.DrawWireDisc(p1, Vector3.back, rad);
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
                    Handles.Label(p1 + new Vector2(SNAPING_RAD , SNAPING_RAD), _ContourPoints[i].RoundCoordsToInt().ToString());
                }
                
            }
            if (ApartmentConfigWindow.Config.IsDrawSquare)
            {
                Handles.Label(grid.GridToGUI(Centroid), Square.ToString());
            }
        }
        public void Move(Vector2 dv)
        {
            for (int i = 0; i < _ContourPoints.Count; i++)
            {
                _ContourPoints[i] += dv;
            }
        }
        public bool IsLastPoint(Vector2 point)
        {
            return Vector2.Distance(point, _ContourPoints[0]) < SNAPING_RAD;
        }
        public void RoundContourPoints()
        {
            for(int i = 0; i < _ContourPoints.Count; i++)
            {
                _ContourPoints[i] = _ContourPoints[i].RoundCoordsToInt();
            }
        }
        public int GetContourVertIndex(Vector2 point)
        {
            for(int i = 0; i < _ContourPoints.Count; i++)
            {
                if(Vector2.Distance(point, _ContourPoints[i]) < SNAPING_RAD)
                {
                    return i;
                }
            }
            return -1;
        }

        public Room()
        { 
            _ContourPoints = new List<Vector2>();
            _ContourColor = new Color(Random.Range(0.5f, 1), Random.Range(0.5f, 1), Random.Range(0.5f, 1));
            _Type = Type.None;

        }
        public enum Type
        {
            Kitchen,
            Bathroom,
            Toilet,
            BathroomAndToilet,
            None
        }
    }
    public abstract class WallObject
    {
        public int WallIndex;
        public float WallPosition;  //from 0 to 1
        public float Width;
        public float Height;
    }
    public class Door : WallObject
    {

    }
    public class Window : WallObject
    {
        public float DistanceFromFloor;
    }
}