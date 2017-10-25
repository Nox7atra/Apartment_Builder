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
                apartment.Dimensions = new Vector2(1000,1000);
                AssetDatabase.CreateAsset(apartment, path);
            }
            return apartment;
        }
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

        public Vector2 Dimensions;

        [SerializeField]
        private List<Room> _Rooms;

        private string _Name;
        public void Draw(Grid grid)
        {
            DrawDimensions(grid);
            for(int i = 0; i < _Rooms.Count; i++)
            {
                _Rooms[i].Draw(grid);
            }
        }
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

    }
}
