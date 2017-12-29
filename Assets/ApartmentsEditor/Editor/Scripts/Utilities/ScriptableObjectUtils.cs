using System.Collections;
using System.Collections.Generic;
using System.IO;
using Foxsys.ApartmentEditor;
using UnityEditor;
using UnityEngine;

namespace Foxsys.ApartmentEditor
{
    public static class ScriptableObjectUtils
    {
        public static T CreateOrGet<T>(string name) where T : ScriptableObject
        {
            var path = PathsConfig.Instance.GetPathByType(typeof(T));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var fullpath = Path.Combine(path, name + ".asset");
            T so = AssetDatabase.LoadAssetAtPath<T>(fullpath);
            if (so == null)
            {
                so = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(so, fullpath);
                AssetDatabase.SaveAssets();
            }
            return so;
        }
    }
}