using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nox7atra.ApartmentEditor
{
    public class ApartmentsManager
    {
        #region properties
        public Apartment CurrentApartment
        {
            get
            {
                return _CurrentApartment;
            }
        }
        #endregion
        #region attributes
        private Apartment _CurrentApartment;
        #endregion
        #region constructor
        public ApartmentsManager()
        {
            _CurrentApartment = new Apartment();
        }
        #endregion

    }
}