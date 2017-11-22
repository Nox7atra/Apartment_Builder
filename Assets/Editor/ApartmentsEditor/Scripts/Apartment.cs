using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using JetBrains.Annotations;

namespace Nox7atra.ApartmentEditor
{
    public class Apartment : ScriptableObject
    {
        //factory
        public static Apartment Create()
        {
            var path = ApartmentsManager.DataPath + "test.asset";
            Apartment apartment = AssetDatabase.LoadAssetAtPath<Apartment>(path);

            if (apartment == null)
            {
                apartment = CreateInstance<Apartment>();
                apartment._Rooms = new List<Room>();
                apartment.Dimensions = new Rect(-Vector2.one * 500, Vector2.one * 1000);
                AssetDatabase.CreateAsset(apartment, path);
            }
            return apartment;
        }

        public float Height;
        public Rect Dimensions;

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
                room.Draw(grid);
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

    }
}
