using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Foxsys.ApartmentEditor
{
    [Serializable]
    public class Window : WallObject
    {
        [SerializeField]
        public float DistanceFromFloor;
        public static Window CreateOrGet(string name, float width, float height, float distanceFromFloor)
        {
            var pathsToDoors = PathsConfig.Instance.PathToWindows;

            var fullpath = Path.Combine(pathsToDoors, name + ".asset");
            Window window = AssetDatabase.LoadAssetAtPath<Window>(fullpath);
            if (window == null)
            {
                window = CreateInstance<Window>();
                window.Height = height;
                window.Width = width;
                window.DistanceFromFloor = distanceFromFloor;
                AssetDatabase.CreateAsset(window, fullpath);
                AssetDatabase.SaveAssets();
            }
            return window;
        }
    }
}