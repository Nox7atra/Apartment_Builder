using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{

    public class EarCuttingTriangulator : Triangulator
    {
        private LinkedList<int> _Ears;
        private LinkedList<int> _ConvexVerts;
        private CyclicalList<int> _CountourIndexes;
        private List<int> _Tris;
        private CyclicalList<Vector2> _Polygon;
        public override Mesh CreateMesh(List<Vector2> contour, List<List<Vector2>> holes = null)
        {
            InitPolygon(contour, holes);

            float sign = 0;
            
            for (int i = 0; i < contour.Count; i++)
            {
                Vector2 point1 = contour[i],
                    point2 = contour[(i + 1) % contour.Count];
                sign += (point2.x - point1.x) * (point2.y + point1.y);
            }

            bool isClockwisePolygon = sign < 0;

            InitConvexPoints(isClockwisePolygon);
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

        private void ClipEar( bool isClockwisePolygon)
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

                if (i == firstIndex || i == index || i == lastIndex)
                    continue;

                isEar = !MathUtils.IsPointInsideCountour(new List<Vector2>
                    {
                        _Polygon[firstIndex],
                        _Polygon[index],
                        _Polygon[lastIndex]
                    },
                    _Polygon[i]);
                if (!isEar)
                    break;
            }
            return isEar;
        }

        private void InitPolygon(List<Vector2> contour, List<List<Vector2>> holes = null)
        {
            _Polygon = new CyclicalList<Vector2>();
            _CountourIndexes = new CyclicalList<int>();
            for (int i = 0; i < contour.Count; i++)
            {
                _Polygon.Add(contour[i]);
                _CountourIndexes.Add(i);
            }
        }

        private void ValidateVert(int index, bool isClockwisePolygon)
        {
            if (!_Ears.Contains(index))
            {
                if (IsVertConvex(index, isClockwisePolygon))
                {
                    if (IsEar(index))
                    {
                        _Ears.AddLast(index % _Polygon.Count);
                    }
                }
            }
        }
        private void InitConvexPoints( bool isClockwisePolygon)
        {
            _ConvexVerts = new LinkedList<int>();
            for (int i = 0; i < _Polygon.Count; i++)
            {
                if (IsVertConvex( i, isClockwisePolygon))
                    _ConvexVerts.AddLast(i);
            }
        }

        private bool IsVertConvex(int vertNum, bool isClockwisePolygon)
        {
            int index = _CountourIndexes.FindIndex(x => x == vertNum);
            Vector2 point1 = _Polygon[_CountourIndexes[index - 1]],
                point2 = _Polygon[_CountourIndexes[index]],
                point3 = _Polygon[_CountourIndexes[index + 1]];
             return ((point1.x * (point2.y - point3.y) + point2.x * (point3.y - point1.y) +
                              point3.x * (point1.y - point2.y)) <= 0) ^ isClockwisePolygon;
        }
    }
}
