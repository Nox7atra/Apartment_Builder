using UnityEngine;
namespace Foxsys.ApartmentEditor
{
    public class NormalStateHotkeys : StateHotkeys
    {
        #region attributes
        NormalState _Parent;
        #endregion

        #region public methods
        public override void Use(KeyCode keyCode, bool isKeyDown)
        {
            base.Use(keyCode, isKeyDown);
            switch (keyCode)
            {
                case KeyCode.Delete:
                    Delete();
                    break;
                case KeyCode.S:
                    _Parent.SaveCurrentApartment();
                    break;
            }
        }
        #endregion

        #region hotkeys
        void Delete()
        {
            _Parent.DeleteSelected();
        }
        #endregion

        #region constructor
        public NormalStateHotkeys(NormalState parent)
        {
            _Parent = parent;
        }
        #endregion
    }
}