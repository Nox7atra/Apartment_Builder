using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foxsys.ApartmentEditor
{
    public interface IWallObject
    {
        bool TryAddObject(Vector2 position);
        void Draw(Vector2 position);
    }
}