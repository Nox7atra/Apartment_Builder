using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    public class PathsConfig : ScriptableObject
    {
        public string PathToApartments = "";
        public string PathToModels = "";

  

        private static PathsConfig _Instance;

        public static PathsConfig Instance
        {
            get
            {
                if (_Instance == null)
                {
                    var path = Path.Combine(ASSET_FOLDER, ASSET_NAME);
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
        private const string ASSET_NAME = "Paths.asset";
        private const string ASSET_FOLDER = "Assets/Editor/ApartmentsEditor";
    }
}