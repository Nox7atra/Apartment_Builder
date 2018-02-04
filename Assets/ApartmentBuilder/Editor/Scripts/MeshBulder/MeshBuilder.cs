using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Enumerable = System.Linq.Enumerable;
using Object = UnityEngine.Object;

namespace Foxsys.ApartmentBuilder
{
    public static class MeshBuilder
    {
        private const float MeasurmentK = 100;

        private static string _ApartmentName;
        public static GameObject GenerateApartmentMesh(Apartment apartment)
        {
            _ApartmentName = apartment.name;
            GameObject go = new GameObject(_ApartmentName);
            ClearOld();
            var rooms = GenerateRooms(apartment, new EarCuttingTriangulator());

            rooms.transform.SetParent(go.transform);
            AssetDatabase.Refresh();
            return go;
        }

        private static void ClearOld()
        {
            var path = Path.Combine(PathsConfig.Instance.PathToModels, _ApartmentName);
            if(Directory.Exists(path))
                Directory.Delete(path, true);
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
            GameObject roomGO = new GameObject(room.name + (inside ? "" : "_outside"));

            float height = inside ? apartment.Height : apartment.Height * 1.1f;
            room.MakeClockwiseOrientation();

            var wallContour =  inside ? room.GetContourWithThickness() : room.Contour.Select(x => x.Position).ToList();

            if(!inside)
                wallContour.Reverse();
  

            for (int j = 0, count = wallContour.Count; j < count; j++)
            {
                Vector2 begin = wallContour[j], end = wallContour[(j + 1) % count];
                var holes = GetHoles(room, begin, end, inside);
                GameObject wallGO = GenerateWall(
                    begin,
                    end,
                    (end + begin).XYtoXYZ() / 2,
                    holes,
                    triangulator,
                    height,
                    inside ? room.MaterialPreset.WallMaterial : apartment.OutsideMaterial,
                    j);

                SaveMesh(wallGO.GetComponent<MeshFilter>().sharedMesh, room.name, inside);
                wallGO.transform.SetParent(roomGO.transform);
            }

            Vector3 centroid = room.Centroid.XYtoXYZ();

            GameObject floorGO = GenerateFloor(
                wallContour,
                triangulator, 
                centroid, 
                inside ? room.MaterialPreset.FloorMaterial : apartment.OutsideMaterial);
            SaveMesh(floorGO.GetComponent<MeshFilter>().sharedMesh, room.name, inside);
            GameObject roofGO = PrepareRoof(
                floorGO,
                height,
                inside ? room.MaterialPreset.RoofMaterial : apartment.OutsideMaterial);
            SaveMesh(roofGO.GetComponent<MeshFilter>().sharedMesh, room.name, inside);
            if (!inside)
                GameObject.DestroyImmediate(floorGO);
            else 
                floorGO.transform.SetParent(roomGO.transform);
            roofGO.transform.SetParent(roomGO.transform);

            return roomGO;
        }

        private static List<List<Vector2>> GetHoles(Room room, Vector2 begin, Vector2 end, bool isInside)
        {
            List<List<Vector2>> holes = new List<List<Vector2>>();
            var normal = new Vector2(begin.y - end.y, end.x - begin.x).normalized;
            foreach (var rm in room.ParentApartment.Rooms)
            {
                foreach (var wallObject in rm.WallObjects)
                {
                    var position = wallObject.GetVector2Position();
                    if (MathUtils.IsPointInsideLineSegment(position - (isInside ? normal * room.WallThickness : Vector2.zero), begin, end))
                    {
                        holes.Add(wallObject.GetHole(position, begin, end));
                        
                    }
                }

            }
            return holes;
        }
        private static GameObject PrepareRoof(GameObject floor, float apartmentHeight, Material roofMat)
        {
            GameObject roofGO = GameObject.Instantiate(floor);
            roofGO.name = "roof";
            roofGO.transform.position += Vector3.up * apartmentHeight / MeasurmentK;
            var mf = roofGO.GetComponent<MeshFilter>();
            var mr = roofGO.GetComponent<MeshRenderer>();
            var mesh = Object.Instantiate(mf.sharedMesh);
     

            var tris = mesh.triangles;
            for (int i = 0; i < tris.Length; i+=3)
            {
                var tmp = tris[i];
                tris[i] = tris[i + 2];
                tris[i + 2] = tmp;
            }
            mesh.triangles = tris;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.name = roofGO.name;
            mf.sharedMesh = mesh;
            mr.sharedMaterial = roofMat;
            return roofGO;
        }
        private static GameObject GenerateFloor(
            List<Vector2> roomContour, 
            Triangulator triangulator, 
            Vector3 centroid, 
            Material floorMat)
        {
            GameObject go = new GameObject("floor");
            MeshFilter mf = go.AddComponent<MeshFilter>();
            MeshRenderer mr = go.AddComponent<MeshRenderer>();

            go.transform.position = centroid / MeasurmentK;

            var mesh = triangulator.CreateMesh(roomContour, null);
            var verts = mesh.vertices;

            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] = (centroid  - new Vector3(verts[i].x, verts[i].z, verts[i].y)) / MeasurmentK;
            }
            mesh.vertices = verts;

