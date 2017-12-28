using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Foxsys.ApartmentEditor
{
    [Serializable]
    public abstract class WallObject : ScriptableObject
    {
        #region fields

        [SerializeField]
        public float Width;
        [SerializeField]
        public float Height;

        #endregion

        public abstract List<Vector2> GetHole(Vector2 position, Vector2 wallBegin, Vector2 wallEnd);
        public void Draw(ApartmentEditorGrid grid, Vector2 position)
        {
            var apartment = ApartmentsManager.Instance.CurrentApartment;
            var gridpos = grid.GUIToGrid(position);
            var room = apartment.GetNearestRoom(gridpos);
            if (room != null)
            {
                Vector2 tangent;
                var projection = room.GetNearestPointOnContour(gridpos, out tangent);
                Handles.color = this is Door ? SkinManager.Instance.CurrentSkin.DoorColor : SkinManager.Instance.CurrentSkin.WindowColor;

                WindowObjectDrawer.DrawCircle(projection.Value);
                WindowObjectDrawer.DrawLine(projection.Value - tangent * Width / 2, projection.Value + tangent * Width / 2);
            }
        }

        public float CalculateOffset(Vector2 position, Vector2 wallBegin, Vector2 wallEnd, bool isFromBegin)
        {
            return Vector2.Distance(position, wallEnd) - Vector2.Distance(wallEnd, wallBegin) / 2 + (isFromBegin ? Width / 2 : -Width / 2);
        }
        public bool TryAddObject(ApartmentEditorGrid grid, Vector2 position)
        {
            var gridpos = grid.GUIToGrid(position);
            var room = ApartmentsManager.Instance.CurrentApartment.GetNearestRoom(gridpos);
            var projection = room.GetNearestPointOnContour(gridpos);
            if (room != null)
            {
                room.WallObjects.Add(new CountourObject
                {
                    Parent = room,
                    Position = room.PointToPositionOnContour(projection.Value),
                    Object = this
                });
                return true;
            }
            return false;
        }
    }
}