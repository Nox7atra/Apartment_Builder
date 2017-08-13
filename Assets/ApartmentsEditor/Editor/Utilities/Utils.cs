using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    public static class MathUtils
    {
        public static bool IsPointInsideLineSegment(Vector2 point, Vector2 linePoint1, Vector2 linePoint2)
        {
            return Vector3.Distance(linePoint1, linePoint2) + float.Epsilon
                <= Vector3.Distance(point, linePoint1)
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
        public static bool IsPointInsideCountour(List<Vector2> contour, Vector2 point)
        {
            Vector2 p1, p2;
            bool inside = false;

            if (contour.Count < 3)
            {
                return inside;
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
    }
    public static class ExtentionMethods
    {
        public static Vector2 RoundCoordsToInt(this Vector2 vec)
        {
            return new Vector2(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y));
        }
    }
}