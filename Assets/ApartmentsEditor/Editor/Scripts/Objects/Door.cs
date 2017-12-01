using System;
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
            var fullpath = Path.Combine(PathsConfig.Instance.PathToApartments, "/Doors/" + name + ".asset");
            Door door = AssetDatabase.LoadAssetAtPath<Door>(fullpath);
            if (door == null)
            {
                door = CreateInstance<Door>();
                door.Height = height;
                door.Width = width;
                AssetDatabase.CreateAsset(door, fullpath);
            }
            return door;
        }
    }
}