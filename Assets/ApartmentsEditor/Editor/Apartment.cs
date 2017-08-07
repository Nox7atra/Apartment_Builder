using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    public class Apartment
    {
        #region properties
        public List<Room> Rooms
        {
            get
            {
                return _Rooms;
            }
        }
        public float Square
        {
            get
            {
                float result = 0;
                for (int i = 0; i < _Rooms.Count; i++)
                {
                    result += _Rooms[i].Square;
                }
                return result;
            }
        }
        #endregion

        #region attributes
        public Vector2 Dimensions;

        private List<Room> _Rooms;
        #endregion

        #region public methods
        public void Draw(Grid grid)
        {
            DrawDimensions(grid);
            for(int i = 0; i < _Rooms.Count; i++)
            {
                _Rooms[i].Draw(grid);
            }
        }
        #endregion

        #region service methods
        private void DrawDimensions(Grid grid)
        {
            Handles.color = Color.green;
            Handles.DrawLine(
                grid.GridToGUI(
                    new Vector3(Dimensions.x / 2, Dimensions.y / 2)),
                grid.GridToGUI(
                    new Vector3(Dimensions.x / 2, -Dimensions.y / 2)));
            Handles.DrawLine(
                grid.GridToGUI(
                    new Vector3(Dimensions.x / 2, -Dimensions.y / 2)),
                grid.GridToGUI(
                    new Vector3(-Dimensions.x / 2, -Dimensions.y / 2)));
            Handles.DrawLine(
                grid.GridToGUI(
                    new Vector3(-Dimensions.x / 2, -Dimensions.y / 2)),
                grid.GridToGUI(
                    new Vector3(-Dimensions.x / 2, Dimensions.y / 2)));
            Handles.DrawLine(
                grid.GridToGUI(
                    new Vector3(-Dimensions.x / 2, Dimensions.y / 2)),
                grid.GridToGUI(
                    new Vector3(Dimensions.x / 2, Dimensions.y / 2)));
        }
        #endregion

        #region constructor
        public Apartment()
        {
            _Rooms = new List<Room>();
            Dimensions = new Vector2(1000, 1000);
        }
        #endregion
    }
}
