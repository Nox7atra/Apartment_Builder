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
    public class Room : ScriptableObject, ISelectable
    {
        #region factory

        public static Room Create(Apartment parent)
        {
            Room room = CreateInstance<Room>();
            room.name = parent.GetRoomName(room.CurrentType);
            AssetDatabase.AddObjectToAsset(room, parent);
            room._ParentApartment = parent;
            room.WallObjects = new List<CountourObject>();
            EditorUtility.SetDirty(parent);
            room._Contour = new List<RoomVert>();
            AssetDatabase.SaveAssets();
            return room;
        }

        #endregion

        #region fields
        
        public Type CurrentType;
        
        public float WallThickness;
        
        [SerializeField]
        private Apartment _ParentApartment;

        [SerializeField]
        private List<RoomVert> _Contour;

        public bool IsShowSizes;
        public bool IsShowSquare;
        public bool IsShowPositions;
        public List<CountourObject> WallObjects;
        #endregion

        #region properties

        public List<RoomVert> Contour
        {
            get
            {

                return _Contour;
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
                for (int i = 0, count = _Contour.Count; i < count; i++)
                {
                    Vector2 p1 = _Contour[i].Position,
                        p2 = _Contour[(i + 1) % count].Position;
                    result +=
                        0.5f * (p1.x * p2.y - p1.y * p2.x);
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
                for (int i = 0, count = _Contour.Count; i < count; i++)
                {
                    Vector2 p1 = _Contour[i].Position,
                        p2 = _Contour[(i + 1) % count].Position;
                    float x0 = p1.x;
                    float y0 = p1.y;
                    float x1 = p2.x;
                    float y1 = p2.y;
                    float a = x0 * y1 - x1 * y0;
                    signedArea += a;
                    centroid.x += (x0 + x1) * a;
                    centroid.y += (y0 + y1) * a;
                }
                return centroid / (3 * signedArea);
            }
        }

        public float ContourLength
        {
            get
            {
                float length = 0;
                for (int i = 0, count = _Contour.Count; i < count; i++)
                {
                    length += Vector2.Distance(_Contour[i].Position, _Contour[(i + 1) % count].Position);
                }
                return length;
            }
        }
        #endregion

        #region service methods

        public bool Add(Vector2 point)
        {
            if (_Contour.Count > 2 && IsLastPoint(point))
            {
                return false;
            }
            
            Undo.RecordObject(this, "Add Room point");
            _Contour.Add(new RoomVert(this, point));
            return true;
        }
        public void MoveTo(Vector2 position)
        {
            var center = Centroid;
            var oldPoses = new Vector2[_Contour.Count];
            for(int i = 0, count = _Contour.Count; i < count; i++)
            {
                oldPoses[i] = _Contour[i].Position;
                var newPos = _Contour[i].Position + position - center;
                _Contour[i].MoveTo(newPos);
            }
                //if (!this.IsInsideRect(_ParentApartment.Dimensions))
                //{
                //    for (int i = 0; i < oldPoses.Length; i++)
                //    {
                //        _Contour[i].MoveTo(oldPoses[i]);
                //    }
                //}
            
        }

        public void RemoveVert(int index)
        {
            _Contour.RemoveAt(index);
        }

        public void RoundContourPoints()
        {
            foreach (var vert in _Contour)
            {
                vert.RoundCoordsToInt();
            }
        }

        public List<Vector2> GetContourWithThickness()
        {
            List<Vector2> contour = new List<Vector2>();
            for (int i = 0, count = _Contour.Count; i < count; i++)
            {
                RoomVert vert1 = _Contour[i],
                    vert2 = _Contour[(i + 1) % count],
                    vert3 = _Contour[(i + 2) % count];
                Wall wall1 = new Wall(vert1, vert2), wall2 = new Wall(vert2, vert3);
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

        public Vector2? GetNearestPointOnContour(Vector2 point, out Vector2 tangent)
        {
            var result = GetNearestPointOnContour(point);
            tangent = new Vector2();
            for (int i = 0, count = _Contour.Count; i < count; i++)
            {
                RoomVert v1 = _Contour[i],
                    v2 = _Contour[(i + 1) % count];
                if (MathUtils.IsPointInsideLineSegment(result.Value, v1.Position, v2.Position))
                {
                    var wall = new Wall(v1, v2);
                    tangent = wall.Tangent;
                }
            }
            return result;
        }

        public Vector2? GetNearestPointOnContour(Vector2 point)
        {

            Vector2 result = new Vector2();
            float minDistance = float.MaxValue;
            for (int i = 0, count = _Contour.Count; i < count; i++)
            {
                Vector2 p1 = _Contour[i].Position,
                    p2 = _Contour[(i + 1) % count].Position;

                var projection = MathUtils.PointProjectionToOnLine(point, p1, p2);

                float distance = MathUtils.DistanceFromPointToLine(point, p1, p2);
                
                if (distance < minDistance)
                {
                    minDistance = distance;
                    
                    result = MathUtils.IsPointInsideLineSegment(projection, p1, p2) ? (Vector2) projection :
                        (Vector2.Distance(p1, projection) < Vector2.Distance(p2, projection) ? p1 : p2);
      
                }
            }
            return result;
        }
        public void MakeClockwiseOrientation()
        {
            if (MathUtils.IsContourClockwise(_Contour.Select(x => x.Position).ToList()))
            {
                _Contour.Reverse();
            }
        }

        public RoomVert GetVertInPos(Vector2 point)
        {
            return _Contour.Find(x => Vector2.Distance(x.Position, point) < SnapingRad);
        }
        public bool IsVertInsideRect(int vertNum, Rect rect)
        {
            return rect.Contains(_Contour[vertNum].Position);
        }

        public bool IsPointInside(Vector2 point)
        {
            return MathUtils.IsPointInsideCountour(_Contour.Select(x => x.Position).ToList(), point);
        }
        public bool IsInsideRect(Rect rect)
        {
            return _Contour.All(vert => rect.Contains(vert.Position));
        }

        public bool IsLastPoint(Vector2 point)
        {
            return Vector2.Distance(point, _Contour[0].Position) < SnapingRad;
        }

        public float PointToPositionOnContour(Vector2 point)
        {
            float length = 0;
            for (int i = 0, count = _Contour.Count; i < count; i++)
            {
                Vector2 p1 = _Contour[i].Position, p2 = _Contour[(i + 1) % count].Position;
                if (MathUtils.IsPointInsideLineSegment(point, p1, p2))
                {
                    length += Vector2.Distance(p1, point);
                    break;
                }
                else
                {
                    length += Vector2.Distance(p1, p2);
                }
            }

            return length / ContourLength;
        }

        public Vector2 PositionOnContourToPoint(float position)
        {
            float length = 0;
            for (int i = 0, count = _Contour.Count; i < count; i++)
            {
                Vector2 p1 = _Contour[i].Position, p2 = _Contour[(i + 1) % count].Position;
                var distance = Vector2.Distance(p1, p2) / ContourLength;
                if (length + distance < position)
                {
                    length += distance;
                }
                else
                {
                    return Vector2.Lerp(p1, p2, (position - length) * ContourLength / Vector2.Distance(p1, p2));
                }
            }
            return _Contour[0].Position;
        }
        #endregion


        #region drawing

        public void Draw(ApartmentEditorGrid grid, bool isClosed = true)
        {
            var currentSkin = SkinManager.Instance.CurrentSkin;
            var labelStyle = currentSkin.TextStyle;
            Color color = currentSkin.GetColor(CurrentType);
            for (int i = 0, count = _Contour.Count ; i < (isClosed ? count : count - 1); i++)
            {
                var p1 = _Contour[i].Position;
                var p2 = _Contour[(i + 1) % count].Position;

                Handles.color = color;
                Handles.DrawLine(grid.GridToGUI(p1), grid.GridToGUI(p2));

                Handles.color = currentSkin.VertColor;
                float rad = SnapingRad / grid.Zoom;
                Handles.DrawWireDisc(grid.GridToGUI(p1), Vector3.back, rad);

                if (IsShowSizes)
                {
                    Handles.Label(grid.GridToGUI((p1 + p2) / 2),
                        Vector2.Distance(
                            p1,
                            p2).ToString(), labelStyle);
                }
                if (IsShowPositions)
                {
                    Handles.Label(grid.GridToGUI(p1) + new Vector2(SnapingRad, SnapingRad), p1.RoundCoordsToInt().ToString(), labelStyle);
                }
            }
            if (WallThickness > 0)
            {
                var contour = GetContourWithThickness();
                for (int i = 0, count = contour.Count; i < contour.Count; i++)
                {
                    Handles.color = new Color(color.r, color.g, color.b, 0.5f);
                    Vector2 p1 = grid.GridToGUI(contour[i]), p2 = grid.GridToGUI(contour[(i + 1) % count]);
                    Handles.DrawLine(grid.GridToGUI(_Contour[(i + 1) % _Contour.Count].Position), p1);
                    Handles.DrawLine(p1, p2);
                }
            }
            if (WallObjects.Count > 0)
            {
                for (int i = 0, count = WallObjects.Count; i < count; i++)
                {
                    WallObjects[i].Object.Draw(grid, grid.GridToGUI(PositionOnContourToPoint(WallObjects[i].Position)));
                }
            }
            if (IsShowSquare)
            {
                Handles.Label(grid.GridToGUI(Centroid), Square.ToString(), labelStyle);
            }
        }

        public void Delete()
        {
            _ParentApartment.Rooms.Remove(this);
            _ParentApartment = null;
            DestroyImmediate(this, true);

        }

        public void DrawSelection(ApartmentEditorGrid grid, Color color)
        {
            Handles.color = color;
            for (int i = 0, count = _Contour.Count; i < count; i++)
            {
                var p1 = _Contour[i].Position;
                var p2 = _Contour[(i + 1) % count].Position;

                Handles.color = color;
                Handles.DrawLine(grid.GridToGUI(p1), grid.GridToGUI(p2));
            }
        }

        public void EndMoving()
        {
            RoundContourPoints();
        }

        #endregion

        public const float SnapingRad = 20f;
        public enum Type
        {
            Kitchen = 0,
            Bathroom = 1,
            Toilet = 2,
            Living = 3
        }
    }
    [Serializable]
    public class RoomVert : ISelectable
    {
        [SerializeField] private Room _Parent;
        [SerializeField] private Vector2 _Position;

        public Vector2 Position
        {
            get { return _Position; }
        }

        
        public void MoveTo(Vector2 position)
        {
            var room = _Parent.ParentApartment.GetRoomWithInsidePoint(position, _Parent);
            position = room != null
                ? room.GetNearestPointOnContour(position).Value
                : position;
            _Position = _Parent.ParentApartment.Dimensions.Clamp(position);
        }

        public void Delete()
        {
            _Parent.Contour.Remove(this);
            _Parent = null;
        }

        public void DrawSelection(ApartmentEditorGrid grid, Color color)
        {
            Handles.color = color;
            Handles.DrawWireDisc(grid.GridToGUI(_Position), Vector3.back, Room.SnapingRad / grid.Zoom);
        }

        public void EndMoving()
        {
            _Position = _Position.RoundCoordsToInt();
        }

        public void RoundCoordsToInt()
        {
            _Position = _Position.RoundCoordsToInt();
        }
        public RoomVert(Room parent, Vector2 position)
        {
            _Parent = parent;
            _Position = position;
        }
    }

    [Serializable]
    public class CountourObject : ISelectable
    {
        public float Position;
        public Room Parent;
        public WallObject Object;

        public void Delete()
        {
            Parent.WallObjects.Remove(this);
        }

        public void DrawSelection(ApartmentEditorGrid grid, Color color)
        {
            Handles.color = color;
            Handles.DrawWireDisc(grid.GridToGUI(Parent.PositionOnContourToPoint(Position)), Vector3.back, Room.SnapingRad / grid.Zoom);
        }

        public void EndMoving()
        {
        }

        public void MoveTo(Vector2 position)
        {
            Position = Parent.PointToPositionOnContour(Parent.GetNearestPointOnContour(position).Value);
        }
    }
}