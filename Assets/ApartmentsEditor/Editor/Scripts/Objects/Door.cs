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
    }
}