using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Foxsys.ApartmentEditor
{
    [Serializable]
    public class Door : WallObject
    {
        public static Door CreateOrGet(string name, float width, float height)
        {
            var pathsToDoors = PathsConfig.Instance.PathToDoors;
          
            var fullpath = Path.Combine(pathsToDoors, name + ".asset");
            Door door = AssetDatabase.LoadAssetAtPath<Door>(fullpath);
            if (door == null)
            {
                door = CreateInstance<Door>();
                door.Height = height;
                door.Width = width;
                AssetDatabase.CreateAsset(door, fullpath);
                AssetDatabase.SaveAssets();
            }
            return door;
        }

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