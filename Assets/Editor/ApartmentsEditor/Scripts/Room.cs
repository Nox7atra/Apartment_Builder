using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        public List<Wall> Walls
        {
            get
            {
                return _Walls;
            }
        }
        public float Square
        {
            get
            {
                float result = 0;
                int pointsCount = _Walls.Count;
                for (int i = 0; i < pointsCount; i++)
                {
                    Wall wall = _Walls[i];
                    result +=
                        0.5f * (wall.Begin.x * wall.End.y - wall.Begin.y * wall.End.x);
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
                for(int i = 0; i < _Walls.Count; i++)
                {
                    Wall wall = _Walls[i];
                    float x0 = wall.Begin.x;
                    float y0 = wall.Begin.y;
                    float x1 = wall.End.x;
                    float y1 = wall.End.y;
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
        private List<Wall> _Walls;

        public void Draw(Grid grid)
        {
            for (int i = 0; i < _Walls.Count; i++)
            {
                var p1 = grid.GridToGUI(_Walls[i].Begin);
                var p2 = grid.GridToGUI(_Walls[i].End);

                _Walls[i].Draw(grid, _ContourColor);
             
                Handles.color = Color.white;
                float rad = SNAPING_RAD / grid.Zoom;
                Handles.DrawWireDisc(p1, Vector3.back, rad);
                if (ApartmentConfig.Current.IsDrawSizes)
                {
                    Handles.color = Color.white;
                    Handles.Label((p1 + p2) / 2, 
                        Vector2.Distance(
                            p1,
                            p2).ToString());
                }
                if (ApartmentConfig.Current.IsDrawPositions)
                {
                    Handles.Label(p1 + new Vector2(SNAPING_RAD , SNAPING_RAD), p1.RoundCoordsToInt().ToString());
                }
                
            }
            if (ApartmentConfig.Current.IsDrawSquare)
            {
                Handles.Label(grid.GridToGUI(Centroid), Square.ToString());
            }
        }

        public bool Add(Vector2 point)
        {
            
            var wallsCount = _Walls.Count;
            if(wallsCount > 0)
            {
                _Walls[wallsCount - 1].End = point;
                if (IsLastPoint(point))
                {
                    _Walls[wallsCount - 1].End = _Walls[0].Begin;
                    return true;
                }
                else
                {
                    _Walls.Add(new Wall(point));
                }
            }
            else
            {
                _Walls.Add(new Wall(point));
            }
            return false;
        }
        public void Move(Vector2 dv)
        {
            for (int i = 0; i < _Walls.Count; i++)
            {
                _Walls[i].Move(dv);
            }
        }
        public void MoveVert(int index, Vector2 dv)
        {
            _Walls[index].Begin   += dv;
            if (index > 0)
                _Walls[index - 1].End += dv;
            else
                _Walls[_Walls.Count - 1].End += dv;
        }
        public void RemoveVert(int index)
        {
            if (index > 0)
            {
                _Walls[index - 1].End = _Walls[index].End;
            }
            else
            {
                _Walls[_Walls.Count - 1].End = _Walls[index].End;
            }
            _Walls.RemoveAt(index);
        }
        public bool IsLastPoint(Vector2 point)
        {
            return Vector2.Distance(point, _Walls[0].Begin) < SNAPING_RAD;
        }
        public void RoundContourPoints()
        {
            for(int i = 0; i < _Walls.Count; i++)
            {
                _Walls[i].Begin = _Walls[i].Begin.RoundCoordsToInt();
                _Walls[i].End = _Walls[i].End.RoundCoordsToInt();
            }
        }
        public Vector2 GetVertPosition(int index)
        {
            return _Walls[index].Begin;
        }
        public int GetContourVertIndex(Vector2 point)
        {
            for(int i = 0; i < _Walls.Count; i++)
            {
                if(Vector2.Distance(point, _Walls[i].Begin) < SNAPING_RAD)
                {
                    return i;
                }
            }
            return -1;
        }
        public List<Vector2> GetContour()
        {
            return _Walls.Select(x => x.Begin).ToList();
        }
        public Room()
        {
            _Walls = new List<Wall>();
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
    public class Wall
    {
        public Vector2 Begin;
        public Vector2 End;
        public List<WallObject> _Objects;
        public void Move(Vector2 dv)
        {
            Begin += dv;
            End += dv;
        }
        public void Draw(Grid grid, Color color)
        {
            Handles.color = color;
            Handles.DrawLine(grid.GridToGUI(Begin), grid.GridToGUI(End));
        }
        public Wall()
        {
            _Objects = new List<WallObject>();
        }
        public Wall(Vector2 point)
        {
            Begin = point;
            End   = point;
            _Objects = new List<WallObject>();
        }
    }
    public abstract class WallObject
    {
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