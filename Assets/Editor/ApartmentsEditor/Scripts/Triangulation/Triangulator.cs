using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    public abstract class Triangulator
    {
        public abstract Mesh CreateMesh(List<Vector2> contour, List<List<Vector2>> holes = null);
    }
}
