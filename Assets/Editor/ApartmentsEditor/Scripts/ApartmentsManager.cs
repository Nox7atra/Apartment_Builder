using System;
using System.CodeDom;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Nox7atra.ApartmentEditor
{
    public class ApartmentsManager
    {
        private Apartment _CurrentApartment;

        private List<Apartment> _Apartments;

        public Apartment CurrentApartment
        {
            get
            {
                if (_CurrentApartment == null)
                {
                    _CurrentApartment = Apartment.Create("test");
                }
                return _CurrentApartment;
            }
        }
        private ApartmentsManager()
        {
            var objects = AssetDatabase.LoadAllAssetsAtPath(PathsConfig.Instance.PathToApartments);
            _Apartments = new List<Apartment>(objects.Length);
            foreach (Object obj in objects)
            {
                var apartment = obj as Apartment;
                if(apartment)
                    _Apartments.Add(apartment);
            }
        }

        private static ApartmentsManager _Instance;
        public static ApartmentsManager Instance
        {
            get { return _Instance ?? (_Instance = new ApartmentsManager()); }
        }
    }
}