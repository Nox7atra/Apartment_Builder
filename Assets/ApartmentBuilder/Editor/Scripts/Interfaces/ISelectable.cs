using UnityEngine;

namespace Foxsys.ApartmentBuilder
{
    public interface ISelectable
    {
        void MoveTo(Vector2 position);
        void Delete();
        void DrawSelection(ApartmentBuilderGrid grid, Color color);
        void EndMoving(bool IsSnap = true);

    }
}