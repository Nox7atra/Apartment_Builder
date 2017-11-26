using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace Foxsys.ApartmentEditor
{
    public class Apartment : ScriptableObject
    {
        //factory
        public static Apartment Create(string name)
        {
            var fullpath = Path.Combine(PathsConfig.Instance.PathToApartments,"test.asset");
            Apartment apartment = AssetDatabase.LoadAssetAtPath<Apartment>(fullpath);

            if (apartment == null)
            {
                apartment = CreateInstance<Apartment>();
                apartment._Rooms = new List<Room>();
                apartment.Dimensions = new Rect(-Vector2.one * 500, Vector2.one * 1000);
                AssetDatabase.CreateAsset(apartment, fullpath);
            }
            return apartment;
        }

        public float Height;
        public float WallThickness;
        public Rect Dimensions;
        public Material WallMaterial;
        public Material FloorMaterial;
        [SerializeField]
        private List<Room> _Rooms;

       
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
                foreach (Room room in _Rooms)
                {
                    result += room.Square;
                }
                return result;
            }
        }

        public void Draw(Grid grid)
        {
            DrawDimensions(grid);
            foreach (Room room in _Rooms)
            {
                room.Draw(grid, WallThickness);
            }
        }
        private void DrawDimensions(Grid grid)
        {
            Handles.color = Color.green;
            Handles.DrawLine(
                grid.GridToGUI(
                    new Vector3(Dimensions.width / 2, Dimensions.height/ 2)),
                grid.GridToGUI(
                    new Vector3(Dimensions.width / 2, -Dimensions.height / 2)));
            Handles.DrawLine(
                grid.GridToGUI(
                    new Vector3(Dimensions.width / 2, -Dimensions.height / 2)),
                grid.GridToGUI(
                    new Vector3(-Dimensions.width / 2, -Dimensions.height / 2)));
            Handles.DrawLine(
                grid.GridToGUI(
                    new Vector3(-Dimensions.width / 2, -Dimensions.height / 2)),
                grid.GridToGUI(
                    new Vector3(-Dimensions.width / 2, Dimensions.height / 2)));
            Handles.DrawLine(
                grid.GridToGUI(
                    new Vector3(-Dimensions.width / 2, Dimensions.height / 2)),
                grid.GridToGUI(
                    new Vector3(Dimensions.width / 2, Dimensions.height / 2)));
        }

        public bool IsApartmentInRect(Rect rect)
        {
            foreach (var room in _Rooms)
            {
                if (!room.IsInsideRect(rect))
                    return false;
            }
            return true;
        }
    }
}
