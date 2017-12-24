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
                Handles.DrawWireDisc(grid.GridToGUI(projection.Value), Vector3.back, Room.SnapingRad / grid.Zoom);
                Handles.DrawLine(grid.GridToGUI(projection.Value - tangent * Width / 2), grid.GridToGUI(projection.Value + tangent * Width / 2));
            }
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