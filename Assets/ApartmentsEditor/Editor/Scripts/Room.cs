using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Foxsys.ApartmentEditor
{
    [Serializable]
    public class Room : ScriptableObject
    {
        #region factory

        public static Room Create(Apartment parent)
        {
            Room room = CreateInstance<Room>();
            room.name = "Room_" + GUID.Generate();
            AssetDatabase.AddObjectToAsset(room, parent);
            room._ParentApartment = parent;
            room.ContourColor = Color.HSVToRGB(Random.Range(0f, 1f), 0.8f, 1f);
            EditorUtility.SetDirty(parent);
            room._Walls = new List<Wall>();
            AssetDatabase.SaveAssets();
            return room;
        }

        #endregion

        #region fields

        public Color ContourColor;

        public float WallThickness;

        [SerializeField]
        private Apartment _ParentApartment;

        [SerializeField]
        private List<Wall> _Walls;

        #endregion

        #region properties

        public List<Wall> Walls
        {
            get
            {

                return _Walls;
            }
        }

        public Apartment ParentApartment
        {
            get { return _ParentApartment; }
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
                for (int i = 0; i < _Walls.Count; i++)
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

        #endregion

        #region service methods

        public bool Add(Vector2 point)
        {
            var wallsCount = _Walls.Count;
            if (wallsCount > 0)
            {
                _Walls[wallsCount - 1].End = point;
                if (IsLastPoint(point))
                {
                    _Walls[wallsCount - 1].End = _Walls[0].Begin;

                    return false;
                }
            }
            Undo.RecordObject(this, "Add Room point");
            _Walls.Add(new Wall(point));
            return true;
        }
        public void Move(Vector2 dv)
        {
            foreach (Wall wall in _Walls)
            {
                wall.Move(dv);
            }
        }
        public void MoveVertTo(int index, Vector2 position)
        {
            _Walls[index].Begin = position;
            if (index > 0)
                _Walls[index - 1].End = position;
            else
                _Walls[_Walls.Count - 1].End = position;
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

        public void RoundContourPoints()
        {
            foreach (var wall in _Walls)
            {
                wall.Begin = wall.Begin.RoundCoordsToInt();
                wall.End = wall.End.RoundCoordsToInt();
            }
        }

        public Vector2 GetVertPosition(int index)
        {
            return _Walls[index].Begin;
        }

        public int GetContourVertIndex(Vector2 point)
        {
            for (int i = 0; i < _Walls.Count; i++)
            {
                if (Vector2.Distance(point, _Walls[i].Begin) < SnapingRad)
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

        public List<Vector2> GetContourWithThickness()
        {
            List<Vector2> contour = new List<Vector2>(_Walls.Count);
            for (int i = 0, count = _Walls.Count; i < count; i++)
            {
                Wall wall1 = _Walls[i], wall2 = _Walls[(i + 1) % count];
                Vector2 p1 = wall1.Begin - wall1.Normal * WallThickness,
                    p2 = wall1.End - wall1.Normal * WallThickness,
                    p3 = wall2.Begin - wall2.Normal * WallThickness,
                    p4 = wall2.End - wall2.Normal * WallThickness;

                var intersection = MathUtils.LinesInterseciton(p1, p2, p3, p4);
                if (intersection.HasValue)
                    contour.Add(intersection.Value);
            }
            return contour;
        }

        public Vector2 GetNearestPointOnContour(Vector2 point)
        {
            Vector2 result = new Vector2();
            float minDistance = float.MaxValue;
            foreach (var wall in _Walls)
            {
                float distance = MathUtils.DistanceFromPointToLine(point, wall.Begin, wall.End);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    result = MathUtils.PointProjectionToOnLine(point, wall.Begin, wall.End);
                }
            }
            return result;
        }
        public void MakeClockwiseOrientation()
        {
            var contour = GetContour();
            if (MathUtils.IsContourClockwise(contour))
            {
                _Walls.Reverse();
                foreach (var wall in _Walls)
                {
                    wall.Reverse();
                }
            }
        }

        public bool IsVertInsideRect(int vertNum, Rect rect)
        {
            return rect.Contains(_Walls[vertNum].Begin);
        }

        public bool IsPointInside(Vector2 point)
        {
            return MathUtils.IsPointInsideCountour(GetContour(), point);
        }
        public bool IsInsideRect(Rect rect)
        {
            return _Walls.All(wall => rect.Contains(wall.Begin));
        }

        public bool IsLastPoint(Vector2 point)
        {
            return Vector2.Distance(point, _Walls[0].Begin) < SnapingRad;
        }

        #endregion


        #region drawing

        public void Draw(Grid grid)
        {
            int k = 0;
            foreach (Wall wall in _Walls)
            {
                var p1 = wall.Begin;
                var p2 = wall.End;

                wall.Draw(grid, ContourColor);
                Handles.color = Color.white;
                float rad = SnapingRad / grid.Zoom;
                Handles.DrawWireDisc(grid.GridToGUI(p1), Vector3.back, rad);

                Handles.Label(grid.GridToGUI(p2) - Vector2.down * 10, k.ToString());
                k++;
                if (ApartmentConfig.Current.IsDrawSizes)
                {
                    Handles.color = Color.white;
                    Handles.Label(grid.GridToGUI((p1 + p2) / 2),
                        Vector2.Distance(
                            p1,
                            p2).ToString());
                }
                if (ApartmentConfig.Current.IsDrawPositions)
                {
                    Handles.Label(grid.GridToGUI(p1) + new Vector2(SnapingRad, SnapingRad), p1.RoundCoordsToInt().ToString());
                }
            }
            if (WallThickness > 0)
            {
                var contour = GetContourWithThickness();
                for (int i = 0, count = contour.Count; i < contour.Count; i++)
                {
                    Handles.color = new Color(ContourColor.r, ContourColor.g, ContourColor.b, 0.3f);
                    Vector2 p1 = grid.GridToGUI(contour[i]), p2 = grid.GridToGUI(contour[(i + 1) % count]);
                    Handles.DrawLine(grid.GridToGUI(_Walls[i].End), p1);
                    Handles.DrawLine(p1, p2);
                }
            }
            if (ApartmentConfig.Current.IsDrawSquare)
            {
                Handles.Label(grid.GridToGUI(Centroid), Square.ToString());
            }
        }

        #endregion

        public const float SnapingRad = 6f;
        public enum Type
        {
            Kitchen,
            Bathroom,
            Toilet,
            BathroomAndToilet,
            None
        }
    }
}