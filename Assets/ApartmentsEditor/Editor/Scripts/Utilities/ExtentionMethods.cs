using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Foxsys.ApartmentEditor
{
    public static class ExtentionMethods
    {
        public static Vector2 RoundCoordsToInt(this Vector2 vec)
        {
            return new Vector2(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y));
        }
        public static Vector3 XYtoXYZ(this Vector2 vec)
        {
            return  new Vector3(vec.x, 0, vec.y);
        }
        public static Vector3 GetCorner(this Bounds bounds, CornerType type)
        {
            var center = bounds.center;
            var extents = bounds.extents;
            switch (type)
            {
                default:
                case CornerType.LeftDownBackward:
                    return center - extents;
                case CornerType.RightUpForward:
                    return center + extents;
                case CornerType.LeftDownForward:
                    return center + new Vector3(-extents.x, -extents.y, extents.z);
                case CornerType.LeftUpBackward:
                    return center + new Vector3(-extents.x, extents.y, -extents.z);
                case CornerType.LeftUpForward:
                    return center + new Vector3(-extents.x, extents.y, extents.z);
                case CornerType.RightDownBackward:
                    return center + new Vector3(extents.x, -extents.y, -extents.z);
                case CornerType.RightUpBackward:
                    return center + new Vector3(extents.x, extents.y, -extents.z);
                case CornerType.RightDownForward:
                    return center + new Vector3(extents.x, -extents.y, extents.z);
            }
        }
       
    }
    public enum CornerType
    {
        LeftUpForward,
        RightUpForward,
        LeftUpBackward,
        RightUpBackward,
        LeftDownForward,
        RightDownForward,
        LeftDownBackward,
        RightDownBackward
    }
}