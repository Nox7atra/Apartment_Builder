using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Foxsys.ApartmentBuilder
{
    public class ApartmentsManager
    {
        #region factory

        public Apartment CreateOrGetApartment(string name)
        {
            var fullpath = Path.Combine(PathsConfig.Instance.PathToApartments, name + ".asset");
            Apartment apartment = AssetDatabase.LoadAssetAtPath<Apartment>(fullpath);

            if (apartment == null)
            {
                apartment = Apartment.CreateInstance<Apartment>();
                apartment.Dimensions = new Rect(-Vector2.one * 500, Vector2.one * 1000);
                apartment.Height = 273;
                ProjectWindowUtil.CreateAsset(apartment, fullpath);
            }
            return apartment;
        }
        #endregion
        private Apartment _CurrentApartment;

        private List<Apartment> _Apartments;

        public Apartment CurrentApartment
        {
            get
            {
                if (_CurrentApartment == null)
                {
                    Refresh();
                    _CurrentApartment = _Apartments.Count > 0 ? _Apartments[0] : CreateOrGetApartment("default");
                }
                return _CurrentApartment;
            }
        }

        public List<Apartment> Apartments
        {
            get { return _Apartments; }
        }

        public void SelectApartment(Apartment apartment)
        {
            _CurrentApartment = apartment;
        }

        private ApartmentsManager()
        {
            Refresh();
        }

        public void Refresh()
        {
            var pathsToApartments = PathsConfig.Instance.PathToApartments;
            if (!Directory.Exists(pathsToApartments))
            {
                Directory.CreateDirectory(pathsToApartments);
            }
            string[] assets = Directory.GetFiles(PathsConfig.Instance.PathToApartments,"*.asset");
            _Apartments = new List<Apartment>(assets.Length);
            foreach (var asset in assets)
            {
                var apartment = AssetDatabase.LoadAssetAtPath<Apartment>(asset);
                if (apartment)
                    _Apartments.Add(apartment);
            }
            _CurrentApartment = null;
        }
        private static ApartmentsManager _Instance;
        public static ApartmentsManager Instance
        {
            get { return _Instance ?? (_Instance = new ApartmentsManager()); }
        }
    }
}