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
        public static bool IsLinesIntersects(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {

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

            return doIntersect;
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
}