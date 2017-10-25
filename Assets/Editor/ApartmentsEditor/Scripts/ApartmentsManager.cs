using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    public class ApartmentsManager
    {
        public Apartment CurrentApartment { get; private set; }

        public bool NeedToSave;

    
        private ApartmentsManager()
        {
            CurrentApartment = Apartment.Create();
        }

        public const string DataPath = "Assets/Editor/ApartmentsEditor/Data/";

        private static ApartmentsManager _Instance;
        public static ApartmentsManager Instance
        {
            get { return _Instance ?? (_Instance = new ApartmentsManager()); }
        }
    }
}