using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    public class Apartment
    {
        #region properties
        public List<Room> Rooms
        {
            get
            {
                return _Rooms;
            }
        }
        public float Square
        {
            get
            {
                float result = 0;
                for (int i = 0; i < _Rooms.Count; i++)
                {
                    result += _Rooms[i].Square;
                }
                return result;
            }
        }
        #endregion

        #region attributes
        private List<Room> _Rooms;
        #endregion

        #region public methods
        public void Draw()
        {
            for(int i = 0; i < _Rooms.Count; i++)
            {
                _Rooms[i].Draw();
            }
        }
        #endregion

        #region constructor
        public Apartment()
        {
            _Rooms = new List<Room>();
        }
        #endregion
    }
}
