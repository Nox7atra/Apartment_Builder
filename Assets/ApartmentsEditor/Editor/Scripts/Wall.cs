using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Foxsys.ApartmentEditor
{
    public struct Wall
    {
        #region fields

        private RoomVert _Begin;
        private RoomVert _End;

        #endregion

        #region properties

        public Vector2 Begin
        {
            get { return _Begin.Position; }
        }

        public Vector2 End
        {
            get { return _End.Position; }
        }
        public Vector2 Center
        {
            get { return (End + Begin) / 2; }
        }

        public Vector2 Normal
        {
            get { return new Vector2(Begin.y - End.y, End.x - Begin.x).normalized; }
        }

        public Vector2 Tangent
        {
            get { return new Vector2(Begin.x - End.x, Begin.y - End.y).normalized; }
        }

        public float Length
        {
            get { return Vector2.Distance(Begin, End); }
        }

        public float Rotation
        {
            get { return -Vector2.SignedAngle(Vector2.right, End - Begin); }
        }

        #endregion

        
        #region service methods

   
        public bool IsPointOnWall(Vector2 point)
        {
            return MathUtils.IsPointInsideLineSegment(point, Begin, End);
        }

        public float GetPositionWallObjectFromPoint(Vector2 point)
        {
            return Vector2.Distance(Begin, point) / Length;
        }

        #endregion

        #region drawing

        public void Draw(Grid grid, Color color)
        {
            Handles.color = color;
            Handles.DrawLine(grid.GridToGUI(Begin), grid.GridToGUI(End));
        }

        public void DrawWallObject(Grid grid, WallObject wallObj, float wallObjPosition)
        {
            Handles.color = Color.magenta;
            Vector2 begin = grid.GridToGUI(Begin), end = grid.GridToGUI(End);
            Vector2 center = Vector2.Lerp(begin, end, wallObjPosition);
            Handles.DrawWireDisc(center, Vector3.back, 6 / grid.Zoom);
            Handles.DrawLine(center + Tangent * wallObj.Width / 2, center - Tangent * wallObj.Width / 2);
            Handles.DrawLine(center + Tangent * wallObj.Width / 2, center - Tangent * wallObj.Width / 2);
        }

        #endregion

        public Wall(RoomVert begin, RoomVert end)
        {
            _Begin = begin;
            _End = end;
        }
        public static Wall? MergeWalls(Wall w1, Wall w2)
        {
            Wall? merged = null;
            if (w1.Begin == w2.Begin)
            {
                merged = new Wall
                {
                    _Begin = w1._End,
                    _End = w2._End
                };
            }
            else if(w1.End == w2.End)
            {
                merged = new Wall
                {
                    _Begin = w1._Begin,
                    _End = w2._Begin
                };
            }
            else if (w1.Begin == w2.End)
            {
                merged = new Wall
                {
                    _Begin = w1._End,
                    _End = w2._Begin
                };
            }
            else if (w1.End == w2.Begin)
            {
                merged = new Wall
                {
                    _Begin = w1._Begin,
                    _End = w2._End
                };
            }
            return merged;
        }

    }
}