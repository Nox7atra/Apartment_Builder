using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Foxsys.ApartmentEditor
{
    public static class MathUtils
    {
        public static bool IsPointInsideTriangle(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p)
        {
            var A = 1 / 2f * (-p1.y * p2.x + p0.y * (-p1.x + p2.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y);
            var sign = A < 0 ? -1 : 1;
            var s = (p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * p.x + (p0.x - p2.x) * p.y) * sign;
            var t = (p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * p.x + (p1.x - p0.x) * p.y) * sign;
            return s > 0 && t > 0 && (s + t) < 2 * A * sign;
        }

        public static List<Vector2> MergeContours(List<Vector2> contour1, List<Vector2> contour2)
        {
            List<Vector2> result = null;
            for (int i = 0, count1 = contour1.Count; i < count1; i++)
            {
                for (int j = 0, count2 = contour2.Count; j < count2; j++)
                {
                    MathUtils.IsPointInsideCountour(contour1, contour2[i]);
                }
            }
            return result;
        }
        public static bool IsContourClockwise(List<Vector2> contour)
        {
            float sign = 0;

            for (int i = 0; i < contour.Count; i++)
            {
                Vector2 point1 = contour[i],
                    point2 = contour[(i + 1) % contour.Count];
                sign += (point2.x - point1.x) * (point2.y + point1.y);
            }

            return sign < 0;
        }
        public static bool IsPointInsideLineSegment(Vector2 point, Vector2 linePoint1, Vector2 linePoint2)
        {
            return Vector3.Distance(linePoint1, linePoint2) + float.Epsilon
                >= Vector3.Distance(point, linePoint1)
                + Vector3.Distance(point, linePoint2);
        }
        public static float DistanceFromPointToLine(Vector2 point, Vector2 linePoint1, Vector2 linePoint2)
        {
            return Mathf.Abs(
                (linePoint2.y - linePoint1.y)
                * point.x
                - (linePoint2.x - linePoint1.x)
                * point.y
                + linePoint2.x * linePoint1.y
                - linePoint2.y * linePoint1.x)
                / (linePoint2 - linePoint1).sqrMagnitude;
        }
        public static Vector3 PointProjectionToOnLine(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
        {
            float t = ((point.x - linePoint1.x) * (linePoint2.x - linePoint1.x)
                     + (point.y - linePoint1.y) * (linePoint2.y - linePoint1.y)
                     + (point.x - linePoint1.z) * (linePoint2.z - linePoint1.z))
                     / (linePoint2 - linePoint1).sqrMagnitude;
            return new Vector3(
                linePoint1.x + (linePoint2.x - linePoint1.x) * t,
                linePoint1.y + (linePoint2.y - linePoint1.y) * t,
                linePoint1.z + (linePoint2.z - linePoint1.z) * t);
        }
        public static Vector2? LineSegmentsIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            Vector2? result = null;

            Vector2 a = p2 - p1;
            Vector2 b = p3 - p4;
            Vector2 c = p1 - p3;

            float alphaNumerator = b.y * c.x - b.x * c.y;
            float alphaDenominator = a.y * b.x - a.x * b.y;
            float betaNumerator = a.x * c.y - a.y * c.x;
            float betaDenominator = a.y * b.x - a.x * b.y;

            bool doIntersect = true;

            if (alphaDenominator == 0 || betaDenominator == 0)
            {
                doIntersect = false;
            }
            else
            {

                if (alphaDenominator > 0)
                {
                    if (alphaNumerator < 0 || alphaNumerator > alphaDenominator)
                    {
                        doIntersect = false;
                    }
                }
                else if (alphaNumerator > 0 || alphaNumerator < alphaDenominator)
                {
                    doIntersect = false;
                }

                if (doIntersect &&  betaDenominator > 0) {
                    if (betaNumerator < 0 || betaNumerator > betaDenominator)
                    {
                        doIntersect = false;
                    }
                } else if (betaNumerator > 0 || betaNumerator < betaDenominator)
                {
                    doIntersect = false;
                }
            }

            if (doIntersect)
            {

                result = p1 + alphaNumerator * (p2 - p1) / alphaDenominator;
            }
            return result;
        }
        public static Vector2? LinesInterseciton(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            float A1 = p2.y - p1.y, A2 = p4.y - p3.y, B1 = p1.x - p2.x, B2 = p3.x - p4.x;
            float det = A1 * B2 - B1 * A2;

            if (det == 0)
                return null;

            float C1 = A1 * p1.x + B1 * p1.y, C2 = A2 * p3.x + B2 * p3.y;

            return new Vector2(
                (B2 * C1 - B1 * C2) / det,
                (A1 * C2 - A2 * C1) / det
            );
        }
        public static bool IsPointInsideCountour(List<Vector2> contour, Vector2 point)
        {
            Vector2 p1, p2;
            bool inside = false;

            if (contour.Count < 3)
            {
                return false;
            }

            Vector2 oldPoint = new Vector2(contour[contour.Count - 1].x, contour[contour.Count - 1].y);

            for (int i = 0; i < contour.Count; i++)
            {
                Vector2 newPoint = new Vector2(contour[i].x, contour[i].y);

                if (newPoint.x > oldPoint.x)
                {
                    p1 = oldPoint;
                    p2 = newPoint;
                }
                else
                {
                    p1 = newPoint;
                    p2 = oldPoint;
                }

                if ((newPoint.x < point.x) == (point.x <= oldPoint.x) &&
                   (point.y - p1.y) * (p2.x - p1.x) < (p2.y - p1.y) * (point.x - p1.x))
                {
                    inside = !inside;
                }

                oldPoint = newPoint;
            }

            return inside;
        }

        public static List<Vector2> ReorderContour(List<Vector2> contour, Vector2 firstItem)
        {
            List<Vector2> result = new List<Vector2>();
            int offset = contour.IndexOf(firstItem);
            for (int i = 0; i < contour.Count; i++)
            {
                result.Add(contour[(i + offset) % contour.Count]);
            }
            return result;
        }
        public static Vector2[] CreatePlaneUVs(Mesh mesh)
        {
            try
            {
                Vector3[] verts = mesh.vertices;
                Vector2[] uvs = new Vector2[verts.Length];
                Vector3 minPoint = GetPointWithMinCords(verts);
                Vector3 maxPoint = GetPointWithMaxCords(verts);
                for (int i = 0; i < uvs.Length; i++)
                {
                    uvs[i] = GetPlaneUVPoint(minPoint, maxPoint, verts[i], MAX_DIMENSIONS);
                }

                return uvs;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                throw e;
            }
        }
        private static Vector2 GetPlaneUVPoint(Vector3 min, Vector3 max, Vector3 vert, Vector2 maxDimensions)
        {
            Vector2 uv;
            if (min.x == max.x)
            {
                uv = new Vector2(
                    1 - (vert.y - max.y) / (min.y - max.y) * (max.x - min.x) / maxDimensions.x,
                    1 - (vert.z - max.z) / (min.z - max.z) * (max.y - min.y) / maxDimensions.y);
            }
            else if (min.y == max.y)
            {
                uv = new Vector2(
                    1 - (vert.x - max.x) / (min.x - max.x) * (max.x - min.x) / maxDimensions.x,
                    1 - (vert.z - max.z) / (min.z - max.z) * (max.z - min.z) / maxDimensions.y);
            }
            else
            {
                uv = new Vector2(
                    1 - (vert.x - max.x) / (min.x - max.x) * (max.x - min.x) / maxDimensions.x,
                    1 - (vert.y - max.y) / (min.y - max.y) * (max.y - min.y) / maxDimensions.y);
            }
            return uv;
        }
        private static Vector3 GetPointWithMinCords(Vector3[] points)
        {
            Vector3 minPoint = new Vector3(
                float.MaxValue,
                float.MaxValue,
                float.MaxValue);
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].x < minPoint.x)
                {
                    minPoint.x = points[i].x;
                }
                if (points[i].y < minPoint.y)
                {
                    minPoint.y = points[i].y;
                }
                if (points[i].z < minPoint.z)
                {
                    minPoint.z = points[i].z;
                }
            }
            return minPoint;
        }
        private static Vector3 GetPointWithMaxCords(Vector3[] points)
        {
            Vector3 maxPoint = new Vector3(
                float.MinValue,
                float.MinValue,
                float.MinValue);
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].x > maxPoint.x)
                {
                    maxPoint.x = points[i].x;
                }
                if (points[i].y > maxPoint.y)
                {
                    maxPoint.y = points[i].y;
                }
                if (points[i].z > maxPoint.z)
                {
                    maxPoint.z = points[i].z;
                }
            }
            return maxPoint;
        }
        private static readonly Vector2 MAX_DIMENSIONS = new Vector2(100, 100);
    }
}