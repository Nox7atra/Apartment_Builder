using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    public static class MathUtils
    {
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