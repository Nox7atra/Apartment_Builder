using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    public static class MeshBuilder
    {
        private const float MeasurmentK = 100;
        public static GameObject GenerateApartmentMesh(Apartment apartment)
        {
            GameObject go = new GameObject(apartment.name);

            var rooms = GenerateRooms(apartment, new EarCuttingTriangulator());

            rooms.transform.SetParent(go.transform);
            return go;
        }

        private static GameObject GenerateRooms(Apartment apartment, Triangulator triangulator)
        {
            GameObject go = new GameObject("Rooms");
            Vector2 maxDimension = apartment.Dimensions.size;
            for (var i = 0; i < apartment.Rooms.Count; i++)
            {
                GameObject roomGo = new GameObject("Room" + i);
                var room = apartment.Rooms[i];

                room.MakeClockwiseOrientation();
                var contour = room.GetContour();
                for (var j = 0; j < contour.Count; j++)
                {
                    contour[j] = contour[j] / MeasurmentK;
                }

                float height = apartment.Height / MeasurmentK;
                foreach (var wall in room.Walls)
                {
                    GameObject wallGO = GenerateWall(wall, triangulator, maxDimension, height, apartment.WallMaterial);
                    wallGO.transform.SetParent(roomGo.transform);
                }
                GameObject floorGO = GenerateFloorRoof(contour, triangulator, height, maxDimension, true, apartment.FloorMaterial);
                contour.Reverse();
                GameObject roofGO = GenerateFloorRoof(contour, triangulator, height, maxDimension, false, apartment.FloorMaterial);
                roofGO.transform.SetParent(roomGo.transform);
                floorGO.transform.SetParent(roomGo.transform);

                roomGo.transform.SetParent(go.transform);
            }
            return go;
        }

        private static GameObject GenerateFloorRoof(List<Vector2> roomContour, Triangulator triangulator, float apartmentHeight, Vector2 maxDimensions, bool isFloor, Material floorMat)
        {
            GameObject go = new GameObject(isFloor ? "floor" : "roof");
            MeshFilter mf = go.AddComponent<MeshFilter>();
            MeshRenderer mr = go.AddComponent<MeshRenderer>();

            if (!isFloor)
            {
                go.transform.position += Vector3.up * apartmentHeight;
            }
            
            mf.sharedMesh = triangulator.CreateMesh(roomContour, null);
            var verts = mf.sharedMesh.vertices;

            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] = new Vector3(verts[i].x, verts[i].z, verts[i].y);
            }
            mf.sharedMesh.vertices = verts;
            
            mf.sharedMesh.uv = MathUtils.CreatePlaneUVs(mf.sharedMesh, maxDimensions);
            mf.sharedMesh.RecalculateNormals();
            mr.sharedMaterial = floorMat;
            return go;
        }
        private static GameObject GenerateWall(Wall wall, Triangulator triangulator, Vector2 maxDimensions, float apartmentHeight, Material wallMat)
        {
            GameObject wallGO = new GameObject("wall");
            MeshFilter wallMf = wallGO.AddComponent<MeshFilter>();
            MeshRenderer wallMr = wallGO.AddComponent<MeshRenderer>();

            float length = Vector2.Distance(wall.Begin, wall.End) / MeasurmentK;
            List<Vector2> wallContour = new List<Vector2>
            {
                new Vector2(-length / 2, -apartmentHeight / 2),
                new Vector2(-length / 2,  apartmentHeight / 2),
                new Vector2( length / 2,  apartmentHeight / 2),
                new Vector2( length / 2, -apartmentHeight / 2)
            };

            wallMf.sharedMesh = triangulator.CreateMesh(wallContour);
            wallMf.sharedMesh.RecalculateNormals();
            wallMf.sharedMesh.uv = MathUtils.CreatePlaneUVs(wallMf.sharedMesh, maxDimensions);
            wallMr.sharedMaterial = wallMat;
            wallGO.transform.position = wall.Center.XYtoXYZ() / MeasurmentK + Vector3.up * apartmentHeight / 2;
            wallGO.transform.rotation = Quaternion.Euler(0, wall.Rotation, 0);
            return wallGO;
        }
    }
}