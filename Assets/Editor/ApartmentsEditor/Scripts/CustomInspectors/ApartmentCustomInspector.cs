using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Nox7atra.ApartmentEditor
{
    [CustomEditor(typeof(Apartment))]
    public class ApartmentCustomInspector : Editor
    {
        private Apartment _ThisApartment;

        private Rect _Dimensions;

        private void OnEnable()
        {
            _ThisApartment = (Apartment) target;
            _Dimensions = _ThisApartment.Dimensions;
        }

        public override void OnInspectorGUI()
        {
            _ThisApartment.Height = EditorGUILayout.FloatField("Height (cm)", _ThisApartment.Height);

            var dimensions = EditorGUILayout.Vector2Field("Dimensions", _Dimensions.size).RoundCoordsToInt();
            _ThisApartment.FloorMaterial =
                (Material)EditorGUILayout.ObjectField("Floor Material", _ThisApartment.FloorMaterial, typeof(Material), false);
            _ThisApartment.WallMaterial =
                (Material) EditorGUILayout.ObjectField("Wall Material", _ThisApartment.WallMaterial, typeof(Material), false);


            var dimensionsRect = new Rect(-dimensions.x / 2, -dimensions.y / 2, dimensions.x, dimensions.y);

            if(_ThisApartment.IsApartmentInRect(dimensionsRect))
                _Dimensions = dimensionsRect;

            _ThisApartment.Dimensions = _Dimensions;
        }
    }
}