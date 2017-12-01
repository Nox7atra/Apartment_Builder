using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foxsys.ApartmentEditor
{
    [Serializable]
    public class Window : WallObject
    {
        [SerializeField]
        public float DistanceFromFloor;
    }
}