using UnityEngine;

namespace Foxsys.ApartmentEditor
{
    public interface ISelectable
    {
        void MoveTo(Vector2 position);
        void Delete();
        void DrawSelection(ApartmentEditorGrid grid, Color color);
        void EndMoving(bool IsSnap = true);
    }
}