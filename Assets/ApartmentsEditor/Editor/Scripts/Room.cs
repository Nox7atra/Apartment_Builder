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
            room.MaterialPreset = ScriptableObjectUtils.CreateOrGet<RoomMaterialPreset>("default");
            room.name = parent.GetRoomName(room.MaterialPreset);
            AssetDatabase.AddObjectToAsset(room, parent);
            room._ParentApartment = parent;
            room.WallObjects = new List<ContourObject>();
            EditorUtility.SetDirty(parent);
            room._Contour = new List<RoomVert>();
            AssetDatabase.SaveAssets();
            return room;
        }

        #endregion

        #region fields
        
        public float WallThickness;
        
        [SerializeField]
        private Apartment _ParentApartment;

        [SerializeField]
        private List<RoomVert> _Contour;

        public bool IsShowSizes;
        public bool IsShowSquare;
        public bool IsShowPositions;
        public RoomMaterialPreset MaterialPreset;
        [SerializeField]
        public List<ContourObject> WallObjects;
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
            var snappedPoint = ParentApartment.GetVertInPos(point, this);
            point = snappedPoint != null ? snappedPoint.Position : point;
            Undo.RecordObject(this, "Add Room point");
            _Contour.Add(new RoomVert(this, point));
            return true;
        }
        public void MoveTo(Vector2 position)
        {
            //var center = Centroid;
            //for (int i = 0, count = _Contour.Count; i < count; i++)
            //{
            //    var newPos = _Contour[i].Position + position - center;
            //    if (!_ParentApartment.Dimensions.Contains(newPos))
            //        return;
            //    foreach (Room room in _ParentApartment.Rooms)
            //    {
            //        if(room != this && room.IsPointInside(newPos))
            //            return;
            //    }
            //}
            //for (int i = 0, count = _Contour.Count; i < count; i++)
            //{
            //    var newPos = _Contour[i].Position + position - center;
            //    _Contour[i].MoveTo(newPos);
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
                Vector2 vert1 = _Contour[i].Position,
                    vert2 = _Contour[(i + 1) % count].Position,
                    vert3 = _Contour[(i + 2) % count].Position;
                Vector2 normal1 = MathUtils.CalculateNormal(vert1, vert2),
                    normal2 = MathUtils.CalculateNormal(vert2, vert3);
                Vector2 p1 = vert1 - normal1 * WallThickness,
                    p2 = vert2 - normal1 * WallThickness,
                    p3 = vert2 - normal2 * WallThickness,
                    p4 = vert3 - normal2 * WallThickness;

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

                    tangent = MathUtils.CalculateTangent(v1.Position, v2.Position);
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
                foreach (var wallObject in WallObjects)
                {
                    wallObject.Position = 1 - wallObject.Position;
                }
            }
        }

        public RoomVert GetVertInPos(Vector2 point)
        {
            return _Contour.Find(x => Vector2.Distance(x.Position, point) < SkinManager.Instance.CurrentSkin.CirclesRad);
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
            return Vector2.Distance(point, _Contour[0].Position) < SkinManager.Instance.CurrentSkin.CirclesRad;
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

        public void Draw( bool isClosed = true)
        {
            var color = MaterialPreset.Color;
            for (int i = 0, count = _Contour.Count ; i < (isClosed ? count : count - 1); i++)
            {
                var p1 = _Contour[i].Position;
                var p2 = _Contour[(i + 1) % count].Position;

                Handles.color = color;

                WindowObjectDrawer.DrawLine(p1, p2);

                _Contour[i].Draw();

                if (IsShowSizes)
                {
                    WindowObjectDrawer.DrawLabel(
                        (p1 + p2) / 2,
                        Vector2.Distance(p1, p2).ToString());
                }
                if (IsShowPositions)
                {
                    WindowObjectDrawer.DrawLabel(p1 , p1.RoundCoordsToInt().ToString());
                }
            }
            if (WallThickness > 0)
            {
                var contour = GetContourWithThickness();
                for (int i = 0, count = contour.Count; i < contour.Count; i++)
                {
                    Handles.color = new Color(color.r, color.g, color.b, 0.5f);
                    Vector2 p1 = contour[i], p2 = contour[(i + 1) % count];
                    WindowObjectDrawer.DrawLine(_Contour[(i + 1) % _Contour.Count].Position, p1);
                    WindowObjectDrawer.DrawLine(p1, p2);
                }
            }
         
            if (IsShowSquare)
            {
                WindowObjectDrawer.DrawLabel(Centroid, Square.ToString());;
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
                WindowObjectDrawer.DrawLine(p1, p2);
            }
        }

        public void EndMoving(bool IsSnap = true)
        {
            RoundContourPoints();
        }

        #endregion
    }
    [Serializable]
    public class RoomVert : ISelectable, IWallObject
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
            WindowObjectDrawer.DrawCircle(_Position);
            WindowObjectDrawer.DrawLabel(_Position, _Position.RoundCoordsToInt().ToString());
        }

        public void EndMoving(bool IsSnap = true)
        {
            var vert = _Parent.ParentApartment.GetVertInPos(_Position, _Parent);
            _Position = vert != null ? vert.Position : _Position.RoundCoordsToInt();
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

        public bool TryAddObject(Vector2 position)
        {
            var room = ApartmentsManager.Instance.CurrentApartment.GetNearestRoom(position);
            if (room != null)
            {
                var projection = room.GetNearestPointOnContour(position);
                var contour = room.Contour;
                _Position = projection.Value;
                for (int i = 0, count = contour.Count; i < count; i++)
                {
                    Vector2 p1 = contour[i].Position, p2 = contour[(i + 1) % count].Position;
                    if(MathUtils.IsPointInsideLineSegment(_Position, p1, p2))
                    {
                        _Parent = room;
                        room.Contour.Insert(i + 1, this);
                        return true;
                    }
                }
            }
            return false;
        }

        public void DrawOnWall(Vector2 position)
        {
            var apartment = ApartmentsManager.Instance.CurrentApartment;
            var room = apartment.GetNearestRoom(position);
            if (room != null)
            {
                var projection = room.GetNearestPointOnContour(position);
                Handles.color = SkinManager.Instance.CurrentSkin.VertColor;

                WindowObjectDrawer.DrawCircle(projection.Value);
            }
        }

        public void Draw()
        {
            Handles.color = SkinManager.Instance.CurrentSkin.VertColor;
            WindowObjectDrawer.DrawCircle(_Position);
        }
    }

    [Serializable]
    public class ContourObject : ISelectable
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
            var position = GetVector2Position();
            Handles.color = color;
            WindowObjectDrawer.DrawCircle(position);

            WindowObjectDrawer.DrawLabel(position, position.RoundCoordsToInt().ToString());
        }

        public void EndMoving(bool IsSnap = true)
        {
        }

        public List<Vector2> GetHole(Vector2 position, Vector2 wallBegin, Vector2 wallEnd)
        {
            return Object.GetHole(position, wallBegin, wallEnd);
        }
        public Vector2 GetVector2Position()
        {
            return Parent.PositionOnContourToPoint(Position).RoundCoordsToInt();
        }
        public void MoveTo(Vector2 position)
        {
            Position = Parent.PointToPositionOnContour(Parent.GetNearestPointOnContour(position).Value);
        }
    }
}