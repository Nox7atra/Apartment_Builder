using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Foxsys.ApartmentEditor
{
    public class RoomMaterialPreset : ScriptableObject
    {
        [MenuItem("Window/ApartmentBuilder/Material Presets/Create default")]
        public static void CreateDefault()
        {
            ScriptableObjectUtils.CreateOrGet<RoomMaterialPreset>("default");
        }


        public Color Color;
        public Material FloorMaterial;
        public Material RoofMaterial;
        public Material WallMaterial;
    }
}