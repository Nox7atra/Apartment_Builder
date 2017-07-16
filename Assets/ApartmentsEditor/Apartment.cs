using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    public class Apartment
    {
        private Room[]  m_Rooms;

        public float Square
        {
            get
            {
                float result = 0;
                for(int i = 0; i < m_Rooms.Length; i++)
                {
                    result += m_Rooms[i].Square;
                }
                return result;
            }
        }
    }
    public class Room
    {
        private Type m_Type;
        private Vector2[] m_ContourPoints;
        
        public float Square
        {
            get
            {
                float result = 0;
                int pointsCount = m_ContourPoints.Length;
                for (int i = 0; i < pointsCount; i++)
                {
                    result +=
                        0.5f * (m_ContourPoints[i].x + m_ContourPoints[(i + 1) % pointsCount].x)
                             * (m_ContourPoints[i].y - m_ContourPoints[(i + 1) % pointsCount].y);
                }
                return result;
            }
        }
        public enum Type
        {
            Kitchen,
            Bathroom,
            Toilet,
            BathroomAndToilet
        }
    }

}