            mesh.uv = MathUtils.CreatePlaneUVs(mesh);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.name = go.name;
            mf.sharedMesh = mesh;
            mr.sharedMaterial = floorMat;
            go.transform.rotation = Quaternion.Euler(0, 180, 0);
            return go;
        }
        private static GameObject GenerateWall(
            Vector3 begin,
            Vector3 end,
            Vector3 center,
            List<List<Vector2>> holes,
            Triangulator triangulator,
            float apartmentHeight,
            Material wallMat,
            int index)
        {
            GameObject wallGO = new GameObject("wall" + index);
            MeshFilter wallMf = wallGO.AddComponent<MeshFilter>();
            MeshRenderer wallMr = wallGO.AddComponent<MeshRenderer>();

            float wallLength = (Vector2.Distance(begin, end) / 2) / MeasurmentK;
            float wallHeight = apartmentHeight / MeasurmentK;
            List<Vector2> wallContour = new List<Vector2>
            {
                new Vector2(-wallLength, 0),
                new Vector2(-wallLength,  wallHeight),
                new Vector2( wallLength,  wallHeight),
                new Vector2( wallLength, 0)
         
            };
            for(int iter = 0; iter < holes.Count; iter++)
            {
                var hole = holes[iter];
                for (int i = 0; i < hole.Count; i++)
                {
                    hole[i] = hole[i] / MeasurmentK;
                }
                if (!hole.TrueForAll(v => v.y != 0))      //is Door
                {
                    ReorderHolePoints(hole);

                    int insertIndex = -1;
                    for (int j = 0, count = wallContour.Count; j < count; j++)
                    {
                        Vector2 p1 = wallContour[j], p2 = wallContour[(j + 1) % count];
          
                        if ( MathUtils.IsPointInsideLineSegment(hole[0], p1, p2))
                        {

                            insertIndex = j + 1;
                            break;
                        }
                    }

                    for (int j = 0, count = wallContour.Count; j < count; j++)
                    {
                        for (int k = 0; k < hole.Count; k++)
                        {
                            if (hole[k] == wallContour[j])
                            {
                                hole.RemoveAt(k);
                                wallContour.RemoveAt(j);
                                insertIndex--;
                                j--;
                                k--;
                            }
                        }
                    }
                    wallContour.InsertRange(insertIndex, hole);
                    holes.Remove(hole);
                    iter--;
                }
            
            }
            var mesh = triangulator.CreateMesh(wallContour, holes);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.uv = MathUtils.CreatePlaneUVs(mesh);
            mesh.name = wallGO.name;
            wallMf.sharedMesh = mesh;
            wallMr.sharedMaterial = wallMat;

            var transform = wallGO.transform;
            transform.rotation = Quaternion.Euler(0, -Vector2.SignedAngle(Vector2.right, end - begin), 0);
            transform.position = center / MeasurmentK;
            return wallGO;
        }

        private static void ReorderHolePoints(List<Vector2> hole)
        {
            var bot = hole.Where(v => Math.Abs(v.y) <= 0).ToList();
            bot.Sort((v1, v2) => v1.x > v2.x ? -1 : 1);
            var top = hole.Where(v =>Math.Abs(v.y) > 0).ToList();
            top.Sort((v1, v2) => v1.x > v2.x ? -1 : 1);
            hole[0] = bot[0];
            hole[1] = top[0];
            hole[2] = top[1];
            hole[3] = bot[1];
        }
        private static void SaveMesh(Mesh mesh, string roomName, bool isIndide)
        {
            var path = Path.Combine(PathsConfig.Instance.PathToModels, _ApartmentName);
            path = Path.Combine(path, isIndide? roomName : roomName + "/Outside/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = Path.Combine(path, mesh.name + ".asset");
            AssetDatabase.CreateAsset(mesh,path);
        }
    }
}