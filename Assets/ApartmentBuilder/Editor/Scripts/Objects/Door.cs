using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Foxsys.ApartmentBuilder
{
    [Serializable]
    public class Door : WallObject
    {
        public override List<Vector2> GetHole(Vector2 position, Vector2 wallBegin, Vector2 wallEnd)
        {
            var hole = new List<Vector2>();
            float left = CalculateOffset(position, wallBegin, wallEnd, true),
                right = CalculateOffset(position, wallBegin, wallEnd, false);
            hole.Add(new Vector2(left,  0));
            hole.Add(new Vector2(right, 0));
            hole.Add(new Vector2(right, Height));
            hole.Add(new Vector2(left, Height));
            return hole;
        }
    }
}