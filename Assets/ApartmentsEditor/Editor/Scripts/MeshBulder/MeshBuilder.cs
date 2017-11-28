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
          
            var rooms = apartment.Rooms;
            foreach (var room in rooms)
            {
                var roomGo = GenerateRoom(room, triangulator);
              
                roomGo.transform.SetParent(go.transform);

                if (apartment.IsGenerateOutside)
                {
                    var outside = GenerateRoom(room, triangulator, false);
                    outside.transform.SetParent(roomGo.transform);
                }
            }
            return go;
        }

        private static GameObject GenerateRoom(Room room, Triangulator triangulator, bool inside = true)
        {
            var apartment = room.ParentApartment;
            Vector2 maxDimension = room.ParentApartment.Dimensions.size;
            GameObject roomGO = new GameObject(inside ? "Room" : "Outside");

            float height = inside ? apartment.Height : apartment.Height * 1.1f;
            room.MakeClockwiseOrientation();

            var wallContour =  inside ? room.GetContourWithThickness() : room.GetContour();

            if(!inside)
                wallContour.Reverse();
            
            for (int j = 0, count = wallContour.Count; j < count; j++)
            {
                Vector2 begin = wallContour[j], end = wallContour[(j + 1) % count];
                GameObject wallGO = GenerateWall(
                    begin,
                    end,
                    (end + begin).XYtoXYZ() / 2,
                    triangulator,
                    maxDimension,
                    height,
                    apartment.WallMaterial);
                wallGO.transform.SetParent(roomGO.transform);
            }

            Vector3 centroid = room.Centroid.XYtoXYZ();

            GameObject floorGO = GenerateFloorRoof(wallContour, triangulator, centroid, maxDimension, apartment.FloorMaterial);
           
            GameObject roofGO = PrepareRoof(floorGO, height);

            if (!inside)
                GameObject.DestroyImmediate(floorGO);
            else 
                floorGO.transform.SetParent(roomGO.transform);
            roofGO.transform.SetParent(roomGO.transform);

            return roomGO;
        }
        private static GameObject PrepareRoof(GameObject floor, float apartmentHeight)
        {
            GameObject roofGO = GameObject.Instantiate(floor);
            roofGO.name = "roof";
            roofGO.transform.position += Vector3.up * apartmentHeight / MeasurmentK;
            var mf = roofGO.GetComponent<MeshFilter>();
            var mesh = Object.Instantiate(mf.sharedMesh);
            mf.sharedMesh = mesh;

            var tris = mesh.triangles;
            for (int i = 0; i < tris.Length; i+=3)
            {
                var tmp = tris[i];
                tris[i] = tris[i + 2];
                tris[i + 2] = tmp;
            }
            mesh.triangles = tris;
            mesh.RecalculateNormals();
            return roofGO;
        }
        private static GameObject GenerateFloorRoof(List<Vector2> roomContour, Triangulator triangulator, Vector3 centroid, Vector2 maxDimensions, Material floorMat)
        {
            GameObject go = new GameObject("floor" );
            MeshFilter mf = go.AddComponent<MeshFilter>();
            MeshRenderer mr = go.AddComponent<MeshRenderer>();

            go.transform.position = centroid / MeasurmentK;
            
            mf.sharedMesh = triangulator.CreateMesh(roomContour, null);
            var verts = mf.sharedMesh.vertices;

            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] = (centroid  - new Vector3(verts[i].x, verts[i].z, verts[i].y)) / MeasurmentK;
            }
            mf.sharedMesh.vertices = verts;
            
            mf.sharedMesh.uv = MathUtils.CreatePlaneUVs(mf.sharedMesh, maxDimensions);
            mf.sharedMesh.RecalculateNormals();
            mr.sharedMaterial = floorMat;
            go.transform.rotation = Quaternion.Euler(0, 180, 0);
            return go;
        }
        private static GameObject GenerateWall(
            Vector3 begin,
            Vector3 end,
            Vector3 center,
            Triangulator triangulator,
            Vector2 maxDimensions, 
            float apartmentHeight,
            Material wallMat)
        {
            GameObject wallGO = new GameObject("wall");
            MeshFilter wallMf = wallGO.AddComponent<MeshFilter>();
            MeshRenderer wallMr = wallGO.AddComponent<MeshRenderer>();

            float wallLength = (Vector2.Distance(begin, end) / 2) / MeasurmentK;
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
            transform.rotation = Quaternion.Euler(0, -Vector2.SignedAngle(Vector2.right, end - begin), 0);
            transform.position = (center + Vector3.up * apartmentHeight / 2) / MeasurmentK;
            return wallGO;
        }
    }
}