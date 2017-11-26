using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Foxsys.ApartmentEditor
{

    public class EarCuttingTriangulator : Triangulator
    {
        private LinkedList<int> _Ears;
        private LinkedList<int> _ConvexVerts;
        private CyclicalList<int> _CountourIndexes;
        private List<int> _Tris;
        private CyclicalList<Vector2> _Polygon;

        public List<Vector2> Polygon
        {

            get { return _Polygon; }
        }
        public override Mesh CreateMesh(List<Vector2> contour, List<List<Vector2>> holes = null)
        {
            bool isClockwisePolygon = InitPolygon(contour, holes);
            
            CreateEars();

            _Tris = new List<int>();
    
            while (_Ears.Count != 0 && _CountourIndexes.Count > 3)
            {
                ClipEar(isClockwisePolygon);
            }

            for (int i = 0; i < _CountourIndexes.Count; i++)
            {
                _Tris.Add(_CountourIndexes[i]);
            }
            Mesh mesh = new Mesh();
            mesh.vertices = Array.ConvertAll(_Polygon.ToArray(), x => (Vector3) x );
            mesh.triangles = _Tris.ToArray();
            mesh.RecalculateNormals();
            return mesh;
        }

        private void ClipEar(bool isClockwisePolygon)
        {
            var ear = _Ears.First;
            int index = ear.Value;
            var posInContour = _CountourIndexes.FindIndex(x => x == ear.Value);

            int first = _CountourIndexes[posInContour - 1];
            int last = _CountourIndexes[posInContour + 1];
            _Tris.Add(first);
            _Tris.Add(index);
            _Tris.Add(last);

            _Ears.Remove(ear);
            _CountourIndexes.Remove(ear.Value);
            
            ValidateVert(first, isClockwisePolygon);
            ValidateVert(last, isClockwisePolygon);
        }
        private void CreateEars()
        {
            _Ears = new LinkedList<int>();
            foreach (int convexVert in _ConvexVerts)
            {
                if (IsEar(convexVert))
                {
                    _Ears.AddLast(convexVert);
                }
            }
        }

        private bool IsEar(int index)
        {
            bool isEar = true;
            
            int indexInContour = _CountourIndexes.FindIndex(x => x == index);
            for (int i = 0; i < _CountourIndexes.Count; i++)
            {
                int firstIndex = _CountourIndexes[indexInContour - 1];
                int lastIndex = _CountourIndexes[indexInContour + 1];

                if (_Polygon[i] == _Polygon[firstIndex] || _Polygon[i] == _Polygon[index] || _Polygon[i] == _Polygon[lastIndex])
                    continue;

                isEar = !MathUtils.IsPointInsideTriangle(
                        _Polygon[firstIndex],
                        _Polygon[index],
                        _Polygon[lastIndex],
                        _Polygon[i]);
                if (!isEar)
                    break;
            }
            return isEar;
        }

        private bool InitPolygon(List<Vector2> contour, List<List<Vector2>> holes = null)
        {
            _Polygon = new CyclicalList<Vector2>();
            _CountourIndexes = new CyclicalList<int>();

            
            for (int i = 0; i < contour.Count; i++)
            {
                _Polygon.Add(contour[i]);

                _CountourIndexes.Add(i);
            }
            bool isClockwisePolygon = MathUtils.IsContourClockwise(contour);

            if (holes != null)
            {
                holes.Sort((item1, item2) => item2.OrderByDescending(i => i.x).FirstOrDefault().x.CompareTo(item1.OrderByDescending(j => j.x).FirstOrDefault().x));
                foreach (List<Vector2> hole in holes)
                {
                    Vector2? maxXpoint = null;
                    float maxX = float.MinValue;
                    List<Vector2> holeVerts = hole;

                    bool isHoleClockwise = MathUtils.IsContourClockwise(holeVerts);

                    if (!(isClockwisePolygon ^ isHoleClockwise))
                    {
                        holeVerts.Reverse();
                    }
                    for (int j = 0, count = holeVerts.Count; j < count; j++)
                    {
                        if (maxX < holeVerts[j].x)
                        {
                            maxXpoint = holeVerts[j];
                            maxX = holeVerts[j].x;
                        }
                    }
                    if (maxXpoint.HasValue)
                    {
                        holeVerts = MathUtils.ReorderContour(holeVerts, maxXpoint.Value);
                        int index = FindMutuallyVisibleVertIndex(maxXpoint.Value);
                        if (index >= 0)
                        {
                            for (int k = 0; k < holeVerts.Count + 2; k++)
                            {
                                _CountourIndexes.Add(_CountourIndexes.Count);
                            }
                            _Polygon.InsertRange(index + 1, holeVerts);
                            _Polygon.Insert(index + holeVerts.Count + 1, holeVerts[0]);
                            _Polygon.Insert(index + holeVerts.Count + 2, _Polygon[index]);
                        }
                    }
                }
            }

            InitConvexPoints(isClockwisePolygon);

            return isClockwisePolygon;
        }
        private int FindMutuallyVisibleVertIndex(Vector2 vert)
        {
            Vector2 minIntersection = new Vector2();
            int startIndex = -1;
            float minDistance = float.MaxValue;
            for (int i = 0; i < _Polygon.Count; i++)
            {
                Vector2 p1 = _Polygon[i], p2 = _Polygon[(i + 1) % _Polygon.Count];
                var intersection = MathUtils.LineSegmentsIntersection(p1, p2, vert, new Vector2(100000, vert.y));
                if (intersection.HasValue)
                {
                    float distance = Vector2.Distance(vert, intersection.Value);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        startIndex = i;
                        minIntersection = intersection.Value;
                    }
                }
            }

            int index = _Polygon.IndexOf(minIntersection);

            if (index >= 0)
            {
                return index;
            }
            index = _Polygon[startIndex].x >= _Polygon[(startIndex + 1) % _Polygon.Count].x
                ? startIndex
                : (startIndex + 1) % _Polygon.Count;
            
            List<Vector2> triangle = new List<Vector2>() {vert, minIntersection, _Polygon[index]};
            List<Vector2> insidePoints = new List<Vector2>();
            for (int i = 0; i < _Polygon.Count; i++)
            {
                if (i != index)
                {
                    if (MathUtils.IsPointInsideCountour(triangle, _Polygon[i]))
                    {
                        insidePoints.Add(_Polygon[i]);
                    }
                }
            }
            if (insidePoints.Count > 0)
            {
                float minAngle = float.MaxValue;
                Vector2 pointWithMinAngle = insidePoints[0];
                for (int i = 0; i < insidePoints.Count; i++)
                {
                    var angle = Vector2.Angle(new Vector2(100000, vert.y) - vert, insidePoints[i] - vert);
                    if (minAngle > angle)
                    {
                        minAngle = angle;
                        pointWithMinAngle = insidePoints[i];
                    }
                }
                return _Polygon.IndexOf(pointWithMinAngle);
            }
            return index;
        }
        private void ValidateVert(int index, bool isClockwisePolygon)
        {
            bool isEar = IsEar(index);
            if (!_Ears.Contains(index))
            {
                if (IsVertConvex(index, isClockwisePolygon))
                {
                    if (isEar)
                    {
                        _Ears.AddLast(index % _Polygon.Count);
                    }
                }
            }
            else
            {
                if (!isEar)
                {
                    _Ears.Remove(index);
                }
            }
        }
        private void InitConvexPoints(bool isClockwisePolygon)
        {
            _ConvexVerts = new LinkedList<int>();
            for (int i = 0; i < _Polygon.Count; i++)
            {
                if (IsVertConvex(i, isClockwisePolygon))
                    _ConvexVerts.AddLast(i);
            }
        }

        private bool IsVertConvex(int vertNum, bool isClockwisePolygon)
        {
            int index = _CountourIndexes.FindIndex(x => x == vertNum);
            Vector2 point1 = _Polygon[_CountourIndexes[index - 1]] / 1000,
                point2 = _Polygon[_CountourIndexes[index]] / 1000,
                point3 = _Polygon[_CountourIndexes[index + 1]] / 1000;
             return (point1.x * (point2.y - point3.y) - point1.y * (point2.x - point3.x) +
                             (point2.x * point3.y - point3.x * point2.y) <= 0) ^ isClockwisePolygon;
        }

    }
}
