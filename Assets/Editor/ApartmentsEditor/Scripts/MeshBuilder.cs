using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    public static class MeshBuilder
    {
        public static GameObject GenerateApartmentMesh(Apartment apartment)
        {
            GameObject go = new GameObject("Apartment");

            var rooms = GenerateRooms(apartment, new EarCuttingTriangulator());
            rooms.transform.SetParent(go.transform);
            return go;
        }

        private static GameObject GenerateRooms(Apartment apartment, Triangulator triangulator)
        {
            GameObject go = new GameObject("Room");

            for (int i = 0; i < apartment.Rooms.Count; i++)
            {
                var room = apartment.Rooms[i];
                for (int j = 0; j < room.Walls.Count; j++)
                {
                    var wall = room.Walls[i];
                    GameObject wallGO = GenerateWall(wall, triangulator);

                    wallGO.transform.SetParent(go.transform);
                }
            }
            return go;
        }

        private static GameObject GenerateWall(Wall wall, Triangulator triangulator)
        {
            GameObject wallGO = new GameObject("wall");
            MeshFilter wallMf = wallGO.AddComponent<MeshFilter>();
            MeshRenderer wallMr = wallGO.AddComponent<MeshRenderer>();

            wallGO.transform.position = wall.Center;
            wallGO.transform.rotation = Quaternion.Euler(0, wall.Rotation, 0);
            float length = Vector2.Distance(wall.Begin, wall.End);
            Debug.Log(length);
            List<Vector2> wallContour = new List<Vector2>
            {
                new Vector2(length / 2, length / 2),
                new Vector2(-length / 2, length / 2),
                new Vector2(-length / 2, -length / 2),
                new Vector2(length / 2, -length / 2)
            };

            wallMf.mesh = triangulator.CreateMesh(wallContour);
            wallMf.sharedMesh.RecalculateNormals();
            return wallGO;
        }
    }
}