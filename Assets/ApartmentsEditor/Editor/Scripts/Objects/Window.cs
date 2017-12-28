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

        public override List<Vector2> GetHole(Vector2 position, Vector2 wallBegin, Vector2 wallEnd)
        {
            var hole = new List<Vector2>();
            float left = CalculateOffset(position, wallBegin, wallEnd, true),
                right = CalculateOffset(position, wallBegin, wallEnd, false);
            hole.Add(new Vector2(left, DistanceFromFloor));
            hole.Add(new Vector2(right, DistanceFromFloor));
            hole.Add(new Vector2(right, Height + DistanceFromFloor));
            hole.Add(new Vector2(left, Height + DistanceFromFloor));
            return hole;
        }
    }
}