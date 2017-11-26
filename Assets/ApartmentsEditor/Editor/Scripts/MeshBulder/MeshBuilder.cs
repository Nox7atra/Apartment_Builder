using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foxsys.ApartmentEditor
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

               
         
                foreach (var wall in room.Walls)
                {
                    GameObject wallGO = GenerateWall(wall, triangulator, maxDimension, apartment.Height, apartment.WallThickness, apartment.WallMaterial);
                    wallGO.transform.SetParent(roomGo.transform);
                }

                Vector3 centroid = room.Centroid.XYtoXYZ();

                var contour = room.GetContour();
                for (var j = 0; j < contour.Count; j++)
                {
                    contour[j] = contour[j] / MeasurmentK;
                }

                GameObject floorGO = GenerateFloorRoof(contour, triangulator, centroid, apartment.Height, maxDimension, true, apartment.FloorMaterial);

                contour.Reverse();

                GameObject roofGO = GenerateFloorRoof(contour, triangulator, centroid, apartment.Height, maxDimension, false, apartment.FloorMaterial);

                floorGO.transform.SetParent(roomGo.transform);
                roofGO.transform.SetParent(roomGo.transform);

                roomGo.transform.SetParent(go.transform);
            }
            return go;
        }

        private static GameObject GenerateFloorRoof(List<Vector2> roomContour, Triangulator triangulator, Vector3 centroid, float apartmentHeight, Vector2 maxDimensions, bool isFloor, Material floorMat)
        {
            GameObject go = new GameObject(isFloor ? "floor" : "roof");
            MeshFilter mf = go.AddComponent<MeshFilter>();
            MeshRenderer mr = go.AddComponent<MeshRenderer>();

            go.transform.position = centroid / MeasurmentK;
       
            if (!isFloor)
            {
                go.transform.position += Vector3.up * apartmentHeight / MeasurmentK;
            }
            
            mf.sharedMesh = triangulator.CreateMesh(roomContour, null);
            var verts = mf.sharedMesh.vertices;

            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] = centroid / MeasurmentK - new Vector3(verts[i].x, verts[i].z, verts[i].y);
            }
            mf.sharedMesh.vertices = verts;
            
            mf.sharedMesh.uv = MathUtils.CreatePlaneUVs(mf.sharedMesh, maxDimensions);
            mf.sharedMesh.RecalculateNormals();
            mr.sharedMaterial = floorMat;
            go.transform.rotation = Quaternion.Euler(0, 180, 0);
            return go;
        }
        private static GameObject GenerateWall(Wall wall, Triangulator triangulator, Vector2 maxDimensions, float apartmentHeight, float wallThikness, Material wallMat)
        {
            GameObject wallGO = new GameObject("wall");
            MeshFilter wallMf = wallGO.AddComponent<MeshFilter>();
            MeshRenderer wallMr = wallGO.AddComponent<MeshRenderer>();

            float wallLength = (Vector2.Distance(wall.Begin, wall.End) / 2 + wallThikness) / MeasurmentK;
            float wallHeigth = apartmentHeight / MeasurmentK / 2;
            List<Vector2> wallContour = new List<Vector2>
            {
                new Vector2(-wallLength, -wallHeigth),
                new Vector2(-wallLength,  wallHeigth),
                new Vector2( wallLength,  wallHeigth),
                new Vector2( wallLength, -wallHeigth)
            };

            wallMf.sharedMesh = triangulator.CreateMesh(wallContour);
            wallMf.sharedMesh.RecalculateNormals();
            wallMf.sharedMesh.uv = MathUtils.CreatePlaneUVs(wallMf.sharedMesh, maxDimensions);
            wallMr.sharedMaterial = wallMat;

            var transform = wallGO.transform;
            transform.rotation = Quaternion.Euler(0, wall.Rotation, 0);
            transform.position = (wall.Center.XYtoXYZ()  + Vector3.up * apartmentHeight / 2 - transform.forward * wallThikness) / MeasurmentK;
            return wallGO;
        }
    }
}