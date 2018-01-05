using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Foxsys.ApartmentEditor
{
    public class PathsConfig : ScriptableObject
    {
        public string PathToModels = "";
        public string PathToSkins = "";
        public string PathToData = "";
        public string PathToMaterialPresets = "";
        public string PathToApartments
        {
            get { return Path.Combine(PathToData, "Apartments"); }
        }
        public string PathToDoors
        {
            get
            {
                var pathToDoors = Path.Combine(PathToData, "Doors");
                if (!Directory.Exists(pathToDoors))
                {
                    Directory.CreateDirectory(pathToDoors);
                }
                return pathToDoors;
            }
        }
        public string PathToWindows
        {
            get { return Path.Combine(PathToData, "Windows"); }
        }
        private static PathsConfig _Instance;

        public string GetPathByType(Type type)
        {
            if (type == typeof(Door))
            {
                return PathToDoors;
            }
            if (type == typeof(Apartment))
            {
                return PathToApartments;
            }
            if (type == typeof(Window))
            {
                return PathToWindows;
            }
            if (type == typeof(RoomMaterialPreset))
            {
                return PathToMaterialPresets;
            }
            Debug.Log("You don't have special path for objects of type " + type.ToString());
            return null;
        }
        public static PathsConfig Instance
        {
            get
            {
                if (_Instance == null)
                {
                    var path = Path.Combine(AssetFolder, AssetName);
                    _Instance = AssetDatabase.LoadAssetAtPath<PathsConfig>(path);

                    if (_Instance == null)
                    {
                        _Instance = CreateInstance<PathsConfig>();
                        AssetDatabase.CreateAsset(_Instance, path);
                    }
                }

                return _Instance;
            }
        }
        private const string AssetName = "Paths.asset";
        private const string AssetFolder = "Assets/ApartmentsEditor/Editor";
    }
}