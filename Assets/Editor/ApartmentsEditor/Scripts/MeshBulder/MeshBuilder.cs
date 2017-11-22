using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    public static class MeshBuilder
    {
        
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
                    contour[j] = contour[j] / 100;
                    Debug.Log(contour[j]);
                }

                foreach (var wall in room.Walls)
                {
                    GameObject wallGO = GenerateWall(wall, triangulator, maxDimension, apartment.Height);
                    wallGO.transform.SetParent(roomGo.transform);
                }
                GameObject floorGO = GenerateFloorRoof(contour, triangulator, apartment.Height, maxDimension, true);
                contour.Reverse();
                GameObject roofGO = GenerateFloorRoof(contour, triangulator, apartment.Height, maxDimension, false);
                roofGO.transform.SetParent(roomGo.transform);
                floorGO.transform.SetParent(roomGo.transform);

                roomGo.transform.SetParent(go.transform);
            }
            return go;
        }

        private static GameObject GenerateFloorRoof(List<Vector2> roomContour, Triangulator triangulator, float apartmentHeight, Vector2 maxDimensions, bool isFloor)
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
            
            return go;
        }
        private static GameObject GenerateWall(Wall wall, Triangulator triangulator, Vector2 maxDimensions, float apartmentHeight)
        {
            GameObject wallGO = new GameObject("wall");
            MeshFilter wallMf = wallGO.AddComponent<MeshFilter>();
            MeshRenderer wallMr = wallGO.AddComponent<MeshRenderer>();

            float length = Vector2.Distance(wall.Begin, wall.End) / 100;
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
            wallGO.transform.position = wall.Center.XYtoXYZ() / 100 + Vector3.up * apartmentHeight / 2;
            wallGO.transform.rotation = Quaternion.Euler(0, wall.Rotation, 0);
            return wallGO;
        }
    }
}