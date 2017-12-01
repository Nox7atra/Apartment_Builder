using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foxsys.ApartmentEditor
{
    [Serializable]
    public abstract class WallObject : ScriptableObject
    {
        #region fields

        [SerializeField]
        public float Width;
        [SerializeField]
        public float Height;

        #endregion
    }
}