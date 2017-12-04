using UnityEngine;

namespace Foxsys.ApartmentEditor
{
    public interface ISelectable
    {
        void MoveTo(Vector2 position);
        void Delete();
        void DrawSelection(Grid grid, Color color);
        void EndMoving();
    }
}